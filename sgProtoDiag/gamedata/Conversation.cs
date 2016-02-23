using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace sgProtoDiag.gamedata
{
    public class ConversationManager : util.Singleton<ConversationManager>, IObjectDepot
    {
        private ConversationManager()
        {
        }

        BaseType IObjectDepot.FindObject(string aType, string aKey, string aValue)
        {
            if (aType == "conversation")
            {
                try
                {
                    int key = int.Parse(aKey);

                    Conversation conv;
                    if (aKey.ToLower() == "id" && m_conversationMap.TryGetValue(key, out conv))
                    {
                        return conv;
                    }
                }
                catch (System.Exception) {}
            }
            return null;
        }

        public ConversationEngine Load(System.Xml.XmlDocument aDoc)
        {
            gamedata.ChatMapperData.Project p = new gamedata.ChatMapperData.Project();
            if (p.Load(aDoc))
            {
                ConversationEngine eng = new ConversationEngine(p);
                m_projectList.Add(new ConversationEngine(p));

                List<gamedata.ChatMapperData.ConversationData>.Enumerator it = p.ConversationList;
                while(it.MoveNext())
                {
                    gamedata.Conversation conv = new gamedata.Conversation();
                    if (conv.Load(p, it.Current.ID))
                    {
                        m_conversationMap.Add(it.Current.ID, conv);
                    }
                }
                return eng;
            }
            return null;
        }

        public Conversation GetConversationByID(int aID)
        {
            Conversation c;
            if (m_conversationMap.TryGetValue(aID, out c))
            {
                return c;
            }
            return null;
        }

        //
        // data
        //
        public class ConversationEngine
        {
            public gamedata.ChatMapperData.Project m_proj = null;
            public util.LuaState m_lua = new util.LuaState();

            public ConversationEngine(gamedata.ChatMapperData.Project aProject)
            {
                m_proj = aProject;
            }
        }
        private Dictionary<int, Conversation> m_conversationMap = new Dictionary<int, Conversation>();
        private List<ConversationEngine> m_projectList = new List<ConversationEngine>();
    }

    /// <summary>
    /// the main talker and any supporters or allies
    /// </summary>
    public class ConversationTalker
    {
    }

    /// <summary>
    /// the main target conversant plus any detractors of Talker
    /// </summary>
    public class ConversationTarget
    {
    }

    /// <summary>
    /// a set of binary files for any dialog or conversation
    /// </summary>
    public class ConversationFiles
    { 
    }

    /// <summary>
    /// an instance of a dialog entry, could be in any state
    /// </summary>
    [DebuggerDisplay("Name = {m_DialogEntry.Title}, ID = {m_DialogEntry.ID}")]
    public class ConversationDialog
    {
        public ConversationFiles Pictures { get; private set; }
        public ConversationFiles Audio { get; private set; }
        public ConversationFiles Animations { get; private set; }

        public int DialogEntryID { get { return m_DialogEntry.ID; } }
        public ChatMapperData.DialogEntry TheDialogEntry { get { return m_DialogEntry; } }

        // data
        private ChatMapperData.DialogEntry m_DialogEntry = null;

        public ConversationDialog(ChatMapperData.DialogEntry aDialogEntry)
        {
            m_DialogEntry = aDialogEntry;
        }

        public List<ConversationOption> GetOptions(ConversationContext aCtx)
        {
            List<ConversationOption> options = new List<ConversationOption>();
            foreach (var o in m_DialogEntry.OutgoingLinkList)
            {
                ConversationDialog diag = aCtx.TheConversation.GetDialogById(o.destinationDialogID);
                if (null != diag)
                {
                    options.Add(new ConversationOption(diag));
                }
            }
            return options;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", m_DialogEntry.Name, m_DialogEntry.ID);
        }
    }

    public class ConversationContext
    {
        public int Condition { get; set; }
        public bool IsComplete { get; set; }
        public List<ConversationTalker> Talkers { get; private set; }
        public List<ConversationTarget> Targets { get; private set; }
        public Conversation TheConversation { get; set; }

        public ConversationContext(Conversation aConversation)
        {
            TheConversation = aConversation;
            Talkers = new List<ConversationTalker>();
            Targets = new List<ConversationTarget>();
        }
    }

    public class ConversationOption
    {
        public int OriginDialogID { get; private set; }
        public int DestinationDialogID { get; private set; }
        public bool IsConnector { get; private set; }

        public bool IsPassThrough { get; private set; }
        public bool IsRoot { get; private set; }
        public string MenuText { get; private set; }
        public string DialogueText { get; private set; }
        public string UserScript { get; private set; }
        public string ConditionScript { get; private set; }

        public ConversationOption(ConversationDialog aDlg)
        {
            ChatMapperData.DialogEntry.OutgoingLink? outLink = 
                aDlg.TheDialogEntry.OutgoingLinkList.Find(delegate(ChatMapperData.DialogEntry.OutgoingLink link) 
                { 
                    return link.destinationDialogID == aDlg.DialogEntryID; 
                });

            OriginDialogID = aDlg.TheDialogEntry.ID;
            DestinationDialogID = aDlg.DialogEntryID;
            IsConnector = (outLink.HasValue) ? outLink.Value.isConnector : false;
            IsPassThrough = (aDlg.TheDialogEntry.FalseCondtionAction == "Passthrough");
            IsRoot = aDlg.TheDialogEntry.IsRoot;
            UserScript = aDlg.TheDialogEntry.UserScript;
            ConditionScript = aDlg.TheDialogEntry.ConditionsScipt;
            DialogueText = aDlg.TheDialogEntry.DialogueText;

            if (!string.IsNullOrEmpty(aDlg.TheDialogEntry.Title))
            {
                MenuText = aDlg.TheDialogEntry.Title;
            }
            else if (!string.IsNullOrEmpty(aDlg.TheDialogEntry.MenuText))
            {
                MenuText = aDlg.TheDialogEntry.MenuText;
            }
            else if (!string.IsNullOrEmpty(aDlg.TheDialogEntry.Name))
            {
                MenuText = aDlg.TheDialogEntry.Name;
            }

            //util.Monitoring.Logging.Debug("ConversationOption: {0}, {1}, {2}, {3}, {4}", OriginDialogID, DestinationDialogID, MenuText, DialogueText, aDlg.TheDialogEntry.Title);
            util.Monitoring.Logging.Debug("ConversationOption: {0}, {1}, {2}, {3}, {4}", OriginDialogID, DestinationDialogID, IsConnector, IsPassThrough, MenuText);
        }
    }

    public class Conversation : BaseType
    {
        /// properties
        public int ID { get { return m_ConversationData.ID; } }
        public List<ConversationDialog> DialogList { get { return new List<ConversationDialog>(m_dialogMap.Values); } }
        public List<gamedata.ChatMapperData.Actor> ActorList { get; private set; }
        public List<gamedata.ChatMapperData.UserVariable> UserVariableList { get; private set; }
        public List<gamedata.ChatMapperData.Location> LocationList { get; private set; }
        public List<gamedata.ChatMapperData.Item> ItemList { get; private set; }

        // data
        private ChatMapperData.ConversationData m_ConversationData = null;
        private Dictionary<int, ConversationDialog> m_dialogMap = new Dictionary<int, ConversationDialog>();

        public Conversation()
        {
            ActorList = new List<gamedata.ChatMapperData.Actor>();
            UserVariableList = new List<gamedata.ChatMapperData.UserVariable>();
            LocationList = new List<gamedata.ChatMapperData.Location>();
            ItemList = new List<gamedata.ChatMapperData.Item>();
        }

        internal bool Load(ChatMapperData.Project aProj, int aConvId)
        {
            List<ChatMapperData.ConversationData>.Enumerator itConv = aProj.ConversationList;
            while( itConv.MoveNext() )
            {
                if (itConv.Current.ID == aConvId)
                {
                    m_ConversationData = itConv.Current;

                    Name = m_ConversationData.Title;

                    foreach (var d in m_ConversationData.DialogList)
                    {
                        ConversationDialog diag = new ConversationDialog(d);
                        m_dialogMap.Add(d.ID, diag);
                    }
                    List<ChatMapperData.Actor>.Enumerator itActor = aProj.ActorList;
                    while( itActor.MoveNext() )
                    {
                        ActorList.Add(itActor.Current);
                    }
                    List<ChatMapperData.UserVariable>.Enumerator itVar = aProj.UserVariableList;
                    while (itVar.MoveNext())
                    {
                        UserVariableList.Add(itVar.Current);
                    }
                    List<ChatMapperData.Location>.Enumerator itLoc = aProj.LocationList;
                    while (itLoc.MoveNext())
                    {
                        LocationList.Add(itLoc.Current);
                    }
                    List<ChatMapperData.Item>.Enumerator itItem = aProj.ItemList;
                    while (itItem.MoveNext())
                    {
                        ItemList.Add(itItem.Current);
                    }
                    return m_dialogMap.Count > 0;
                }
            }
            return false;
        }

        public List<ConversationOption> GetOptions(ConversationDialog aDialog, ConversationContext aCtx)
        {
            if (aDialog != null)
            {
                return aDialog.GetOptions(aCtx);
            }
            return new List<ConversationOption>();
        }

        internal ConversationDialog GetDialogById(int aDiagId)
        {
            ConversationDialog diag = null;
            if (m_dialogMap.TryGetValue(aDiagId, out diag))
            {
                return diag;
            }
            return null;
        }

        public override string ToString()
        {
            return m_ConversationData.Title;
        }

        public static void DoTest()
        {
            ChatMapperData.Project proj = new ChatMapperData.Project();
            proj.Load(@"C:\Users\allen\Projects\spygame\proto\sgProtoDiag\sgProtoDiag\docs\Example.xml");
        }
    }
}
