using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.util
{
    public class LuaState
    {
        private LuaInterface.Lua m_LVM = null;
        private int m_autoChunkNum = 1;
        private Dictionary<string, LuaInterface.LuaFunction> m_luaMap = new Dictionary<string, LuaInterface.LuaFunction>();

        public LuaState()
        {
            m_LVM = new LuaInterface.Lua();
            m_LVM.HookException += new EventHandler<LuaInterface.HookExceptionEventArgs>(OnLuaException);
        }

        void OnLuaException(object sender, LuaInterface.HookExceptionEventArgs e)
        {
            util.Monitoring.Logging.Exception(new System.Exception(e.ToString()));
        }

        /// <summary>
        /// used to pinvoke a VM to work with
        /// </summary>
        /// <param name="aLuaState"></param>
        public LuaState(long aLuaState)
        {
            m_LVM = new LuaInterface.Lua(aLuaState);
        }

        public LuaInterface.Lua THE_LVM
        {
            get { return m_LVM; }
        }

        public object[] ExecuteString(string aLuaScript)
        {
            return ExecuteString(aLuaScript, "chunk_" + m_autoChunkNum++);
        }

        public object[] ExecuteString(string aLuaScript, string aScriptName)
        {
            util.Monitoring.Logging.Debug(aLuaScript);
            try
            {
                return m_LVM.DoString(aLuaScript, aScriptName);
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
                return new object[] { };
            }
        }

        public bool EvaluateString(string aLuaSnippet)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("return true == ({0});", aLuaSnippet);

            object[] result = ExecuteString(sb.ToString());
            if (result.Length == 1 && result[0] is bool)
            {
                return (bool)result[0];
            }
            return false;
        }

        internal bool RegisterMethod(object aClass, string aMethod)
        {
            System.Reflection.MethodInfo method = aClass.GetType().GetMethod(aMethod);
            if (null != method)
            {
                m_luaMap.Add(aMethod, m_LVM.RegisterFunction(aMethod, aClass, method));
                return true;
            }
            return false;
        }

        public void PrintTable(LuaInterface.LuaTable aTable)
        {
            foreach (object key in aTable.Keys)
            {
                util.Monitoring.Logging.Debug("key {0} = value {1}", key, aTable[key]);
            }
        }

        public bool GameAssetFromTable<TGameType>(LuaInterface.LuaTable aTable, ref TGameType aGameObject)
        {
            if (null == aTable)
            {
                return false;
            }
            try
            {
                string name = aTable["Name"].ToString();
                ulong id = ulong.Parse(aTable["ID"].ToString());
                object obj = System.Activator.CreateInstance(typeof(TGameType), new object[] { name, id });
                aGameObject = (TGameType)obj;
                return true;
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
            }
            return false;
        }

        public gamelogic.GameObject GameObjectFromTable(LuaInterface.LuaTable aTable)
        {
            if (null == aTable)
            {
                return null;
            }
            try
            {
                string type = aTable["Type"].ToString();
                if (type == "Actor")
                {
                    return new gamelogic.GameActor(aTable);
                }
                else if (type == "Item")
                {
                    return new gamelogic.GameItem(aTable);
                }
                else if (type == "Location")
                {
                    return new gamelogic.GameLocation(aTable);
                }
                else
                {
                    return new gamelogic.GameObject(aTable);
                }
            }
            catch (System.Exception ex)
            {
                util.Monitoring.Logging.Exception(ex);
            }
            return null;
        }

#region Static
        public static string CreateLabel(string aLabel)
        {
            return aLabel.Replace(' ', '_');
        }
#endregion
    }
}
