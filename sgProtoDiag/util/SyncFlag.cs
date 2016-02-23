using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.util
{
    internal class SyncFlag
    {
        private bool m_bRunning = true;

        public SyncFlag(bool bVal)
        {
            m_bRunning = bVal;
        }

        public bool IsSet()
        {
            lock (this)
            {
                return m_bRunning;
            }
        }
        
        public void SetValue(bool isRunning)
        {
            lock (this)
            {
                m_bRunning = isRunning;
            }
        }
    }
}
