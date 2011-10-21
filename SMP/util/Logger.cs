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
using System.Linq;
using System.Text;
using System.IO;

/*
 ****    FILE TODO:   *****
 * - add in logging based on levels
 * - maybe have filenames passed to constructor for other logging i.e. plugins
 */

namespace SMP
{
    
    public class Logger
    {
        public static Logger log = new Logger();
        string LogFile = Environment.CurrentDirectory + "/logs/" + DateTime.Now.ToString("yyyy-MM-dd") + "_server.log";
        string ErrorFile = Environment.CurrentDirectory + "/logs/errors/" + DateTime.Now.ToString("yyyy-MM-dd") + "_error.log";


        /// <summary>
        /// Logs to file and console
        /// </summary>
        /// <param name="log"></param>
        public void Log(string log)
        {
            if (!Server.useGUI) { Console.WriteLine(FormatTime() + "  " + log); }
            else { GUI.MainWindow.Log(FormatTime() + "  " + log); }
            LogToFile(log);
        }

        /// <summary>
        /// Logs to file and console, takes a loglevel
        /// </summary>
        /// <param name="level"></param>
        /// <param name="log"></param>
        public void Log(LogLevel level, string log)
        {
            if (!Server.useGUI) { Console.WriteLine(FormatTime() + "  " + log); }
            else { GUI.MainWindow.Log(FormatTime() + "  " + log); }
            LogToFile(log);

        }

        /// <summary>
        /// Formats and logs to file and console, takes a loglevel
        /// </summary>
        /// <param name="level"></param>
        /// <param name="log"></param>
        /// <param name="args"></param>
        public void Log(LogLevel level, string log, params object[] args)
        {
            this.Log( level, string.Format(log, args));
        }

        /// <summary>
        /// Logs Errors
        /// </summary>
        /// <param name="e"></param>
        public void LogError(Exception e)
        {
            if (!Server.useGUI) { Console.WriteLine(FormatTime() + " [ERROR] " + e.Message + " (See error log for details!)"); }
            else { GUI.MainWindow.Log(FormatTime() + " [ERROR] " + e.Message + " (See error log for details!)"); }
            LogErrorToFile(e);
        }

        /// <summary>
        /// Logs errors and accepts loglevels
        /// </summary>
        /// <param name="level"></param>
        /// <param name="e"></param>

        public void LogError(LogLevel level, Exception e)
        {
            if (!Server.useGUI) { Console.WriteLine(FormatTime() + " [ERROR] " + e.Message + " (See error log for details!)"); }
            else { GUI.MainWindow.Log(FormatTime() + " [ERROR] " + e.Message + " (See error log for details!)"); }
            LogErrorToFile(e);
        
        }

        /// <summary>
        /// logs to file, logs to error file if set to true
        /// </summary>
        public void LogToFile(string log)
        {
            retry:
            int retred = 0;
            try
            {
                if (retred == 5) return;
                if (!Directory.Exists(Environment.CurrentDirectory + "/logs"))
                {
                    Directory.CreateDirectory(Environment.CurrentDirectory + "/logs");
                }
            
                using (StreamWriter fh = File.AppendText(LogFile))
                {
                    fh.WriteLine(FormatTime() + ":  " + log);
                }
            }
            catch (System.IO.IOException) { retred++; goto retry; }
        }

        /// <summary>
        /// logs errors to file
        /// </summary>
        /// <param name="e"></param>
        public void LogErrorToFile(Exception e)
        {
            retry:
            int retred = 0;
            try
            {
                if (retred == 5) return;
                if (!Directory.Exists(Environment.CurrentDirectory + "/logs/errors"))
                    Directory.CreateDirectory(Environment.CurrentDirectory + "/logs/errors");

                using (StreamWriter fh = File.AppendText(ErrorFile))
                {
                    fh.WriteLine(FormatTime() + " " + e.GetType().Name + ": " + e.Message);
                    fh.WriteLine(e.StackTrace);
                    fh.Write(e.StackTrace);
                    fh.WriteLine();
                }
            }
            catch (System.IO.IOException) { retred++; goto retry; }

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

    public enum LogLevel : int
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
