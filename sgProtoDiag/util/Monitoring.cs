using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sgProtoDiag.util.Monitoring
{
    namespace LogConsumers
    {
        public class FileLogConsumer : LogConsumer
        {
            string mFilename;

            public FileLogConsumer(string aFilename, bool bAppend)
            {
                mFilename = aFilename;
                if (false == bAppend)
                {
                    try
                    {
                        System.IO.File.WriteAllText(mFilename, "");
                    }
                    catch (System.Exception) { }
                }
            }

            void LogConsumer.Consume(LogRecord aLogRecord)
            {
                try
                {
                    System.IO.File.AppendAllText(mFilename, FormatMsg(aLogRecord));
                }
                catch (System.Exception) { }
            }

			void LogConsumer.Shutdown()
			{
				WriteLine("FileLogConsumer.Shutdown");
			}

            protected virtual string FormatMsg(LogRecord aLogRecord)
            {
                return string.Format("[{0}] ({1}) {2}\r\n", DateTime.Now.ToString("d/MMM/yyyy:hh:mm:ss zzzz"), aLogRecord.mLevel, aLogRecord.mMsg);
            }

            public void WriteLine(string aLine)
            {
                try
                {
                    System.IO.File.AppendAllText(mFilename, aLine + "\n");
                }
                catch (System.Exception) { }
            }

            public void Write(string aText)
            {
                try
                {
                    System.IO.File.AppendAllText(mFilename, aText);
                }
                catch (System.Exception) { }
            }
        }

        public class ConsoleLogConsumer : LogConsumer
        {
            public Dictionary<Logger.LogLevel, ConsoleColor> m_colorCodes = 
                new Dictionary<Logger.LogLevel, ConsoleColor>();

            public ConsoleLogConsumer()
            {
                m_colorCodes.Add(Logger.LogLevel.Error, ConsoleColor.Red);
                m_colorCodes.Add(Logger.LogLevel.Highlight, ConsoleColor.Cyan);
                m_colorCodes.Add(Logger.LogLevel.Exception, ConsoleColor.DarkRed);
            }

            void LogConsumer.Consume(LogRecord aLogRecord)
            {
                string msg = string.Format("[{0}] {1}\r\n", aLogRecord.mLevel, aLogRecord.mMsg);

                ConsoleColor color;
                if (m_colorCodes.TryGetValue(aLogRecord.mLevel, out color))
                {
                    ConsoleColor old = Console.ForegroundColor;
                    Console.ForegroundColor = color;
                    Console.Write(msg);
                    Console.ForegroundColor = old;
                }
                else
                {
                    Console.Write(msg);
                }
            }
			void LogConsumer.Shutdown()
			{
				ConsoleColor old = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("ConsoleLogConsumer.Shutdown");
				Console.ForegroundColor = old;
			}
        }

        public class DiagnosticsLogConsumer : LogConsumer
        {
            void LogConsumer.Consume(LogRecord aLogRecord)
            {
                System.Diagnostics.Debug.Write(string.Format("[{0}] {1}\r\n", aLogRecord.mLevel, aLogRecord.mMsg));
            }
			void LogConsumer.Shutdown()
			{
				System.Diagnostics.Debug.Write("DiagnosticsLogConsumer.Shutdown()\r\n");
			}
		}

        public class DepotLogConsumer : LogConsumer
        {
            LogRecordList m_logList = new LogRecordList();

            void LogConsumer.Consume(LogRecord aLogRecord)
            {
                m_logList.Add(aLogRecord);
            }

			void LogConsumer.Shutdown()
			{
				LogRecord rec = new LogRecord();
				rec.mMsg = "DepotLogConsumer.Shutdown()";
				m_logList.Add(rec);
			}

            public LogRecordList TakeSnapshot()
            {
                return m_logList.Clone(true);
            }
        }
    }

    public struct LogRecord
    {
        public Logger.LogLevel mLevel;
        public string mMsg;
        public DateTime mTimeStamp;
    }

    /// <summary>
    /// basic consumer interface, meant to be registered to a logger
    /// </summary>
    public interface LogConsumer
    {
        void Consume(LogRecord aLogRecord);
		void Shutdown();
    }

    /// <summary>
    /// a thread safe list of LogRecord entries
    /// </summary>
    public class LogRecordList
    {
        public LogRecordList() { }

        public LogRecordList(List<LogRecord> aRecordList)
        {
            AddRange(aRecordList);
        }

        public void AddRange(IEnumerable<LogRecord> aRecordList)
        {
            lock (m_records)
            {
                m_records.AddRange(aRecordList);
            }
        }

        public void Add(LogRecord aLogRecord)
        {
            lock (m_records)
            {
                m_records.Add(aLogRecord);
            }
        }

        public void Clear()
        {
            lock (m_records)
            {
                m_records.Clear();
            }
        }

        public LogRecordList Clone(bool bClear)
        {
            lock (m_records)
            {
                LogRecordList list = new LogRecordList();
                list.AddRange(m_records);
                if (bClear)
                {
                    m_records.Clear();
                }
                return list;
            }
        }

        /// <summary>
        /// not thread safe! Please Clone() first then enumerate
        /// </summary>
        /// <returns></returns>
        public List<LogRecord>.Enumerator GetEnumerator()
        {
            return m_records.GetEnumerator();
        }

        private List<LogRecord> m_records = new List<LogRecord>();
    }

    public class Logger
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Highlight,
            Error,
            Exception,
            Always
        }

        private LogLevel m_logLevel = LogLevel.Debug;
        public LogLevel TheLogLevel
        {
            get { return m_logLevel; }
            set { m_logLevel = value; }
        }

        public Logger()
        { 
#if DEBUG
            m_logLevel = LogLevel.Debug;
#else
            m_logLevel = LogLevel.Warning;
#endif
        }

        public bool Register(LogConsumer aLogConsumer)
        {
            lock (m_logConsumers)
            {
                m_logConsumers.Add(aLogConsumer);
                return true;
            }
        }

        public bool UnRegister(LogConsumer aLogConsumer)
        {
            lock (m_logConsumers)
            {
				if( m_logConsumers.Contains(aLogConsumer) )
				{
					aLogConsumer.Shutdown();
					return m_logConsumers.Remove(aLogConsumer);
				}
				return false;
            }
        }

        public void UnRegisterAll()
        {
            lock (m_logConsumers)
            {
				m_logConsumers.ForEach(delegate(LogConsumer lc) { lc.Shutdown(); });
                m_logConsumers.Clear();
            }
        }

        public void Trace(Logger.LogLevel aLevel, string aMsg)
        {
            lock (m_logConsumers)
            {
                if (aLevel >= m_logLevel)
                {
                    LogRecord rec = new LogRecord();
                    rec.mLevel = aLevel;
                    rec.mMsg = aMsg;
                    rec.mTimeStamp = DateTime.Now;

                    foreach (LogConsumer lc in m_logConsumers)
                    {
                        lc.Consume(rec);
                    }
                }
            }
        }

        /// <summary>
        /// used to target logging to a certain set of LogConsumers
        /// </summary>
        /// <param name="aLevel"></param>
        /// <param name="aMsg"></param>
        /// <param name="aConsumerType"></param>
        public void Trace(Logger.LogLevel aLevel, string aMsg, Type aConsumerType)
        {
            lock (m_logConsumers)
            {
                LogRecord rec = new LogRecord();
                rec.mLevel = aLevel;
                rec.mMsg = aMsg;
                rec.mTimeStamp = DateTime.Now;

                foreach (LogConsumer lc in m_logConsumers)
                {
                    if (lc.GetType().Equals(aConsumerType))
                    {
                        lc.Consume(rec);
                    }
                }
            }
        }

        private List<LogConsumer> m_logConsumers = new List<LogConsumer>();
    }

    static public class Logging
    {
        static private Logger m_staticLogger = new Logger();

        static Logging()
        {
            // sets up static stuff
            // TODO: read config file and set up log consumers?
        }

		static public void Debug(string aMsg)
		{
			m_staticLogger.Trace(Logger.LogLevel.Debug, aMsg);
		}

		static public void Info(string aMsg)
		{
			m_staticLogger.Trace(Logger.LogLevel.Info, aMsg);
		}

        static public void Highlight(string aMsg)
        {
            m_staticLogger.Trace(Logger.LogLevel.Highlight, aMsg);
        }

		static public void Warning(string aMsg)
		{
			m_staticLogger.Trace(Logger.LogLevel.Warning, aMsg);
		}

		static public void Error(string aMsg)
		{
			m_staticLogger.Trace(Logger.LogLevel.Error, aMsg);
		}

        static public void Always(string aMsg)
		{
			m_staticLogger.Trace(Logger.LogLevel.Always, aMsg);
		}

        static public void Debug(string aMsg, params object[] aParams)
        {
            m_staticLogger.Trace(Logger.LogLevel.Debug, string.Format(aMsg, aParams));
        }

        static public void Highlight(string aMsg, params object[] aParams)
        {
            m_staticLogger.Trace(Logger.LogLevel.Highlight, string.Format(aMsg, aParams));
        }

        static public void Info(string aMsg, params object[] aParams)
        {
            m_staticLogger.Trace(Logger.LogLevel.Info, string.Format(aMsg, aParams));
        }

        static public void Warning(string aMsg, params object[] aParams)
        {
            m_staticLogger.Trace(Logger.LogLevel.Warning, string.Format(aMsg, aParams));
        }

        static public void Error(string aMsg, params object[] aParams)
        {
            m_staticLogger.Trace(Logger.LogLevel.Error, string.Format(aMsg, aParams));
        }

        static public void Always(string aMsg, params object[] aParams)
        {
            m_staticLogger.Trace(Logger.LogLevel.Always, string.Format(aMsg, aParams));
        }

        static public void Exception(System.Exception aException)
        {
            m_staticLogger.Trace(Logger.LogLevel.Exception, aException.Message);

            string[] exceptionLines = aException.ToString().Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string eee in exceptionLines)
            {
                m_staticLogger.Trace(Logger.LogLevel.Exception, eee);
            }
        }

        /// <summary>
        /// meant to process a set of arguments likely from program.exe execution
        /// </summary>
        /// <param name="args"></param>
        static public void ParseArgs(string[] args)
        {
            int argIndex = 0;
            for (argIndex = 0; argIndex < args.Length; ++argIndex)
            {
                string arg = args[argIndex];
                if ((arg == "-loglevel") || (arg == "--loglevel"))
                {
                    ++argIndex;
                    bool bWasSet = false;
                    try
                    {
                        Logging.GetSingleton().TheLogLevel = (Logger.LogLevel)Enum.Parse(typeof(Logger.LogLevel), args[argIndex], true);
                        bWasSet = true;
                    }
                    catch { }
                    try
                    {
                        if (!bWasSet)
                        {
                            Logging.GetSingleton().TheLogLevel = (Logger.LogLevel)int.Parse(args[argIndex]);
                            bWasSet = true;
                        }
                    }
                    catch {}
                }
            }
        }

        static public Logger GetSingleton()
        {
            return m_staticLogger;
        }
    }

    public class Utility
    {
        public static System.Net.IPAddress ResolveAddress(string aSzIpAddress)
        {
            System.Net.IPAddress ipAddress = null;
            if (System.Net.IPAddress.TryParse(aSzIpAddress, out ipAddress))
            {
                return ipAddress;
            }
            System.Net.IPHostEntry ipHost = System.Net.Dns.GetHostEntry(aSzIpAddress);
            foreach (var ip in ipHost.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }
    }
}
