using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace sgProtoDiag.gamedata
{
    class LocationManager : util.Singleton<LocationManager>, IObjectDepot
    {
#region Unit Test
        public bool DoTest()
        {
            try
            {
                XmlDocument d = new XmlDocument();
                d.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\gamedata\world.xml");

                LocationManager locmgr = new LocationManager();
                if (locmgr.Load(d) == false)
                {
                    return false;
                }
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
#endregion

        public bool Load(System.Xml.XmlDocument aDoc)
        {
            XmlNodeList locs = aDoc.SelectNodes("//locations");
            if(locs.Count > 0)
            {
                foreach( XmlElement l in locs.OfType<XmlElement>())
                {
                    Location loc = LoadLocation(l as XmlElement);
                    if (null == loc)
                    {
                        return false;
                    }
                    m_locations.Add(loc);
                }
            }
            return true;
        }

        public Location LoadLocation(XmlElement aElement)
        {
            //<location name="Hotel Room 101"
            //setting="interior"
            //owner="character[@id='gina']"
            //locked="true">
            //</location>
            if (aElement.Name != "location")
            { 
                // warn
                return null;
            }
            Location loc = new Location();
            loc.Name = aElement.GetAttribute("name","");
            loc.Setting = (Location.SettingType)BaseType.ParseEnum(aElement, typeof(Location.SettingType), "setting");
            loc.Locked = BaseType.ParseBool(aElement, "locked", true);
            
            if( aElement.HasAttribute("owner") )
            {
                loc.Owner = (Character)ObjectDepot.GetInstance().FindObjectWithLabel(aElement.GetAttribute("owner", ""));
                // TODO: warn if missing?
            }

            XmlNodeList locs = aElement.SelectNodes("//location");
            if(locs.Count > 0)
            {
                foreach( XmlElement l in locs.OfType<XmlElement>())
                {
                    Location childRoom = LoadLocation(l);
                    if (null != childRoom)
                    {
                        loc.AddChildRoom(childRoom);
                    }
                }
            }

            return loc;
        }

        public BaseType FindObject(string aType, string aKey, string aValue)
        {
            if (aType == "location")
            {
                if (aKey.ToLower().ToLower() == "name")
                {
                    return m_locations.Find(delegate(Location loc)
                    {
                        return loc.Name == aValue;
                    });
                }
            }
            return null;
        }

        private List<Location> m_locations = new List<Location>();
    }

    /// <summary>
    /// a place where 
    /// </summary>
    class Location : BaseType
    {
        public enum SettingType 
        {
            Interior,
            Exterior 
        }

        public gamedata.Character Owner { get; set; }
        public SettingType Setting { get; set; }
        public bool Locked { get; set; }

        private List<Location> m_locations = new List<Location>();

        internal bool IsValid()
        {
            // TODO: verification?
            return true;
        }

        internal void AddChildRoom(Location childRoom)
        {
            m_locations.Add(childRoom);
        }
    }
}
