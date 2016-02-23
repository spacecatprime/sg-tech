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

        class RelationshipMap : Dictionary<KeyValuePair<ulong, ulong>, Dictionary<string, int>>
        { 
        };

        private gamedata.ConversationContext m_ctx = null;
        private gamedata.Conversation m_conv = null;
        private util.LuaState m_luaState = null;
        private gamedata.ConversationDialog m_activeDlg = null;
        private RelationshipMap m_relationshipMap = new RelationshipMap();

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
            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("Dialog = {{ }};\n");
            //sb.AppendFormat("Variable = {{ }};\n");
            //sb.AppendFormat("Actor = {{ }};\n");
            //sb.AppendFormat("Location = {{ }};\n");
            //sb.AppendFormat("Item = {{ }};\n");

            //m_conv.DialogList.ForEach(delegate(gamedata.ConversationDialog dlg) 
            //{
            //    sb.AppendFormat("Dialog[{0}] = {{ }};\n", dlg.DialogEntryID);
            //    sb.AppendFormat("Dialog[{0}].ID = {{ }};\n", dlg.DialogEntryID);
            //    sb.AppendFormat("Dialog[{0}].Type = 'Dialog';", dlg.DialogEntryID);
            //    sb.AppendFormat("Dialog[{0}].SimStatus = 'Untouched';\n", dlg.DialogEntryID);
            //});

            //m_conv.ActorList.ForEach(delegate(gamedata.ChatMapperData.Actor act)
            //{
            //    string label = util.LuaState.CreateLabel(act.Name);
            //    sb.AppendFormat("Actor['{0}'] = {{ }};\n", label);
            //    sb.AppendFormat("Actor['{0}'].Type = 'Actor';", label);
            //    sb.AppendFormat("Actor['{0}'].Name = '{1}';", label, act.Name);
            //    sb.AppendFormat("Actor['{0}'].Status = 'Untouched';\n", label);
            //    sb.AppendFormat("Actor['{0}'].ID = {1};\n", label, act.ID);
            //    sb.AppendFormat("Actor['{0}'].Gender = '{1}';\n", label, act.Gender);
            //    sb.AppendFormat("Actor['{0}'].Age = {1};\n", label, act.Age);
            //    sb.AppendFormat("Actor['{0}'].IsPlayer = {1};\n", label, act.IsPlayer ? "true" : "false");
            //});

            //m_conv.UserVariableList.ForEach(delegate(gamedata.ChatMapperData.UserVariable var)
            //{
            //    if (var.Name.IndexOf(' ') >= 0)
            //    {
            //        util.Monitoring.Logging.Warning("label:{0} has spaces", var.Name);
            //    }
            //    if (var.ValueType == "Text")
            //    {
            //        sb.AppendFormat("Variable['{0}'] = '{1}';\n", var.Name, var.InitialValue);
            //    }
            //    else
            //    {
            //        sb.AppendFormat("Variable['{0}'] = {1};\n", var.Name, var.InitialValue);
            //    }
            //});

            //m_conv.LocationList.ForEach(delegate(gamedata.ChatMapperData.Location loc)
            //{
            //    string label = util.LuaState.CreateLabel(loc.Name);
            //    sb.AppendFormat("Location['{0}'] = {{ }};\n", label);
            //    sb.AppendFormat("Location['{0}'].Type = 'Location';", label);
            //    sb.AppendFormat("Location['{0}'].Name = '{1}';", label, loc.Name);
            //    sb.AppendFormat("Location['{0}'].Learned = false;\n", label);
            //    sb.AppendFormat("Location['{0}'].Visited = false;\n", label);
            //    sb.AppendFormat("Location['{0}'].ID = {1};\n", label, loc.ID);
            //});

            //m_conv.ItemList.ForEach(delegate(gamedata.ChatMapperData.Item item)
            //{
            //    string label = util.LuaState.CreateLabel(item.Name);
            //    sb.AppendFormat("Item['{0}'] = {{ }};\n", label);
            //    sb.AppendFormat("Item['{0}'].Type = 'Item';", label);
            //    sb.AppendFormat("Item['{0}'].Name = '{0}';", label, item.Name);
            //    sb.AppendFormat("Item['{0}'].InInventory = {1};\n", label, item.InInventory ? "true" : "false");
            //    sb.AppendFormat("Item['{0}'].ID = {1};\n", label, item.ID);
            //    sb.AppendFormat("Item['{0}'].Scene = '{1}';\n", label, item.Scene);
            //});

            //m_luaState = new util.LuaState();
            //m_luaState.ExecuteString(sb.ToString(), "GameConversation_ctor");
            //m_luaState.RegisterMethod(this, "DoLog");
            //m_luaState.RegisterMethod(this, "SetRelationship");
            //m_luaState.RegisterMethod(this, "GetRelationship");
            //m_luaState.RegisterMethod(this, "IncRelationship");
            //m_luaState.RegisterMethod(this, "DecRelationship");
            //m_luaState.RegisterMethod(this, "SetStatus");
            //m_luaState.RegisterMethod(this, "GetStatus");

#if false
            m_luaState.ExecuteString("DoLog('HELLO LUA');");
            m_luaState.ExecuteString("SetRelationship(Actor['Karen'], Actor['Tommy'], 'is_upset', 11);");
            m_luaState.ExecuteString("SetRelationship(Actor['Karen'], Actor['Tommy'], 'is_upset', 12);");
            m_luaState.ExecuteString("DoLog(string.format('%u',GetRelationship(Actor['Karen'], Actor['Tommy'], 'is_upset')));");
            m_luaState.ExecuteString("Variable['PasswordAttempts'] = Variable['PasswordAttempts'] + 1;");
            m_luaState.ExecuteString("DoLog( string.format('PasswordAttempts = %u',Variable['PasswordAttempts']) );");
            m_luaState.ExecuteString("SetStatus( Actor['Karen'], Actor['Receptionist'], 'spoke to' );");
            m_luaState.ExecuteString("SetStatus( Actor['Karen'], Actor['Receptionist'], 'likes' );");
            m_luaState.ExecuteString("DoLog( GetStatus(Actor['Karen'], Actor['Receptionist']) );");
#endif
            return SetActiveDialog(0);
        }

        public bool IsComplete()
        {
            return (null == m_activeDlg);
        }

        public bool SetActiveDialog(int aId)
        {
            m_activeDlg = m_conv.GetDialogById(aId);
            if( null != m_activeDlg )
            {
                SetDialogProperty(aId, Property.SimStatus, "WasDisplayed");
                return true;
            }
            return false;
        }

        public List<gamedata.ConversationOption> GetAvaiableOptionsForActive()
        {
            return GetAvaiableOptions(m_activeDlg.DialogEntryID);
        }

        protected List<gamedata.ConversationOption> GetAvaiableOptions(int aDestinationDialogID)
        {
            gamedata.ConversationDialog dlg = m_conv.GetDialogById(aDestinationDialogID);
            if (null != dlg)
            {
                return new List<gamedata.ConversationOption>
                (
                    dlg.GetOptions(m_ctx).Where(opt =>
                        OptionIsAvailable(aDestinationDialogID))
                );
            }
            return new List<gamedata.ConversationOption>();
        }

        public bool OptionIsAvailable(int aDestinationDialogID)
        {
            gamedata.ConversationDialog dlg = m_conv.GetDialogById(aDestinationDialogID);
            if( null != dlg)
            {
                if(string.IsNullOrEmpty(dlg.TheDialogEntry.ConditionsScipt))
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
            if( null == m_activeDlg)
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

            SetDialogProperty(aDlg.DialogEntryID, Property.ChoosenOptionId, aOpt.DestinationDialogID.ToString());

            gamedata.ConversationDialog optDialg = m_conv.GetDialogById(aOpt.DestinationDialogID);
            if (string.IsNullOrEmpty(optDialg.TheDialogEntry.UserScript) == false)
            {
                m_luaState.ExecuteString(optDialg.TheDialogEntry.UserScript);
            }

            return true;
        }

        private enum Property
        { 
            SimStatus,
            ChoosenOptionId,
            Status
        }

        private void SetDialogProperty(int aDlgId, Property aProperty, string aValue)
        {
            m_luaState.ExecuteString(string.Format("Dialog[{0}].{1} = {2};", aDlgId, aProperty.ToString(), aValue));
        }

#region Default Lua Methods For Scripters to Use
        public void DoLog(string aLog)
        {
            util.Monitoring.Logging.Debug(aLog);
        }

        public void SetRelationship(LuaInterface.LuaTable aActor1, LuaInterface.LuaTable aActor2, string aRelationshipType, int aNumberValue)
        {
            gamelogic.GameActor actKey = GameActorFromTable(aActor1);
            gamelogic.GameActor actTarget = GameActorFromTable(aActor2);
            if (null != actKey && null != actTarget)
            {
                KeyValuePair<ulong, ulong> index = new KeyValuePair<ulong, ulong>(actKey.ID, actTarget.ID);
                if (m_relationshipMap.ContainsKey(index))
                {
                    Util.SafeDictionaryAdd<string, int>(m_relationshipMap[index], aRelationshipType, aNumberValue);
                }
                else 
                {
                    Dictionary<string, int> relValDict = new Dictionary<string, int>();
                    relValDict.Add(aRelationshipType, aNumberValue);
                    m_relationshipMap.Add(index, relValDict);
                }
            }
        }
        public int GetRelationship(LuaInterface.LuaTable aActor1, LuaInterface.LuaTable aActor2, string aRelationshipType)
        {
            gamelogic.GameActor actKey = GameActorFromTable(aActor1);
            gamelogic.GameActor actTarget = GameActorFromTable(aActor2);
            if (null != actKey && null != actTarget)
            {
                KeyValuePair<ulong, ulong> index = new KeyValuePair<ulong, ulong>(actKey.ID, actTarget.ID);
                if (m_relationshipMap.ContainsKey(index) && m_relationshipMap[index].ContainsKey(aRelationshipType) )
                {
                    return m_relationshipMap[index][aRelationshipType];
                }
            }
            return 0;
        }

        public int IncRelationship(LuaInterface.LuaTable aActor1, LuaInterface.LuaTable aActor2, string aRelationshipType, int aNumberValue)
        {
            int val = 0;
            gamelogic.GameActor actKey = GameActorFromTable(aActor1);
            gamelogic.GameActor actTarget = GameActorFromTable(aActor2);
            if (null != actKey && null != actTarget)
            {
                KeyValuePair<ulong, ulong> index = new KeyValuePair<ulong, ulong>(actKey.ID, actTarget.ID);
                if (m_relationshipMap.ContainsKey(index) && m_relationshipMap[index].ContainsKey(aRelationshipType))
                {
                    val = m_relationshipMap[index][aRelationshipType] + aNumberValue;
                    m_relationshipMap[index][aRelationshipType] = val;
                }
            }
            return val;
        }

        public int DecRelationship(LuaInterface.LuaTable aActor1, LuaInterface.LuaTable aActor2, string aRelationshipType, int aNumberValue)
        {
            return IncRelationship(aActor1, aActor2, aRelationshipType, -aNumberValue);
        }

        public void SetStatus(LuaInterface.LuaTable aAsset1, LuaInterface.LuaTable aAsset2, string aState)
        {
            //SetStatus(Actor["Karen"], Actor["Receptionist"], "spoke to")
            gamelogic.GameObject objKey = GameAssetFromTable(aAsset1);
            gamelogic.GameObject objTarget = GameAssetFromTable(aAsset2);
            if (null == objKey && null == objTarget)
            {
                return;
            }

            // remove old state, if any
            string oldState = GetStatus(aAsset1, aAsset2);
            if( string.IsNullOrEmpty(oldState) == false && aState != oldState )
            {
                KeyValuePair<ulong, ulong> index = new KeyValuePair<ulong, ulong>(objKey.ID, objTarget.ID);
                if (m_relationshipMap.ContainsKey(index))
                {
                    m_relationshipMap[index].Remove(Property.Status.ToString() + "|" + oldState);
                }
            }
            ApplyValueToMap(objKey, objTarget, Property.Status.ToString() + "|" + aState, 1);
        }

        public string GetStatus(LuaInterface.LuaTable aAsset1, LuaInterface.LuaTable aAsset2)
        {
            //GetStatus(Asset1, Asset2)
            gamelogic.GameObject actKey = GameAssetFromTable(aAsset1);
            gamelogic.GameObject actTarget = GameAssetFromTable(aAsset2);
            if (null != actKey && null != actTarget)
            {
                string prefix = Property.Status.ToString() + "|";
                KeyValuePair<ulong, ulong> index = new KeyValuePair<ulong, ulong>(actKey.ID, actTarget.ID);
                if (m_relationshipMap.ContainsKey(index))
                {
                    Dictionary<string, int> data = m_relationshipMap[index];
                    foreach (KeyValuePair<string, int> kvp in data)
                    {
                        if (kvp.Key.StartsWith(prefix))
                        {
                            return kvp.Key.Substring(prefix.Length);
                        }
                    }
                }
            }
            return "";
        }

        public void TrackVariable(LuaInterface.LuaTable aTable, string aVariableName)
        {
            util.Monitoring.Logging.Debug("TrackVariable -> {0} {1}", aTable.ToString(), aVariableName);
        }

        public void TrackStatus(LuaInterface.LuaTable aAsset1, LuaInterface.LuaTable aAsset2)
        {
            util.Monitoring.Logging.Debug("TrackStatus -> {0} {1}", aAsset1.ToString(), aAsset2.ToString());
        }

        public void TrackRelationship(LuaInterface.LuaTable aActor1, LuaInterface.LuaTable aActor2, string aRelationshipType)
        {
            util.Monitoring.Logging.Debug("TrackRelationship -> {0} {1} {2}", aActor1.ToString(), aActor2.ToString(), aRelationshipType);
        }
#endregion 

        private void ApplyValueToMap(GameObject idKey, GameObject idCtrl, string aStringKey, int aNumber)
        {
            if (null != idKey && null != idCtrl)
            {
                KeyValuePair<ulong, ulong> index = new KeyValuePair<ulong, ulong>(idKey.ID, idCtrl.ID);
                if (m_relationshipMap.ContainsKey(index))
                {
                    Util.SafeDictionaryAdd<string, int>(m_relationshipMap[index], aStringKey, aNumber);
                }
                else
                {
                    Dictionary<string, int> relValDict = new Dictionary<string, int>();
                    relValDict.Add(aStringKey, aNumber);
                    m_relationshipMap.Add(index, relValDict);
                }
            }
        }

        private gamelogic.GameActor GameActorFromTable(LuaInterface.LuaTable aActor)
        {
            try
            {
                if (aActor["Type"].ToString() != "Actor")
                {
                    return null;
                }
                string name = aActor["Name"].ToString();
                ulong id = ulong.Parse(aActor["ID"].ToString());
                gamelogic.GameActor act = new gamelogic.GameActor(name,id);
                return act;
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
            }
            return null;
        }

        private gamelogic.GameObject GameAssetFromTable(LuaInterface.LuaTable aTable)
        {
            if (null == aTable)
            {
                return null;
            }
            try
            {
                string type = aTable["Type"].ToString();
                switch (type)
                {
                    case "Actor":
                    {
                        string name = aTable["Name"].ToString();
                        ulong id = ulong.Parse(aTable["ID"].ToString());
                        return new gamelogic.GameActor(name, id);
                    }
                    case "Location":
                    {
                        string name = aTable["Name"].ToString();
                        ulong id = ulong.Parse(aTable["ID"].ToString());
                        return new gamelogic.GameLocation(name, id);
                    }
                    case "Item":
                    {
                        string name = aTable["Name"].ToString();
                        ulong id = ulong.Parse(aTable["ID"].ToString());
                        return new gamelogic.GameItem(name, id);
                    }
                }
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
            }
            return null;
        }
    }
}
