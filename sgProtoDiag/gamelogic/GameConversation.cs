using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sgProtoDiag.util;

namespace sgProtoDiag.gamelogic
{
    public class GameConversation : GameObject
    {
        struct OptionStatus
        {
            public gamedata.ConversationOption Option { get; set; }
            public List<string> States { get; set; }
        }

        private gamedata.ConversationContext m_ctx = null;
        private gamedata.Conversation m_conv = null;
        private util.LuaState m_luaState = null;
        private gamedata.ConversationDialog m_activeDlg = null;

        public gamedata.Conversation Conversation
        {
            get { return m_conv; }
        }

        public gamedata.ConversationDialog ActiveDialog
        {
            get { return m_activeDlg; }
        }

        public GameConversation(gamedata.Conversation aConv, LuaState aLuaState)
        {
            m_luaState = aLuaState;
            m_conv = aConv;
            m_ctx = new gamedata.ConversationContext(aConv);
            Reset();
        }

        public bool Reset()
        {
            return SetActiveDialog(0);
        }

        public bool IsComplete()
        {
            return (null == m_activeDlg);
        }

        public bool SetActiveDialog(int aId)
        {
            if (aId == 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("Dialog = {{ }};\n");

                m_conv.DialogList.ForEach(delegate(gamedata.ConversationDialog dlg)
                {
                    string label = dlg.DialogEntryID.ToString();
                    sb.AppendFormat("Dialog[{0}] = {{ }};\n", label);
                    sb.AppendFormat("Dialog[{0}].ID = {0};\n", label, dlg.DialogEntryID);
                    sb.AppendFormat("Dialog[{0}].Type = 'Dialog';\n", label);
                    sb.AppendFormat("Dialog[{0}].SimStatus = 'Untouched';\n", label);
                    sb.AppendFormat("Dialog[{0}].ConversationID = '{1}';\n", label, m_conv.ID);
                });

                m_luaState.ExecuteString(sb.ToString(), "GameConversation_" + ID.ToString());

                object test = m_luaState.THE_LVM.GetTable("Location");
            }

            m_activeDlg = m_conv.GetDialogById(aId);
            if (null != m_activeDlg)
            {
                SetDialogProperty(aId, gamelogic.GameStory.Property.SimStatus, "WasDisplayed");
                return true;
            }
            return false;
        }

        public List<gamedata.ConversationOption> GetAvaiableOptionsForActive()
        {
            //m_luaState.EvaluateString("DoLog('test');");
            //m_luaState.ExecuteString("DoLog('HELLO LUA');");
            //m_luaState.ExecuteString("DoLog( toString(Dialog) );");

            LuaInterface.LuaTable table = m_luaState.THE_LVM.GetTable("Dialog");
            foreach (var entry in table)
            {
                util.Monitoring.Logging.Debug(entry["SimStatus"].ToString());
            }

            return GetAvaiableOptions(m_activeDlg.DialogEntryID);
        }

        protected List<gamedata.ConversationOption> GetAvaiableOptions(int aDestinationDialogID)
        {
            List<gamedata.ConversationOption> opts = new List<gamedata.ConversationOption>();
            gamedata.ConversationDialog dlg = m_conv.GetDialogById(aDestinationDialogID);
            if (null != dlg)
            {
                List<gamedata.ConversationOption> allOpts = dlg.GetOptions(m_ctx);
                foreach (var o in allOpts)
                {
                    if (o.IsConnector || OptionIsAvailable(o.DestinationDialogID))
                    {
                        opts.Add(o);
                    }
                }
            }
            return opts;
        }

        protected bool OptionIsAvailable(int aDestinationDialogID)
        {
            gamedata.ConversationDialog dlg = m_conv.GetDialogById(aDestinationDialogID);
            if (null != dlg)
            {
                if (string.IsNullOrEmpty(dlg.TheDialogEntry.ConditionsScipt))
                {
                    return true;
                }
                return m_luaState.EvaluateString(dlg.TheDialogEntry.ConditionsScipt);
            }
            return false;
        }

        /// <summary>
        /// moving the active dialog to the next dialog and setting the current active dialog as "WasDisplayed"
        /// </summary>
        /// <param name="aCtx"></param>
        /// <param name="aOption"></param>
        /// <returns></returns>
        public gamedata.ConversationDialog Advance(gamedata.ConversationOption aOption)
        {
            if (null == m_activeDlg)
            {
                return null;
            }
            else if (m_activeDlg.DialogEntryID == aOption.DestinationDialogID)
            {
                return m_activeDlg;
            }
            else if (ApplyOptionForDialog(m_activeDlg, aOption) == false)
            {
                return m_activeDlg;
            }

            List<gamedata.ConversationOption> options = GetAvaiableOptions(aOption.DestinationDialogID);
            if (options.Count == 1)
            {
                if (options[0].IsRoot)
                {
                    return Advance(options[0]);
                }
                else if (options[0].IsPassThrough && OptionIsAvailable(options[0].DestinationDialogID))
                {
                    return Advance(options[0]);
                }
            }
            SetActiveDialog(aOption.DestinationDialogID);
            return m_activeDlg;
        }

        public bool ApplyOptionForDialog(gamedata.ConversationDialog aDlg, gamedata.ConversationOption aOpt)
        {
            // trying to go to the same dialog?
            if (aOpt.OriginDialogID == aDlg.DialogEntryID)
            {
                return false;
            }

            SetDialogProperty(aDlg.DialogEntryID, gamelogic.GameStory.Property.ChoosenOptionId, aOpt.DestinationDialogID.ToString());

            gamedata.ConversationDialog optDialg = m_conv.GetDialogById(aOpt.DestinationDialogID);
            if (string.IsNullOrEmpty(optDialg.TheDialogEntry.UserScript) == false)
            {
                m_luaState.ExecuteString(optDialg.TheDialogEntry.UserScript);
            }

            return true;
        }

        private void SetDialogProperty(int aDlgId, gamelogic.GameStory.Property aProperty, string aValue)
        {
            m_luaState.ExecuteString(string.Format("Dialog[{0}].{1} = {2};", aDlgId, aProperty.ToString(), aValue));
        }
    }
}
