using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.log
{
    /// <summary>
    /// meant to log to the IDE's Debug text output panel
    /// </summary>
    class DebugLogger : LoggerBase
    {
        public DebugLogger(string aName, Level aInitLevel)
            : base(aName, aInitLevel)
         {
         }

        public override void Process()
        {
            // do nothing
        }

        public override void Trace(Level aLevel, object aMsg)
        {
            base.Trace(aLevel, aMsg);
            base.Process();
        }

        protected override void DoTrace(LogRecord aMsg)
        {
            string m = string.Format("{0}, {1}, {2}, {3}", 
                Name, aMsg.level, aMsg.timeStamp.ToShortTimeString(), aMsg.msg);

            System.Diagnostics.Debug.WriteLine(m);
        }
    }
}
