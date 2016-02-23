using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.gamelogic
{
    namespace GameWorldEvents
    {
        public class WorldEventBase : eventsys.GameEvent
        {
            protected util.WeakRef<GameObject> m_src = null;
            protected util.WeakRef<GameObject> m_target = null;
            protected util.WeakRef<GameLocation> m_location = null;

            public WorldEventBase()
                : base(eventsys.EventManager.TheEventManager, null)
            { 
            }

            public override bool IsValid()
            {
                throw new NotImplementedException();
            }

            public override bool Execute()
            {
                throw new NotImplementedException();
            }

            public override bool ShouldFire(sgProtoDiag.eventsys.GameTime nextTime)
            {
                throw new NotImplementedException();
            }
        }

        public class LocationChange : WorldEventBase
        {
            public LocationChange(GameObject aGameObj, GameLocation aLoc) 
            {
                m_target = new util.WeakRef<GameObject>(aGameObj);
                m_location = new util.WeakRef<GameLocation>(aLoc);
            }
            public override bool IsValid()
            {
                return m_src.IsAlive && m_location.IsAlive;
            }
            public override bool Execute()
            {
                return m_location.Target.Insert(m_target.Target);
            }
        }
    }

    /// <summary>
    /// contains the locations, objects, and actors
    /// </summary>
    class GameWorld
    {
#region STATIC
        public const ulong InvalidId = ulong.MaxValue;
        static private ulong m_nextId = 1;
        static public ulong GetNextId()
        {
            m_nextId++;
            return m_nextId;
        }
        static public GameWorld TheGameWorld { get; private set; }
        static GameWorld()
        {
            TheGameWorld = new GameWorld();
        }        
#endregion

        private GameObjectManager m_objMgr = new GameObjectManager();

        public bool MoveObjectToLocation(GameObject aGameObj, GameLocation aLoc)
        {
            eventsys.EventManager.TheEventManager.AddEvent(
                new GameWorldEvents.LocationChange(aGameObj, aLoc));

            return true;
        }
    }
}
