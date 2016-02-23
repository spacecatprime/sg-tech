using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.eventsys
{
    public enum AdjustmentPhase
    {
        OnAdd,
        OnExecute,
    }

    public abstract class EventListener
    {
        /// <summary>
        /// Test to see if a certain game event
        /// </summary>
        /// <param name="aEvent"></param>
        /// <returns></returns>
        public abstract bool CouldAffect(GameEvent aEvent);

        /// <summary>
        /// describes if the listener is still alive at this GameTime
        /// </summary>
        /// <param name="aGameTime"></param>
        /// <returns>true if the listener is still active; false will likely destroy this listener!</returns>
        public abstract bool StillActive(GameTime aGameTime);

        /// <summary>
        /// apply all adjustments to the game event
        /// </summary>
        /// <param name="aEvent"></param>
        /// <returns></returns>
        public abstract bool ApplyAdjustments(GameEvent aEvent, AdjustmentPhase aPhase);
    }
}
