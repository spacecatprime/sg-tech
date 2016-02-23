using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace sgProtoDiag.gamedata
{
    public class BaseType
    {
#region Util Parsers
        public static object ParseEnum(XmlElement aElem, Type aEnumType, string aLabel)
        {
            string szEnum = aElem.GetAttribute(aLabel, "");
            if (string.IsNullOrEmpty(szEnum))
            {
                return null;
            }
            return System.Enum.Parse(aEnumType, szEnum);
        }

        public static bool ParseBool(XmlElement aElement, string aLabel, bool aDefault)
        {
            if (aElement.HasAttribute(aLabel))
            {
                string szAtt = aElement.GetAttribute(aLabel);
                return szAtt.ToLowerInvariant() == "true" || szAtt.Trim() == "1";
            }
            return aDefault;
        }
#endregion

        public string Name { get; set; }
    }
}
