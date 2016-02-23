using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    /// <summary>
    /// either a NPC or player that can be at a location, contain inventory objects, and use certain objects
    /// </summary>
    public class GameActor : GameObject
    {
        public string Name { get; private set; }
        
        public GameActor(string aName)
        {
            Name = aName;
        }

        public GameActor(string aName, ulong aID) 
            : base(aID)
        {
            Name = aName;
        }

        public GameActor(LuaInterface.LuaTable aTable)
            : base(aTable)
        {
            Name = aTable["Name"].ToString();
        }
    }
}
