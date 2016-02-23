using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace sgProtoDiag.gamedata
{
    class TypeManager
    {
#region UNIT TEST
        public static void DoTest()
        {
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\gamedata\world.xml");
            gamedata.TypeManager tm = new gamedata.TypeManager();
            tm.LoadTypesFromDoc(doc);

            gamedata.TypeData td1 = tm.GetType(new util.PropertyLabel("item.weapon"));
            bool isWeapon = td1.GetValue<bool>("isweapon");
            bool isBreakable = td1.GetValue<bool>("isbreakable");
        }
#endregion

#region STATIC
        private static TypeManager s_TypeManager = new TypeManager();
        public static TypeManager Singleton()
        {
            return s_TypeManager;
        }
#endregion

        private Dictionary<string, TypeData> m_typeDepot = new Dictionary<string, TypeData>();



        /// <summary>
        /// 
        /// </summary>
        /// <param name="aLabel"></param>
        /// <param name="aTypeData"></param>
        /// <returns></returns>
        public bool RegisterType(util.PropertyLabel aLabel, TypeData aTypeData)
        {
            if (m_typeDepot.ContainsKey(aLabel.Label))
            {
                return false;
            }
            m_typeDepot.Add(aLabel.Label, aTypeData);
            return true;
        }

        /// <summary>
        /// gets the key for a TypeData, if registered
        /// </summary>
        /// <param name="aTypeData"></param>
        /// <returns></returns>
        public util.PropertyLabel GetPropertyLabel(TypeData aTypeData)
        {
            try
            {
                if (m_typeDepot.ContainsValue(aTypeData))
                {
                    var keyFor =
                          from entry in m_typeDepot
                          where entry.Value == aTypeData
                          select entry.Key;

                    IEnumerator<string> keyVal = keyFor.GetEnumerator();
                    keyVal.MoveNext();
                    return new util.PropertyLabel(keyVal.Current);
                }
            }
            catch (System.Exception){}
            return util.PropertyLabel.INVALID;
        }

        /// <summary>
        /// find all attributes and generate a new type composing all the attributes
        /// </summary>
        /// <param name="aLabel"></param>
        /// <returns></returns>
        public TypeData GetCompositeType(util.PropertyLabel aLabel)
        {
            TypeData typeData = new TypeData();
            string[] parts = aLabel.Label.Split('.');
            for (int idx = 1; idx <= parts.Length; ++idx)
            {
                string label = string.Join(".", parts, 0, idx);
                TypeData data = GetType(new util.PropertyLabel(label));
                if (data != null)
                {
                    typeData.CloneType(data);
                }
            }
            return typeData;
        }

        /// <summary>
        /// gets a registered type from the depot
        /// </summary>
        /// <param name="aLabel"></param>
        /// <returns></returns>
        public TypeData GetType(util.PropertyLabel aLabel)
        {
            if (m_typeDepot.ContainsKey(aLabel.Label))
            {
                return m_typeDepot[aLabel.Label];
            }
            return null;
        }

        /// <summary>
        /// fills out the depot using a XML document
        /// </summary>
        /// <param name="aDoc"></param>
        /// <returns></returns>
        public bool LoadTypesFromDoc(System.Xml.XmlDocument aDoc)
        {
            int typeCount = m_typeDepot.Count;
            XmlNodeList typeListNodes = aDoc.SelectNodes("//types");
            foreach (XmlNode typeList in typeListNodes)
            {
                XmlNodeList types = typeList.SelectNodes("type");
                foreach (XmlNode type in types)
                {
                    TypeData tdata = new TypeData();
                    XmlNodeList attribList = type.SelectNodes("attribute");
                    foreach (XmlNode att in attribList)
                    { 
                        XmlElement a = att as XmlElement;
                        tdata.SetAttribute(a.GetAttribute("key"), a.GetAttribute("value"));
                    }
                    XmlElement t = type as XmlElement;
                    RegisterType(new util.PropertyLabel(t.GetAttribute("name")), tdata);
                }
            }
            return m_typeDepot.Count > typeCount;
        }
    }

    /// <summary>
    /// hierarchical data types
    /// </summary>
    public class TypeData
    {
        private Dictionary<string, object> m_attributeList = new Dictionary<string, object>();

        public void CloneType(TypeData aData)
        {
            foreach (KeyValuePair<string, object> kvp in aData.m_attributeList)
            {
                if (m_attributeList.ContainsKey(kvp.Key))
                {
                    m_attributeList[kvp.Key] = kvp.Value;
                }
                else
                {
                    m_attributeList.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public void SetAttribute(string aKey, object aValue)
        {
            if (m_attributeList.ContainsKey(aKey))
            {
                m_attributeList[aKey] = aValue;
            }
            else
            {
                m_attributeList.Add(aKey, aValue);
            }
        }

        public Dictionary<string, object>.Enumerator AttributeIterator
        {
            get { return m_attributeList.GetEnumerator(); }
        }

        public T GetValue<T>(string aKey)
        {
            object val = GetValueRaw(aKey);
            if (val != null)
            {
                try
                {
                    return (T)System.Convert.ChangeType(val, typeof(T));
                }
                catch (System.Exception ex)
                {
                    util.Monitoring.Logging.Exception(ex);
                }
            }
            return Activator.CreateInstance<T>();
        }

        public object GetValueRaw(string aKey)
        {
            object val = null;
            if (m_attributeList.TryGetValue(aKey, out val))
            {
                return val;
            }
            return null;
        }
    }
}
