using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    /// <summary>
    /// basic container for game objects
    /// </summary>
    public class GameObjectManager
    {
        private List<GameObject> m_objectList = new List<GameObject>();

        public bool Add(GameObject aGameObject, GameContext aGameCtx)
        {
            if (aGameObject.TheState == GameObject.State.Active)
            {
                util.Monitoring.Logging.Warning("Game object already in another GameObjectManager for " + aGameObject.ID.ToString());
            }

            if (m_objectList.Contains(aGameObject))
            {
                return false;
            }
            aGameObject.TheState = GameObject.State.Active;
            m_objectList.Add(aGameObject);
            EmitBeenAdded(aGameObject, aGameCtx);
            return true;
        }

        public bool Remove(GameObject aGameObject, GameContext aGameCtx)
        {
            if (aGameObject.TheState != GameObject.State.Active)
            {
                util.Monitoring.Logging.Warning("Game object is flagged as removed " + aGameObject.ID.ToString());
            }

            if (m_objectList.Contains(aGameObject))
            {
                aGameObject.TheState = GameObject.State.Removed;
                m_objectList.Remove(aGameObject);
                EmitBeenRemoved(aGameObject, aGameCtx);
            }
            return false;
        }

        public void SignalGameEvent(GameContext aEventContext, GameContext aGameCtx)
        {
            foreach (GameObject gObj in m_objectList)
            {
                EmitGameEvent(gObj, aGameCtx);
            }
        }

#region Game Events
        public delegate bool HandleGameContext(GameObject aGameObj, GameContext aGameContext);

        public event HandleGameContext OnGameEvent;
        protected bool EmitGameEvent(GameObject aGameObj, GameContext aGameContext)
        {
            if (OnGameEvent != null)
            {
                return OnGameEvent(aGameObj, aGameContext);
            }
            return false;
        }

        public event HandleGameContext OnBeenRemoved;
        protected bool EmitBeenRemoved(GameObject aGameObj, GameContext aGameContext)
        {
            if (OnBeenRemoved != null)
            {
                return OnBeenRemoved(aGameObj, aGameContext);
            }
            return false;
        }

        public event HandleGameContext OnBeenAdded;
        protected bool EmitBeenAdded(GameObject aGameObj, GameContext aGameContext)
        {
            if (OnBeenAdded != null)
            {
                return OnBeenAdded(aGameObj, aGameContext);
            }
            return false;
        }
#endregion
    }
}
