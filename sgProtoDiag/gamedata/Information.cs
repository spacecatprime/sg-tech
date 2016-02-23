using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamedata
{
    class InformationManager : util.Singleton<InformationManager>
    {
        internal Information LoadInformation(System.Xml.XmlElement e)
        {
            return new Information();
        }
    }

    class Information
    {
    }
}
