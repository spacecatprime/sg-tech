using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.log
{
    public enum Level
    {
        Fatal,
        Severe,
        Warning,
        Missing,
        Info,
        Debug
    }

    class LoggerBase
    {
        public string Name { get; set; }
        public Level TheLevel { get; set; }

        protected struct LogRecord
        {
            public DateTime timeStamp;
            public string msg;
            public Level level;

            public LogRecord(Level aLevel, object aMsg)
            {
                level = aLevel;
                msg = aMsg.ToString();
                timeStamp = DateTime.Now;
            }
        }

        private Queue<LogRecord> m_LogMessages = new Queue<LogRecord>();

        public LoggerBase(string aName, Level aInitLevel)
        {
            Name = aName;
            TheLevel = aInitLevel;
        }

        public virtual void Process()
        {
            lock (m_LogMessages)
            { 
                foreach (var m in m_LogMessages)
                {
                    DoTrace(m);
                }
                m_LogMessages.Clear();
            }
        }

        public virtual void Trace(Level aLevel, object aMsg)
        {
            if (aLevel <= TheLevel && aMsg != null)
            { 
                lock(m_LogMessages)
                {
                    m_LogMessages.Enqueue(new LogRecord(aLevel, aMsg));
                }
            }
        }

        protected virtual void DoTrace(LogRecord aMsg)
        { 
            // does nothing
        }
    }
}
