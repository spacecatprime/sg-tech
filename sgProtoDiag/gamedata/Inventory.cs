using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamedata
{
    class InventoryManager : util.Singleton<InventoryManager>
    {
        internal Inventory LoadInformation(System.Xml.XmlElement eInv)
        {
            return new Inventory();
        }
    }

    class Inventory
    {
    }
}
