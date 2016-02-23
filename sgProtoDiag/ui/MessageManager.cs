using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.ui
{
    /// <summary>
    /// meant to be the sync place to pass info between the Simulation and User Interface logic; 
    /// in WinForm, it crashes the app to update any controls on non-main threads
    /// </summary>
    class MessageManager
    {
        eventsys.GameTime m_gameTime = new eventsys.GameTime(null);
        public eventsys.GameTime TheGameTime
        {
            get
            {
                lock (m_gameTime)
                {
                    return m_gameTime;
                }
            }

            set 
            {
                lock (m_gameTime)
                {
                    m_gameTime.Clone(value);
                }
            }
        }
    }
}
