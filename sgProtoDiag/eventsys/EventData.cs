using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sgProtoDiag.gamelogic;

namespace sgProtoDiag.eventsys
{
    /// <summary>
    /// basic data structure for most GameEvent types
    /// </summary>
    class EventData
    {
        public util.WeakRef<GameObject> mSource = null;
        public util.WeakRef<GameObject> mTarget = null;
        public util.WeakRef<GameObject> mPayload = null;

        public EventData(GameObject aSrc, GameObject aTar)
        {
            mSource = new util.WeakRef<GameObject>(aSrc);
            mTarget = new util.WeakRef<GameObject>(aTar);
        }
    }
}
