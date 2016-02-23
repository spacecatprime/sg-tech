using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.eventsys
{
    public abstract class GameEvent : Comparer<GameEvent>
    {
        ulong mId = ulong.MaxValue; /// unique id among all events from an event listener
        object mContext = null; /// an optional context to maintain with this event
        GameTime mTimeStamp = null; /// when the game event should fire
        util.PropertyBag mAdjustmentMap = null; /// a map of adjustments pivoted by labels [TODO: come up with adjustment filters?]

        public GameEvent(EventManager aManager, object aCtx)
        {
            mId = aManager.GetNextId();
            mTimeStamp = aManager.GetGameTime();
            mContext = aCtx;
        }

        public object GetContext()
        {
            return mContext;
        }

        /// <summary>
        /// applies an adjustment via named label with a value; 
        /// the GameEvent's Execute() will need to use the adjustments explicitly
        /// </summary>
        /// <param name="aAdjName"></param>
        /// <param name="aAdjValue"></param>
        public void ApplyAdjustment(string aAdjName, double aAdjValue)
        {
            if (mAdjustmentMap == null)
            {
                mAdjustmentMap = new util.PropertyBag();
            }
            mAdjustmentMap.Add(new util.PropertyLabel(aAdjName), aAdjValue);
        }

        /// <summary>
        /// is this game event still valid to Execute()?
        /// </summary>
        /// <returns></returns>
        public abstract bool IsValid();

        /// <summary>
        /// automatically completes an event, returning true for successful event and false otherwise
        /// </summary>
        public abstract bool Execute();

        /// <summary>
        /// needs to be implemented!
        /// </summary>
        /// <param name="nextTime"></param>
        /// <returns></returns>
        public abstract bool ShouldFire(GameTime nextTime);

        /// <summary>
        /// to sort against all other game events
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public override int Compare(GameEvent lhs, GameEvent rhs)
        {
            return GameTime.Compare(lhs.mTimeStamp,rhs.mTimeStamp);
        }
    }
}
