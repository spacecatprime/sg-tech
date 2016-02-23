using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.eventsys
{
    /// <summary>
    /// encapsulates the game's elapsed time; this is seperated from the app's tick count 
    /// </summary>
    public class GameTime
    {
        private DateTime m_dateTime = DateTime.Now;

        public GameTime(GameTime aTimeStamp)
        {
            if (aTimeStamp == null)
            {
                m_dateTime = DateTime.Now;
                return;
            }
            Clone(aTimeStamp);
        }

        public void Advance(double aMillSec)
        {
            m_dateTime = m_dateTime.AddMilliseconds(aMillSec);
        }

        public void Clone(GameTime m_other)
        {
            m_dateTime = DateTime.FromBinary(m_other.m_dateTime.ToBinary());
        }

        public static int Compare(GameTime lhs, GameTime rhs)
        {
            return lhs.m_dateTime.CompareTo(rhs.m_dateTime);
        }

        public override string ToString()
        {
            return m_dateTime.ToLongTimeString();
        }
    }
}
