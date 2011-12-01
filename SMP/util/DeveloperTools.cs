using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace SMP.util
{
    /// <summary>
    /// Plugin Developer Tools for File management.</summary>
    /// <remarks>
    /// Read string lines from websites as well as files</remarks>
    public static class DeveloperTools
    {
        public static class FileTools
        {
            /// <summary>
            /// Read Line from file </summary>
            /// <param name="fileName"> name of the file, directories are not ignored.</param>
            /// <param name="lineNumber"> line index (starts at 0)</param>
            /// <value>Returns string line from file (if its not null)</value>
            public static string ReadLineFromFile(string fileName, int lineNumber = 0)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: {0} doesn't exist", fileName));
                return ReadLinesFromFile(fileName)[lineNumber];
            }

            /// <summary>
            /// Read Lines from file </summary>
            /// <param name="fileName"> name of the file, directories are not ignored.</param>
            /// <value>Returns string array from file (if its not null)</value>
            public static string[] ReadLinesFromFile(string fileName)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                return File.ReadAllLines(fileName);
            }

            /// <summary>
            /// Writes a line to file</summary>
            /// <param name="fileName"> name of the file, directories are not ignored.</param>
            /// <param name="line">String content to write to file</param>
            /// <param name="append">if true, file will not be overwritten</param>
            public static void WriteLineToFile(string fileName, string line, bool append = true)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                using (var sw = append ? File.AppendText(fileName) : new StreamWriter(fileName))
                {
                        sw.WriteLine(line);
                }
            }

            /// <summary>
            /// Creates a file that has no data inside </summary>
            /// <param name="fileName"> name of the file, directories are not ignored.</param>
            public static void CreateEmptyFile(string fileName)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (File.Exists(fileName)) throw new Exception(string.Format("File: \"{0}\" already exists", fileName));
                    File.CreateText(fileName).Close();
            }

            /// <summary>
            /// Deletes a file</summary>
            /// <param name="fileName"> Name of the file, directories are not ignored.</param>
            public static void DeleteFile(string fileName)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                    File.Delete(fileName);

            }

            /// <summary>
            /// Clears data from a file</summary>
            /// <param name="fileName"> Name of the file, directories are not ignored.</param>
            public static void ClearFile(string fileName)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                File.Delete(fileName);
                File.Create(fileName).Close();
            }


            /// <summary>
            /// Read Line from website. A protocol must be specified or it will not have expected results </summary>
            /// <param name="siteName"> address of website</param>
            /// <param name="lineNumber"> line index (starts at 0)</param>
            /// <value>Returns string line from site (if its not null)</value>
            public static string ReadLineFromWebsite(string siteName, int lineNumber = 0)
            {
                string response;
                string[] lines = new string[] { };
                new Thread(
                    new ThreadStart(
                        delegate
                        {
                            using (var web = new WebClient())
                            {
                                try
                                {
                                    response = web.DownloadString(siteName);
                                }
                                catch (WebException)
                                {
                                    throw new WebException();
                                }
                            }
                            if (String.IsNullOrWhiteSpace(response))
                                throw new Exception("Website Returned No Information");
                            lines = response.Split(Environment.OSVersion.Platform == PlatformID.Unix ? '\n' : '\r');
                            if (lineNumber >= lines.Count())
                                throw new Exception(
                                    "Line number exceeds number of lines in website");
                            //maybe add callback feature?
                        })).Start();
                return lines[lineNumber];
            }

            /// <summary>
            /// Read Line from website. A protocol must be specified or it will not have expected results </summary>
            /// <param name="siteName"> address of website</param>
            /// <value>Returns string line from site (if its not null)</value>
            /// <remarks>
            ///
            /// </remarks>
            public static string[] ReadLinesFromWebsite(string siteName)
            {
                string response = null;
                new Thread(
                    new ThreadStart(
                        delegate
                        {
                            using (var web = new WebClient())
                            {
                                try
                                {
                                    response = web.DownloadString(siteName);
                                }
                                catch (WebException)
                                {
                                    throw new WebException();
                                }
                            }
                            if (String.IsNullOrWhiteSpace(response))
                                throw new Exception("Website Returned No Information");
                        })).Start();

                return response.Split('\n');

            }

        }
        public static class PropertyTools
        {
            /// <summary>
            ///Creates a properties file that has no data inside You must specify the extension of file, ".properties" is standard</summary>
            /// <param name="name"> name of the file, directories are not ignored.</param>
            public static void CreatePropertyFile(string name)
            {
                FileTools.CreateEmptyFile(name);
            }
            /// <summary>
            ///Creates a properties file that has no data inside You must specify the extension of file, ".properties" is standard</summary>
            /// <param name="name"> name of the file, directories are not ignored.</param>
            public static void DeletePropertyFile(string name)
            {
                FileTools.DeleteFile(name);
            }

            /// <summary>
            /// Check to see if a node exists in the properties file</summary>
            /// <param name="fileName">name of file. You must specify the extension of file, ".properties" is standard</param>
            /// <param name="nodeName">Name of the node you wish to test</param>
            /// <value>Returns result of node existence</value>
            public static bool NodeExists(string fileName, string nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                string[] lines = FileTools.ReadLinesFromFile(fileName);
                return lines.Any(line => line.Split('=')[0].Contains(nodeName));
            }

            /// <summary>
            /// Create a new node</summary>
            /// <param name="fileName">name of file. You must specify the extension of file, ".properties" is standard</param>
            /// <param name="nodeName">name of new node</param>
            public static void CreateNode(string fileName, string nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (NodeExists(fileName, nodeName)) throw new Exception("Node already exists");
                FileTools.WriteLineToFile(fileName, nodeName + " = default", true);

            }

            public static void CreateNode(string fileName, params string[] nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName.Any().ToString())) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (NodeExists(fileName, nodeName.Any().ToString())) throw new Exception("Node already exists");
                foreach (string s in nodeName)
                {
                    FileTools.WriteLineToFile(fileName, s + " = default", true);
                }
            }

            /// <summary>
            /// Delete a node</summary>
            /// <param name="fileName">name of file. You must specify the extension of file, ".properties" is standard</param>
            /// <param name="nodeName">Name of node you wish to delete</param>
            public static void DeleteNode(string fileName, string nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (!NodeExists(fileName, nodeName)) throw new Exception("Node not found");
                string[] lines = FileTools.ReadLinesFromFile(fileName);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (!lines[i].Split('=').Contains(nodeName))
                        FileTools.WriteLineToFile(fileName, lines[0], false);
                }


            }

            /// <summary>
            /// Get property value from node</summary>
            /// <param name="fileName">name of file. You must specify the extension of file, ".properties" is standard</param>
            /// <param name="nodeName">Name of the node you wish to use</param>
            /// <value>Returns result of the string specified in file</value>
            public static string ReadNode(string fileName, string nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (!NodeExists(fileName, nodeName)) throw new Exception("Node not found");
                string[] lines = FileTools.ReadLinesFromFile(fileName);
                return (from line in lines where line.Split('=')[0].Contains(nodeName) select (line.Split('=').Count()>0 ? line.Split('=')[1].Trim() : "null")).FirstOrDefault();
            }

            /// <summary>
            /// Write Property to node</summary>
            /// <param name="fileName">name of file. You must specify the extension of file, ".properties" is standard</param>
            /// <param name="nodeName">Name of the node you wish to test</param>
            /// <param name="nodeValue">Value of node you wish to write</param>
            public static void WriteNode(string fileName, string nodeName, string nodeValue)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (!NodeExists(fileName, nodeName)) throw new Exception("Node not found");
                string[] lines = FileTools.ReadLinesFromFile(fileName);
                FileTools.ClearFile(fileName);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].Split('=')[0].Contains(nodeName))
                        lines[i] = string.Format("{0} = {1}", nodeName, nodeValue);
                    FileTools.WriteLineToFile(fileName, lines[i]);
                }


            }


        }
    }
}
