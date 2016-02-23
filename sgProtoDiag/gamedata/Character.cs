using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

  //<character name="Charleston 'Chip' Garson"
  //           id="chip"
  //           gender="male"
  //           startLocation="hotel"
  //           storyScript="story.charleston.garson.xml"
  //           nationality="British"
  //           objectType="gamelogic.GameActor">
  //  <inventory>
  //    <item name="Special Plans" type="item.inspect.plans.science" />
  //  </inventory>
  //  <motives>
  //    <motive name="Paranoid" degree="0.8" control="list.motive.controls.paranoia"/>
  //  </motives>
  //  <information>
  //    <tidbit name="The goods on Wendel" type="info.data.gossip" target="character[@id='profwen']" quality="0.7"/>
  //    <tidbit name="The look away" type="info.favor.police" quality="0.25"/>
  //  </information>
  //</character>

namespace sgProtoDiag.gamedata
{
    class CharacterManager : util.Singleton<CharacterManager>, IObjectDepot
    {
#region STATIC
        public bool DoTest()
        {
            try
            {
                XmlDocument d = new XmlDocument();
                d.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\gamedata\world.xml");

                CharacterManager chrmgr = new CharacterManager();
                if (chrmgr.Load(d) == false)
                {
                    return false;
                }

                // do more testing here

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
#endregion
        BaseType IObjectDepot.FindObject(string aType, string aKey, string aValue)
        {
            if (aType == "character")
            { 
                Character ch;
                if( aKey.ToLower() == "id" && m_characterMap.TryGetValue(aValue, out ch))
                {
                    return ch;
                }
            }
            return null;
        }

        private bool Load(XmlDocument d)
        {
            XmlNodeList characterList = d.SelectNodes("//characters");
            foreach (XmlElement e in characterList.OfType<XmlElement>())
            {
                Character ch = LoadCharacter(e);
                if (null == ch)
                {
                    return false;
                }
                m_characterMap.Add(ch.Id, ch);
            }
            return true;
        }

        private Character LoadCharacter(XmlElement e)
        {
            Character ch = new Character();

            ch.Name = e.GetAttribute("name", "");
            ch.Id = e.GetAttribute("id", "");
            ch.Info = new Information();
            ch.Inv = new Inventory();
            ch.Gender = (Character.GenderType)BaseType.ParseEnum(e, typeof(Character.GenderType), "gender");
            ch.CurrentLocation = (Location)ObjectDepot.GetInstance().FindObjectWithLabel(e.GetAttribute("startLocation", ""));

            XmlNodeList infoList = e.SelectNodes("./information");
            foreach (XmlElement eInfo in infoList.OfType<XmlElement>())
            {
                ch.Info = InformationManager.GetInstance().LoadInformation(eInfo);
                break;
            }

            XmlNodeList inv = e.SelectNodes("./inventory");
            foreach (XmlElement eInv in inv.OfType<XmlElement>())
            {
                ch.Inv = InventoryManager.GetInstance().LoadInformation(eInv);
                break;
            }

            return ch;
        }

        private Dictionary<string, Character> m_characterMap = new Dictionary<string, Character>();
    }

    //<character name="Charleston 'Chip' Garson"
    //           id="chip"
    //           gender="male"
    //           startLocation="hotel"
    //           storyScript="story.charleston.garson.xml"
    //           nationality="British"
    //           objectType="gamelogic.GameActor">
    class Character : BaseType
    {
        public enum GenderType
        {
            Male,
            Female
        }

        public Information Info { get; set; }
        public Inventory Inv { get; set; }
        public List<Motive> Motives { get; private set; }
        public string Id { get; set; }
        public GenderType Gender { get; set; }
        public Location CurrentLocation { get; set; }

        public Character()
        {
            Motives = new List<Motive>();
        }
    }
}
