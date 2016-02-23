using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;

namespace sgProtoDiag.gamedata
{
    public class ChatMapperData
    {
        [DebuggerDisplay("Name = {title}, Value = {value}")]
        public struct Field
        {
            public string type;
            public string hint;
            public string title;
            public string value;
        }

        private enum AssetTypeOffset : int
        { 
            Actor = 1000,
            Item = 2000,
            Location = 3000,
        };

        public class Asset
        {
            public int ID { get; set; }
            public System.ConsoleColor NodeColor { get; set; }

            // common fields
            public string Name { get; set; }
            public string Description { get; set; }

            // common loading fields
            protected delegate void OnFieldLoad(Field aField);
            protected void LoadFields(XmlElement aAsset, OnFieldLoad aOnFieldLoad)
            {
                string szID = aAsset.GetAttribute("ID");
                if (false == string.IsNullOrEmpty(szID))
                {
                    ID = int.Parse(szID);
                }

                XmlNodeList fields = aAsset.SelectNodes("./Fields/Field");
                foreach (XmlNode fld in fields)
                {
                    try
                    {
                        XmlElement eFld = fld as XmlElement;
                        Field newField = new Field();
                        newField.type = eFld.GetAttribute("Type", "");
                        newField.hint = eFld.GetAttribute("Hint", "");
                        newField.title = eFld.ChildNodes[0].InnerText;
                        newField.value = eFld.ChildNodes[1].InnerText;
                        m_fieldList.Add(newField);

                        if (!HandleField(newField) 
                            && aOnFieldLoad != null 
                            && !string.IsNullOrEmpty(newField.value))
                        {
                            aOnFieldLoad(newField);
                        }
                    }
                    catch (System.Exception) { }
                }
            }

            protected bool HandleField(Field aField)
            {
                if (aField.type == "Text" && aField.title == "Name")
                {
                    Name = aField.value;
                    return true;
                }
                if (aField.type == "Text" && aField.title == "Description")
                {
                    Description = aField.value;
                    return true;
                }
                return false;
            }

            public List<Field> m_fieldList = new List<Field>();
        }

        public class Actor : Asset
        {
            public enum GenderType { Male, Female, UNKNOWN }

            public bool IsPlayer { get; set; }
            public int Age { get; set; }
            public GenderType Gender { get; set; }

            public void Load(XmlElement aActor)
            {
                IsPlayer = false;
                Age = 0;
                Gender = GenderType.UNKNOWN;

                if (null != aActor)
                {
                    LoadFields(aActor, OnNewField);
                }
            }

            protected void OnNewField(Field aField)
            {
                switch (aField.title)
                {
                    case "IsPlayer": IsPlayer = (aField.value == "True"); break;
                    case "Age": Age = int.Parse(aField.value); break;
                    case "Gender": Gender = (GenderType)Enum.Parse(typeof(GenderType), aField.value); break;
                }
                // handle "Pictures"?
            }
        }

        public class Item : Asset
        {
            public string Purpose { get; set; }
            public string Scene { get; set; }
            public bool InInventory { get; set; }

            public void Load(XmlElement aItem)
            {
                Purpose = "";
                Scene = "";
                InInventory = false;

                if (null != aItem)
                {
                    LoadFields(aItem, OnNewField);
                }
            }

            protected void OnNewField(Field aField)
            {
                switch (aField.title)
                {
                    case "Purpose": Purpose = aField.value; break;
                    case "Scene": Scene = aField.value; break;
                    case "In Inventory": InInventory = (aField.value == "True"); break;
                }
                // handle "Pictures"?
            }
        }

        public class Location : Asset
        {
            public string Purpose { get; set; }
            public string Scene { get; set; }

            public void Load(XmlElement aItem)
            {
                Purpose = "";
                Scene = "";

                if (null != aItem)
                {
                    LoadFields(aItem, OnNewField);
                }
            }

            protected void OnNewField(Field aField)
            {
                switch (aField.title)
                {
                    case "Purpose": Purpose = aField.value; break;
                    case "Scene": Scene = aField.value; break;
                }
                // handle "Pictures"?
            }
        }

        public class UserVariable : Asset
        {
            public string InitialValue { get; set; }
            public string ValueType { get; set; }

            public void Load(XmlElement aItem)
            {
                InitialValue = "";

                if (null != aItem)
                {
                    LoadFields(aItem, OnNewField);
                }
            }

            protected void OnNewField(Field aField)
            {
                switch (aField.title)
                {
                    case "Initial Value":
                        ValueType = aField.type;
                        InitialValue = aField.value; 
                        break;
                }
            }
        }

        [DebuggerDisplay("{Title}, {OutgoingLinkList.Count}, root={IsRoot}, {DialogueText}")]
        public class DialogEntry : Asset
        {
            public enum ConditionPriorityType { Normal, AboveNormal, BelowNormal, High, Low };

            public struct OutgoingLink
            {
                public int conversationID;
                public int originDialogID;
                public int destinationDialogID;
                public bool isConnector;
            }

            public int ConversationID { get; set; }
            public bool IsRoot { get; set; }
            public bool IsGroup { get; set; }
            public bool DelaySimStatus { get; set; }
            public string FalseCondtionAction { get; set; }
            public string Title { get; set; }
            public string MenuText { get; set; }
            public string DialogueText { get; set; }
            public string Parenthetical { get; set; }
            public ConditionPriorityType ConditionPriority { get; set; }

            // flow controls
            public List<OutgoingLink> OutgoingLinkList { get; set; }
            public string UserScript { get; set; }
            public string ConditionsScipt { get; set; }

            public DialogEntry()
            {
                OutgoingLinkList = new List<OutgoingLink>();
            }

            public void Load(XmlElement aItem)
            {
                if (null == aItem)
                {
                    return;
                }

                ConversationID = int.Parse(aItem.GetAttribute("ConversationID"));
                IsRoot = bool.Parse(aItem.GetAttribute("IsRoot"));
                IsGroup = bool.Parse(aItem.GetAttribute("IsGroup"));
                DelaySimStatus = bool.Parse(aItem.GetAttribute("DelaySimStatus"));
                FalseCondtionAction = aItem.GetAttribute("FalseCondtionAction");
                ConditionPriority = (ConditionPriorityType)Enum.Parse(typeof(ConditionPriorityType), aItem.GetAttribute("ConditionPriority"));
                LoadFields(aItem, OnNewField);

                XmlNodeList links = aItem.SelectNodes("./OutgoingLinks/Link");
                foreach (XmlNode link in links)
                {
                    OutgoingLink outlink = new OutgoingLink();
                    outlink.conversationID = int.Parse(link.Attributes.GetNamedItem("ConversationID", "").Value);
                    outlink.originDialogID = int.Parse(link.Attributes.GetNamedItem("OriginDialogID", "").Value);
                    outlink.destinationDialogID = int.Parse(link.Attributes.GetNamedItem("DestinationDialogID", "").Value);
                    outlink.isConnector = bool.Parse(link.Attributes.GetNamedItem("IsConnector", "").Value);
                    OutgoingLinkList.Add(outlink);
                }

                ConditionsScipt = aItem.SelectSingleNode("./ConditionsString").InnerText;
                UserScript = aItem.SelectSingleNode("./UserScript").InnerText;
            }

            protected void OnNewField(Field aField)
            {
                switch (aField.title)
                {
                    case "Title": Title = aField.value; break;
                    case "Menu Text": MenuText = aField.value; break;
                    case "Dialogue Text": DialogueText = aField.value; break;
                    case "Parenthetical": Parenthetical = aField.value; break;
                }
            }
        }

        public class ConversationData : Asset
        {
            public string Scene { get; set; }
            public string Title { get; set; }
            public string Purpose { get; set; }
            public int TalkerID { get; set; }
            public int ConversantID { get; set; }
            public List<DialogEntry> DialogList { get; private set; }

            public ConversationData()
            {
                DialogList = new List<DialogEntry>();
            }

            public void Load(XmlElement aConversationData)
            {
                if (null != aConversationData)
                {
                    ID = int.Parse(aConversationData.GetAttribute("ID"));
                    LoadFields(aConversationData, OnNewField);
                }

                XmlNodeList entries = aConversationData.SelectNodes("./DialogEntries/DialogEntry");
                foreach (var dialog in entries)
                {
                    DialogEntry de = new DialogEntry();
                    de.Load(dialog as XmlElement);
                    DialogList.Add(de);
                }
            }

            protected void OnNewField(Field aField)
            {
                switch (aField.title)
                {
                    case "Scene": Scene = aField.value; break;
                    case "Description": Description = aField.value; break;
                    case "Title": Title = aField.value; break;
                    case "Purpose": Purpose = aField.value; break;
                    case "Actor": TalkerID = int.Parse(aField.value); break;
                    case "Conversant": ConversantID = int.Parse(aField.value); break;
                }
            }

        }

        public class Project
        {
            public bool Load(string aFilename)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(aFilename);
                    return Load(doc);
                }
                catch (System.Exception ex)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.Write(ex.ToString());
                    return false;
                }
            }

            public bool Load(XmlDocument aDoc)
            {
                try
                {
                    LoadProjectSettings(aDoc);
                    LoadAssets(aDoc);
                    return true;
                }
                catch (System.Exception ex)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.Write(ex.ToString());
                    return false;
                }
            }

            private void LoadAssets(XmlDocument doc)
            {
                XmlNodeList assets = doc.SelectNodes("//Assets");
                foreach (XmlNode assetList in assets)
                {
                    foreach (XmlNode kid in assetList)
                    {
                        if (kid.Name == "Actors")
                        {
                            XmlNodeList nlActors = kid.SelectNodes("./Actor");
                            foreach (XmlNode actor in nlActors)
                            {
                                Actor a = new Actor();
                                a.Load(actor as XmlElement);
                                a.ID += (int)AssetTypeOffset.Actor;
                                m_actorList.Add(a);
                            }
                        }
                        else if (kid.Name == "Items")
                        {
                            XmlNodeList nlItems = kid.SelectNodes("./Item");
                            foreach (XmlNode item in nlItems)
                            {
                                Item i = new Item();
                                i.Load(item as XmlElement);
                                i.ID += (int)AssetTypeOffset.Item;
                                m_itemList.Add(i);
                            }
                        }
                        else if (kid.Name == "Locations")
                        {
                            XmlNodeList nlLocations = kid.SelectNodes("./Location");
                            foreach (XmlNode loc in nlLocations)
                            {
                                Location l = new Location();
                                l.Load(loc as XmlElement);
                                l.ID += (int)AssetTypeOffset.Location;
                                m_locationList.Add(l);
                            }
                        }
                        else if (kid.Name == "UserVariables")
                        {
                            XmlNodeList nlUserVariables = kid.SelectNodes("./UserVariable");
                            foreach (XmlNode usr in nlUserVariables)
                            {
                                UserVariable u = new UserVariable();
                                u.Load(usr as XmlElement);
                                m_variableList.Add(u);
                            }
                        }
                        else if (kid.Name == "Conversations")
                        {
                            XmlNodeList nlConversations = kid.SelectNodes("./Conversation");
                            foreach (XmlNode conv in nlConversations)
                            {
                                ConversationData c = new ConversationData();
                                c.Load(conv as XmlElement);
                                m_conversationList.Add(c);
                            }
                        }
                    }
                }
            }

            private void LoadProjectSettings(XmlDocument doc)
            {
                // load settings
                // Title
                // Version
                // Author
            }

            //
            // Properties
            //
            public List<Actor>.Enumerator ActorList { get { return m_actorList.GetEnumerator(); } }
            public List<Item>.Enumerator ItemList { get { return m_itemList.GetEnumerator(); } }
            public List<Location>.Enumerator LocationList { get { return m_locationList.GetEnumerator(); } }
            public List<UserVariable>.Enumerator UserVariableList { get { return m_variableList.GetEnumerator(); } }
            public List<ConversationData>.Enumerator ConversationList { get { return m_conversationList.GetEnumerator(); } }
            public string Title { get; set; }
            public string Version { get; set; }
            public string Author { get; set; }

            //
            // Data
            // 
            private List<Actor> m_actorList = new List<Actor>();
            private List<Item> m_itemList = new List<Item>();
            private List<Location> m_locationList = new List<Location>();
            private List<UserVariable> m_variableList = new List<UserVariable>();
            private List<ConversationData> m_conversationList = new List<ConversationData>();
        }

        public static void DoTest()
        {
            ChatMapperData.Project proj = new ChatMapperData.Project();
            proj.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\docs\Example.xml");
        }
    }
}
