using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace sgProtoDiag.gamedata
{
    /// <summary>
    /// manages named lists
    /// </summary>
    class ListManager
    {
#region UNIT TEST
        public static void DoTest()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\gamedata\world.xml");
            gamedata.TypeManager.Singleton().LoadTypesFromDoc(doc);

            doc = new System.Xml.XmlDocument();
            doc.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\gamedata\world.xml");
            gamedata.ListManager listMgr = new gamedata.ListManager();
            listMgr.LoadFromDoc(doc);

            ListData listGreed = listMgr.GetListData(new sgProtoDiag.util.PropertyLabel("list.motive.controls.greed"));
            TypeData giftDebug = gamedata.TypeManager.Singleton().GetType(new sgProtoDiag.util.PropertyLabel("item.gift.debug"));

            if (listGreed.TestSatified(giftDebug))
            {
                Console.WriteLine("it works!");
            }
        }
#endregion

        // PropertyLabel to ListData
        private Dictionary<string, ListData> m_listDepot = new Dictionary<string, ListData>();

        public bool RegisterList(util.PropertyLabel aLabel, ListData aListData)
        {
            if (m_listDepot.ContainsKey(aLabel.Label))
            {
                return false;
            }
            m_listDepot.Add(aLabel.Label, aListData);
            return true;
        }

        public ListData GetListData(util.PropertyLabel aLabel)
        {
            ListData listdata;
            if (m_listDepot.TryGetValue(aLabel.Label,out listdata))
            {
                return listdata;
            }
            return null;
        }

        internal bool LoadFromDoc(System.Xml.XmlDocument aDoc)
        {
            int listCount = m_listDepot.Count;
            XmlNodeList listDataNodes = aDoc.SelectNodes("//lists");
            foreach (XmlNode listData in listDataNodes)
            {
                XmlNodeList listNodes = listData.SelectNodes("list");
                foreach (XmlNode list in listNodes)
                {
                    ListData data = new ListData();
                    XmlNodeList bitNodes = list.SelectNodes("bit");
                    foreach (XmlNode bit in bitNodes)
                    {
                        XmlElement b = bit as XmlElement;
                        data.SetValue(b.GetAttribute("value"));
                    }
                    XmlElement l = list as XmlElement;
                    RegisterList(new util.PropertyLabel(l.GetAttribute("name")), data);
                }
            }
            return m_listDepot.Count > listCount;
        }
    }

    class ListData
    {
#region Compare Types
        public interface ICompareSatified
        {
            /// <summary>
            /// checks to see if to object values satisfies this compare type
            /// </summary>
            /// <param name="lhs"></param>
            /// <param name="rhs"></param>
            /// <returns></returns>
            bool IsSatified(object lhs, object rhs);
        }

        public static object SmartConvert(object aValue)
        {
            if (aValue.GetType() == typeof(string))
            {
                // try some number conversions first
                if (aValue.ToString().Contains("."))
                {
                    try
                    {
                        return System.Convert.ToDouble(aValue);
                    }
                    catch (System.Exception) { }
                }
                try
                {
                    return System.Convert.ToInt64(aValue);
                }
                catch (System.Exception) { }
            }
            return aValue;
        }

        public struct AttributeCompare
        {
            public string attributeName;
            public object targetValue;
            private ICompareSatified comparer;

            public AttributeCompare(string aAttName, object aValue, ICompareSatified aComparer)
            {
                attributeName = aAttName;
                comparer = aComparer;
                targetValue = SmartConvert(aValue);
            }

            public bool IsSatified(TypeData aInstance)
            {
                object attVal = aInstance.GetValueRaw(attributeName);
                if (null == attVal || null == targetValue)
                {
                    return false;
                }
                return comparer.IsSatified(SmartConvert(attVal), targetValue);
            }
        }

        protected class CompareBase : ICompareSatified
        {
            protected bool GetValue<T>(object aValueObj, ref T aValueNum)
            {
                try
                {
                    if (aValueObj is T)
                    {
                        aValueNum = (T)aValueObj;
                        return true;
                    }
                }
                catch (System.Exception){}
                return false;
            }

            public bool IsSatified(object lhs, object rhs)
            {
                if (lhs.GetType() != rhs.GetType())
                {
                    return false;
                }
                return TestSatified(lhs, rhs);
            }

            public virtual bool TestSatified(object lhs, object rhs)
            {
                // must be impl by derived class
                throw new System.NotImplementedException();
            }
        }

        class CompareEquals : CompareBase
        {
            public override bool TestSatified(object lhs, object rhs)
            {
                long longLHS = 0;
                if (GetValue<long>(lhs, ref longLHS))
                {
                    return long.Equals(lhs, rhs);
                }
                double doubleLHS = 0.0;
                if (GetValue<double>(lhs, ref doubleLHS))
                {
                    return double.Equals(lhs, rhs);
                }
                return false;
            }
        }

        class CompareGreater : CompareBase
        {
            public override bool TestSatified(object lhs, object rhs)
            {
                long longLHS = 0;
                if (GetValue<long>(lhs, ref longLHS))
                {
                    return longLHS.CompareTo(rhs) > 0;
                }
                double doubleLHS = 0.0;
                if (GetValue<double>(lhs, ref doubleLHS))
                {
                    return doubleLHS.CompareTo(rhs) > 0;
                }
                if (lhs.GetType() == typeof(string))
                {
                    return string.Compare(lhs.ToString(), rhs.ToString()) > 0;
                }
                return false;
            }
        }

        class CompareLesser : CompareBase
        {
            public override bool TestSatified(object lhs, object rhs)
            {
                return (new CompareGreater().TestSatified(lhs, rhs)) == false;
            }
        }

        class CompareNotEqual : CompareBase
        {
            public override bool TestSatified(object lhs, object rhs)
            {
                return (new CompareEquals().TestSatified(lhs, rhs)) == false;
            }
        }
#endregion

        private util.PropertyLabel m_prop = null;
        private List<AttributeCompare> m_compares = new List<AttributeCompare>();

        public void SetValue(string aValue)
        {
            // \[(.*?)\] matches [@<any string>]
            System.Text.RegularExpressions.Regex regEx = 
                new System.Text.RegularExpressions.Regex(@"\[@(.*?)\]");

            System.Text.RegularExpressions.MatchCollection regMatches = 
                regEx.Matches(aValue);

            string propLabel = aValue;
            if (regMatches.Count > 0)
            {
                foreach (System.Text.RegularExpressions.Match match in regMatches)
                {
                    propLabel = propLabel.Replace(match.Value, "");

                    string matchVal = match.Value.Trim();
                    matchVal = matchVal.Replace("[@", "");
                    matchVal = matchVal.Replace("]", "");

                    if (matchVal.Contains("=="))
                    {
                        string[] fields = matchVal.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        m_compares.Add(new AttributeCompare(fields[0], fields[1], new CompareEquals()));
                    }
                    else if (matchVal.Contains("!="))
                    {
                        string[] fields = matchVal.Split(new char[] { '!','=' }, StringSplitOptions.RemoveEmptyEntries);
                        m_compares.Add(new AttributeCompare(fields[0], fields[1], new CompareNotEqual()));
                    }
                    else if (matchVal.Contains("<"))
                    {
                        string[] fields = matchVal.Split(new char[] { '<' }, StringSplitOptions.RemoveEmptyEntries);
                        m_compares.Add(new AttributeCompare(fields[0], fields[1], new CompareLesser()));
                    }
                    else if (matchVal.Contains(">"))
                    {
                        string[] fields = matchVal.Split(new char[] { '>' }, StringSplitOptions.RemoveEmptyEntries);
                        m_compares.Add(new AttributeCompare(fields[0], fields[1], new CompareGreater()));
                    }
                }
            }

            m_prop = new util.PropertyLabel(propLabel);
        }

        public bool TestSatified(TypeData aInst)
        {
            util.PropertyLabel propLabel = 
                gamedata.TypeManager.Singleton().GetPropertyLabel(aInst);

            if (null == propLabel)
            {
                return false;
            }

            if (propLabel.DoesMatch(m_prop) == false)
            {
                return false;
            }

            foreach (AttributeCompare comp in m_compares)
            {
                if (comp.IsSatified(aInst) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
