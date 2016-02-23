using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// event system - http://stackoverflow.com/questions/4089900/engine-scripting-and-class-structure

namespace sgProtoDiag.eventsys
{
    /// <summary>
    /// handles a game event stream
    /// </summary>
    public class EventManager
    {
#region STATIC
        private static EventManager s_EventManager = new EventManager();
        public static EventManager TheEventManager
        {
            get { return s_EventManager; }
        }
        //
        // autonomous events; usually handled in Lamba delegate
        //
        public delegate void HandleExecute(object aContext);
        public class AnonymousEvent : sgProtoDiag.eventsys.GameEvent
        {
            private event HandleExecute OnExecute;

            public AnonymousEvent(gamelogic.GameObject aSrc, gamelogic.GameObject aDst, HandleExecute aDelgate)
                : base(s_EventManager, new sgProtoDiag.gamelogic.GameContext(aSrc, aDst, null))
            {
                OnExecute += aDelgate;
            }

            public AnonymousEvent(gamelogic.GameObject aSrc, gamelogic.GameObject aDst, object aCtx, HandleExecute aDelgate)
                : base(s_EventManager, new sgProtoDiag.gamelogic.GameContext(aSrc, aDst, aCtx))
            {
                OnExecute += aDelgate;
            }

            public override bool IsValid()
            {
                return true;
            }
            public override bool Execute()
            {
                OnExecute(GetContext());
                return true;
            }

            public override bool ShouldFire(GameTime nextTime)
            {
                return true;
            }
        }

        public static GameEvent CreateEvent(gamelogic.GameObject aSrc, gamelogic.GameObject aTarget, HandleExecute aExecute)
        {
            return new AnonymousEvent(aSrc, aTarget, aExecute);
        }
        public static GameEvent CreateEvent(gamelogic.GameObject aSrc, gamelogic.GameObject aTarget, object aPayload, HandleExecute aExecute)
        {
            return new AnonymousEvent(aSrc, aTarget, aPayload,aExecute);
        }
#endregion

        private ulong m_nextId = 1; /// used to make unique game event ids
        private GameTime m_gameTime = new GameTime(null); /// active game times
        private List<EventListener> m_listeners = new List<EventListener>(); /// game event listeners
        private List<GameEvent> m_events = new List<GameEvent>(); /// active pending game events
        private log.EventLogger m_log = new log.EventLogger(); /// used to archive completed game events
        private System.Threading.Thread m_thread = null;
                                                                
        public GameTime TheGameTime
        {
            get { return m_gameTime; }
        }

        public void StartThread()
        {
            if (m_thread == null)
            {
                m_thread = new System.Threading.Thread(ProcThread);
                m_thread.Start();
            }
        }

        private void ProcThread()
        {
            TimeSpan hz = new TimeSpan(0,0,0,0,30);
            DateTime tick = DateTime.Now;
            while (Program.s_IsRunning.IsSet() &&
                   Program.s_IsSimulating.IsSet())
            {
                TimeSpan span = DateTime.Now - tick;
                tick = DateTime.Now;
                Process(span.TotalMilliseconds);
                System.Threading.Thread.Sleep(hz);
            }
        }

        public bool Process(double aGameTimeDelta)
        {
            m_gameTime.Advance(aGameTimeDelta);

            m_events.Sort();
            foreach (GameEvent evt in m_events)
            {
                if (evt.ShouldFire(m_gameTime))
                {
                    // apply adjustments right before executing it
                    m_listeners.ForEach(delegate(EventListener aEL)
                    {
                        aEL.ApplyAdjustments(evt, AdjustmentPhase.OnExecute);
                    });

                    bool ret = evt.Execute();
                    m_log.RecordEvent(evt, ret);
                }
            }
            
            Program.s_MM.TheGameTime.Clone(m_gameTime);
            return true;
        }

        public void SubscribeListener(EventListener aEventListener)
        {
            if (m_listeners.Contains(aEventListener) == false)
            {
                m_listeners.Add(aEventListener);
            }
        }

        public void UnsubscribeListener(EventListener aEventListener)
        {
            if (m_listeners.Contains(aEventListener) == true)
            {
                m_listeners.Remove(aEventListener);
            }
        }

        public void AddEvent(GameEvent aGameEvent)
        {
            // apply adjustments when added to the event listener
            m_listeners.ForEach(delegate(EventListener aEL)
            {
                aEL.ApplyAdjustments(aGameEvent, AdjustmentPhase.OnAdd);
            });

            m_events.Add(aGameEvent);
        }

        internal ulong GetNextId()
        {
            ++m_nextId;
            return m_nextId;
        }

        internal GameTime GetGameTime()
        {
            return m_gameTime;
        }
    }
}
