using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    class GameItem : GameObject
    {
        public string Name { get; private set; }

        public GameItem(string name, ulong id)
            : base(id)
        {
            Name = name;
        }

        public GameItem(LuaInterface.LuaTable aTable) 
            : base(aTable)
        {
            Name = aTable["Name"].ToString();
        }

    }
}
