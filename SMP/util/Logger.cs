/*COPYRIGHT
	Copyright 2011 ForgeCraft team
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
	the GNU General Public License, Version 3 (the "Licenses"); you may
	not use this file except in compliance with the Licenses. You may
	obtain a copy of the Licenses at
	
	http://www.opensource.org/licenses/ecl2.php
	http://www.gnu.org/licenses/gpl-3.0.html
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the Licenses are distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the Licenses for the specific language governing
	permissions and limitations under the Licenses.
*/

using System;
using System.Collections.Generic;
using System.IO;
using java.lang;
using Exception = System.Exception;
using String = System.String;
using Thread = System.Threading.Thread;

/*
 ****    FILE TODO:   *****
 * - add in logging based on levels
 * - maybe have filenames passed to constructor for other logging i.e. plugins
 */

namespace SMP.util
{

    public static class Logger
    {
        static readonly string LogFile = "/logs/" + DateTime.Now.ToString("yyyy-MM-dd") + "_Logger.Log";
        static readonly string ErrorFile = "/logs/errors/" + DateTime.Now.ToString("yyyy-MM-dd") + "_error.log";

        private static readonly Queue<string> messageQueue = new Queue<string>();
        public delegate void Logs(string message);
        public static event Logs OnLog;

        private static bool Disposing { get; set; }

        public static void Init()
        {
            Disposing = false;

            if (!Directory.Exists("logs")) Directory.CreateDirectory("logs");
            if (!Directory.Exists("logs/errors")) Directory.CreateDirectory("logs/errors");

        }

        /// <summary>
        /// Logs to file and console
        /// </summary>
        /// <param name="log"></param>
        public static void Log(string log, bool logToFile = true)
        {
            if (String.IsNullOrWhiteSpace(log)) throw new NullPointerException("Message Cannot Be Null or Empty");
            messageQueue.Enqueue(FormatTime() + "  " + log);
            if (OnLog != null) OnLog(messageQueue.Dequeue());
            if (logToFile)
                LogToFile(log);
        }

        /// <summary>
        /// Logs to file and console, takes a loglevel
        /// </summary>
        /// <param name="level"></param>
        /// <param name="log"></param>
        public static void Log(LogLevel level, string log, bool logToFile = true)
        {
            if (String.IsNullOrWhiteSpace(log)) throw new NullPointerException("Message Cannot Be Null or Empty");
            messageQueue.Enqueue(FormatTime() + "  " + log);
            if (OnLog != null) OnLog(messageQueue.Dequeue());
            if (logToFile)
                LogToFile(log);

        }

        /// <summary>
        /// Formats and logs to file and console, takes a loglevel
        /// </summary>
        /// <param name="level"></param>
        /// <param name="log"></param>
        /// <param name="args"></param>
        public static void LogFormat(LogLevel level, string log, params object[] args)
        {
            Log(level, string.Format(log, args));
        }

        /// <summary>
        /// Logs Errors
        /// </summary>
        /// <param name="e"></param>
        public static void LogError(Exception e, bool logToFile = true)
        {
            messageQueue.Enqueue(FormatTime() + " [ERROR] " + e.Message + " (See error log for details!)");
            if (OnLog != null) OnLog(messageQueue.Dequeue());
            if (logToFile)
                LogErrorToFile(e);
        }

        /// <summary>
        /// Logs errors and accepts loglevels
        /// </summary>
        /// <param name="level"></param>
        /// <param name="e"></param>

        public static void LogError(LogLevel level, Exception e, bool logToFile = true)
        {
            messageQueue.Enqueue(FormatTime() + " [ERROR] " + e.Message + " (See error log for details!)");
            if (OnLog != null) OnLog(messageQueue.Dequeue());
            if (logToFile)
                LogErrorToFile(e);

        }

        /// <summary>
        /// logs to file, logs to error file if set to true
        /// </summary>
        public static void LogToFile(string log)
        {
        retry:
            int retred = 0;
            try
            {
                if (retred == 5) return;
                if (!Directory.Exists("/logs"))
                {
                    Directory.CreateDirectory("/logs");
                }

                using (StreamWriter fh = File.AppendText(LogFile))
                {
                    fh.WriteLine(FormatTime() + ":  " + log);
                }
            }
            catch (IOException) { retred++; goto retry; }
        }

        /// <summary>
        /// logs errors to file
        /// </summary>
        /// <param name="e"></param>
        public static void LogErrorToFile(Exception e)
        {
        retry:
            int retred = 0;
            try
            {
                if (retred == 5) return;
                if (!Directory.Exists("/logs/errors"))
                    Directory.CreateDirectory("/logs/errors");

                using (StreamWriter fh = File.AppendText(ErrorFile))
                {
                    fh.WriteLine(FormatTime() + " " + e.GetType().Name + ": " + e.Message);
                    fh.WriteLine(e.StackTrace);
                }
            }
            catch (IOException) { retred++; goto retry; }

            // saved for lulz
            // Console.WriteLine("There was an error in the error. ERRORCEPTION!");
        }


        /// <summary>
        /// formats time for output
        /// </summary>
        /// <returns></returns>
        public static string FormatTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }
    }

    public enum LogLevel
    {
        Trivial = -1,
        Debug = 0,
        Info = 1,
        Warning = 2,
        Caution = 3,
        Notice = 4,
        Error = 5,
        Fatal = 6
    }
}
