using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    /// <summary>
    /// a place, venue, location that actors, inventory objects, and usable objects will be
    /// </summary>
    public class GameLocation : GameObject
    {
        List<GameObject> m_objList = new List<GameObject>();
        public string Name { get; private set; }

        public GameLocation(string name, ulong id)
            : base(id)
        {
            Name = name;
        }

        public GameLocation(LuaInterface.LuaTable aTable) 
            : base(aTable) 
        {
            Name = aTable["Name"].ToString();
        }

        internal bool Insert(GameObject aGameObj)
        {
            if (m_objList.Contains(aGameObj) == false)
            {
                aGameObj.PropertyList.Set("world.location.id", ID);
                m_objList.Add(aGameObj);
                return true;
            }
            return false;
        }
    }
}
