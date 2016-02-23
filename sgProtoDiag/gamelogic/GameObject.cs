using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    /// <summary>
    /// every object in the simulation will need to derive from this interface
    /// </summary>
    public class GameObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aId"></param>
        public GameObject()
        {
            ID = GameWorld.GetNextId();
        }

        public GameObject(ulong aID)
        {
            ID = aID;
        }

        public GameObject(LuaInterface.LuaTable aTable)
        {
            ID = ulong.Parse(aTable["ID"].ToString());
        }

        public ulong ID { get; private set; }

        /// <summary>
        /// basic object states
        /// </summary>
        public enum State { New, Active, Removed };
        private State m_theState = State.New;
        public State TheState
        {
            get
            {
                if (m_theState == State.New)
                {
                    m_theState = State.Active;
                    return State.New;
                }
                return m_theState;
            }
            set
            {
                m_theState = value;
            }
        }

        /// <summary>
        /// override to return a un-usable situation based on a GameContext (default is un-usable)
        /// </summary>
        public virtual bool IsUsable(GameContext aGameCtx)
        {
            return false;
        }

        /// <summary>
        /// game meta label/value list
        /// </summary>
        public util.PropertyBag PropertyList { get; set; }

        /// <summary>
        /// generic label/object metadata
        /// </summary>
        public Dictionary<util.PropertyLabel, object> AttributeList;

        /// <summary>
        /// returns a new SpyToken meant to describe an actor's token representation
        /// </summary>
        /// <returns></returns>
        public virtual SpyToken GetDesc()
        {
            return null;
        }
    }
}
