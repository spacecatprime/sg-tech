using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamedata
{
    interface IObjectDepot
    {
        BaseType FindObject(string aType, string aAttribute, string aValue);
    }

    class ObjectDepot : util.Singleton<ObjectDepot>
    {
        private List<IObjectDepot> m_objDepot = new List<IObjectDepot>();

        public bool RegisterObjectDepot(IObjectDepot aObjectDepot)
        {
            m_objDepot.Add(aObjectDepot);
            return true;
        }

        public BaseType FindObject(string aType, string aAttribute, string aValue)
        {
            foreach (IObjectDepot depot in m_objDepot)
            {
                BaseType data = depot.FindObject(aType, aAttribute, aValue);
                if (null != data)
                {
                    return data;
                }
            }
            return null;
        }

        public BaseType FindObjectWithLabel(string aLabel)
        {
            List<BaseType> objList = FindObjectListWithLabel(aLabel);
            if (objList.Count > 0)
            {
                return objList[0];
            }
            return null;
        }

        public List<BaseType> FindObjectListWithLabel(string aLabel)
        {
            List<BaseType> objList = new List<BaseType>();

            // \[(.*?)\] matches -> type[@attr=value]
            System.Text.RegularExpressions.Regex regEx =
                new System.Text.RegularExpressions.Regex(@"\[@(.*?)\]");

            System.Text.RegularExpressions.MatchCollection regMatches =
                regEx.Matches(aLabel);

            if (regMatches.Count > 0)
            {
                foreach (System.Text.RegularExpressions.Match match in regMatches)
                {
                    string type = aLabel.Replace(match.Value, "");

                    string pair = match.Value.Trim();
                    pair = match.Value.Replace("[@", "");
                    pair = match.Value.Replace("]", "");

                    // attribute='value'
                    string[] parts = pair.Split(new char[] { '=', '\'' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 2)
                    {
                        string value = string.Join(" ", parts.Skip(1).ToArray());
                        string key = parts[0];

                        BaseType objType = FindObject(type, key, value);
                        if (null != objType)
                        {
                            objList.Add(objType);
                        }
                    }
                }
            }

            return objList;
        }
    }
}
