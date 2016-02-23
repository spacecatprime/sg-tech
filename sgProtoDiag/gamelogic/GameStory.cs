using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sgProtoDiag.util;

namespace sgProtoDiag.gamelogic
{
    /// <summary>
    /// contains a number of GameConversation entries and the 
    /// context including what has happened, metrics, and counters
    /// </summary>
    public class GameStory
    {
        private gamedata.ChatMapperData.Project m_project = new gamedata.ChatMapperData.Project();
        private List<GameConversation> m_conversationList = new List<GameConversation>();
        private LuaState m_luaState = new LuaState();

        private class RelationshipMap : Dictionary<KeyValuePair<ulong, ulong>, Dictionary<string, int>>
        {
        };
        private RelationshipMap m_relationshipMap = new RelationshipMap();

        public List<GameConversation> ConversationList { get { return m_conversationList; } }

        public enum Property
        {
            SimStatus,
            ChoosenOptionId,
            Status
        }

        internal bool Load(System.Xml.XmlDocument doc)
        {
            if (m_project.Load(doc))
            {
                Reset();
                return true;
            }
            return false;
        }

        public void Reset()
        {
            m_conversationList.Clear();
            m_relationshipMap.Clear();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Dialog = {{ }};\n");
            sb.AppendFormat("Variable = {{ }};\n");
            sb.AppendFormat("Actor = {{ }};\n");
            sb.AppendFormat("Location = {{ }};\n");
            sb.AppendFormat("Item = {{ }};\n");

            List<gamedata.ChatMapperData.ConversationData>.Enumerator convIt = m_project.ConversationList;
            while (convIt.MoveNext())
            {
                gamedata.Conversation convData = new gamedata.Conversation();
                if (convData.Load(m_project, convIt.Current.ID))
                {
                    m_conversationList.Add(new GameConversation(convData, m_luaState));
                }
            }

            List<gamedata.ChatMapperData.Actor>.Enumerator actorIt = m_project.ActorList;
            while( actorIt.MoveNext() )
            {
                gamedata.ChatMapperData.Actor act = actorIt.Current;
                string label = util.LuaState.CreateLabel(act.Name);
                sb.AppendFormat("Actor['{0}'] = {{ }};\n", label);
                sb.AppendFormat("Actor['{0}'].Type = 'Actor';", label);
                sb.AppendFormat("Actor['{0}'].Name = '{1}';", label, act.Name);
                sb.AppendFormat("Actor['{0}'].Status = 'Untouched';\n", label);
                sb.AppendFormat("Actor['{0}'].ID = {1};\n", label, act.ID);
                sb.AppendFormat("Actor['{0}'].Gender = '{1}';\n", label, act.Gender);
                sb.AppendFormat("Actor['{0}'].Age = {1};\n", label, act.Age);
                sb.AppendFormat("Actor['{0}'].IsPlayer = {1};\n", label, act.IsPlayer ? "true" : "false");
            }

            List<gamedata.ChatMapperData.UserVariable>.Enumerator uservariableIt = m_project.UserVariableList;
            while (uservariableIt.MoveNext())
            {
                gamedata.ChatMapperData.UserVariable var = uservariableIt.Current;
                if (var.Name.IndexOf(' ') >= 0)
                {
                    util.Monitoring.Logging.Warning("label:{0} has spaces", var.Name);
                }
                if (var.ValueType == "Text")
                {
                    sb.AppendFormat("Variable['{0}'] = '{1}';\n", var.Name, var.InitialValue);
                }
                else
                {
                    sb.AppendFormat("Variable['{0}'] = {1};\n", var.Name, var.InitialValue);
                }
            }

            List<gamedata.ChatMapperData.Location>.Enumerator locIt = m_project.LocationList;
            while (locIt.MoveNext())
            {
                gamedata.ChatMapperData.Location loc = locIt.Current;
                string label = util.LuaState.CreateLabel(loc.Name);
                sb.AppendFormat("Location['{0}'] = {{ }};\n", label);
                sb.AppendFormat("Location['{0}'].Type = 'Location';", label);
                sb.AppendFormat("Location['{0}'].Name = '{1}';", label, loc.Name);
                sb.AppendFormat("Location['{0}'].Learned = false;\n", label);
                sb.AppendFormat("Location['{0}'].Visited = false;\n", label);
                sb.AppendFormat("Location['{0}'].ID = {1};\n", label, loc.ID);
            }

            m_luaState.ExecuteString(sb.ToString(), "GameStory_ctor");
            m_luaState.RegisterMethod(this, "DoLog");
            m_luaState.RegisterMethod(this, "SetRelationship");
            m_luaState.RegisterMethod(this, "GetRelationship");
            m_luaState.RegisterMethod(this, "IncRelationship");
            m_luaState.RegisterMethod(this, "DecRelationship");
            m_luaState.RegisterMethod(this, "SetStatus");
            m_luaState.RegisterMethod(this, "GetStatus");

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
            //m_luaState.ExecuteString("Location['Karen'], Actor['Receptionist']) );");
#endif
        }

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

        #region Default Lua Methods For Scripters to Use

        public void DoLog(string aLog)
        {
            util.Monitoring.Logging.Debug(aLog);
        }

        public void SetRelationship(LuaInterface.LuaTable aActor1, LuaInterface.LuaTable aActor2, string aRelationshipType, int aNumberValue)
        {
            gamelogic.GameActor actKey = null;
            gamelogic.GameActor actTarget = null;
            if (m_luaState.GameAssetFromTable<gamelogic.GameActor>(aActor1, ref actKey) && 
                m_luaState.GameAssetFromTable<gamelogic.GameActor>(aActor2, ref actTarget))
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
            gamelogic.GameActor actKey = null;
            gamelogic.GameActor actTarget = null;
            if (m_luaState.GameAssetFromTable<gamelogic.GameActor>(aActor1, ref actKey) &&
                m_luaState.GameAssetFromTable<gamelogic.GameActor>(aActor2, ref actTarget))
            {
                KeyValuePair<ulong, ulong> index = new KeyValuePair<ulong, ulong>(actKey.ID, actTarget.ID);
                if (m_relationshipMap.ContainsKey(index) && m_relationshipMap[index].ContainsKey(aRelationshipType))
                {
                    return m_relationshipMap[index][aRelationshipType];
                }
            }
            return 0;
        }

        public int IncRelationship(LuaInterface.LuaTable aActor1, LuaInterface.LuaTable aActor2, string aRelationshipType, int aNumberValue)
        {
            int val = 0;
            gamelogic.GameActor actKey = null;
            gamelogic.GameActor actTarget = null;
            if (m_luaState.GameAssetFromTable<gamelogic.GameActor>(aActor1, ref actKey) &&
                m_luaState.GameAssetFromTable<gamelogic.GameActor>(aActor2, ref actTarget))
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
            gamelogic.GameObject objKey = m_luaState.GameObjectFromTable(aAsset1);
            gamelogic.GameObject objTarget = m_luaState.GameObjectFromTable(aAsset2);
            if (null == objKey && null == objTarget)
            {
                return;
            }

            // remove old state, if any
            string oldState = GetStatus(aAsset1, aAsset2);
            if (string.IsNullOrEmpty(oldState) == false && aState != oldState)
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
            gamelogic.GameObject actKey = m_luaState.GameObjectFromTable(aAsset1);
            gamelogic.GameObject actTarget = m_luaState.GameObjectFromTable(aAsset2);
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
   
    }
}
