using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

// System.Speech.Synthesis.SpeechSynthesizer

namespace sgProtoDiag
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
#if DEBUG
            gamedata.TypeManager.DoTest();
            gamedata.ListManager.DoTest();
            sgProtoDiag.gamedata.ChatMapperData.DoTest();
            WindowsApplication.Program.StartConsole();
#endif
            util.Monitoring.Logging.GetSingleton().Register(new util.Monitoring.LogConsumers.FileLogConsumer("sgProtoDiag.log", false));
            util.Monitoring.Logging.GetSingleton().Register(new util.Monitoring.LogConsumers.DiagnosticsLogConsumer());
            util.Monitoring.Logging.GetSingleton().Register(new util.Monitoring.LogConsumers.ConsoleLogConsumer());
            util.Monitoring.Logging.Info("Starting...");

            s_theTimer.Interval = 20;
            s_theTimer.Enabled = true;

            eventsys.EventManager.TheEventManager.StartThread();

            s_IsRunning.SetValue(true);
            s_IsSimulating.SetValue(true);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            s_IsRunning.SetValue(false);
            s_IsSimulating.SetValue(false);
        }

        static System.Timers.Timer s_theTimer = new System.Timers.Timer(20);
        static public System.Timers.Timer TheTimer
        {
            get { return s_theTimer; }
        }

        static public ui.MessageManager s_MM = new ui.MessageManager();
        static public util.SyncFlag s_IsRunning = new util.SyncFlag(false);
        static public util.SyncFlag s_IsSimulating = new util.SyncFlag(false);
    }
}
