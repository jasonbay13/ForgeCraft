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
            public static string ReadLineFromFile(string fileName, int lineNumber = 0)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: {0} doesn't exist", fileName));
                using (var sr = new StreamReader(fileName))
                {
                    return sr.ReadLine();
                }
            }

            public static string[] ReadLinesFromFile(string fileName)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                using (var sr = new StreamReader(fileName))
                {
                    return sr.ReadToEnd().Split('\n');
                }
            }
            public static bool WriteLineToFile(string fileName, string line, bool append = true)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                using (var sw = append ? File.AppendText(fileName) : new StreamWriter(fileName))
                {
                    try
                    {
                        sw.WriteLine(line);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
            public static bool CreateEmptyFile(string fileName)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (File.Exists(fileName)) throw new Exception(string.Format("File: \"{0}\" already exists", fileName));
                try
                {
                    File.CreateText(fileName).Close();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            public static bool DeleteFile(string fileName)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                try
                {
                    File.Delete(fileName);
                    return true;
                }
                catch
                {
                    return false;
                }

            }
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
                            lines = response.Split('\n');
                            if (lineNumber >= lines.Count())
                                throw new Exception(
                                    "Line number exceeds number of lines in website");
                            //maybe add callback feature?
                        })).Start();
                return lines[lineNumber];
            }

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

            public static int LinesCountInFile(string fileName)
            {
                return ReadLinesFromFile(fileName).Count();
            }

            public static int LinesCountInWebsite(string siteName)
            {
                return ReadLinesFromWebsite(siteName).Count();
            }

        }
        public static class PropertyTools
        {
            public static void CreatePropertyFile(string name)
            {
                if (!FileTools.CreateEmptyFile(name))
                    throw new Exception("An error occurred in creating property file");
            }
            public static void DeletePropertyFile(string name)
            {
                if (!FileTools.DeleteFile(name))
                    throw new Exception("An error occurred in deleting property file");
            }
            public static bool NodeExists(string fileName, string nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                string[] lines = FileTools.ReadLinesFromFile(fileName);
                return lines.Any(line => line.Split('=')[0].Contains(nodeName));
            }
            public static void CreateNode(string fileName, string nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (NodeExists(fileName, nodeName)) throw new Exception("Node already exists");
                FileTools.WriteLineToFile(fileName, nodeName + " = default");

            }
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
            public static string ReadNode(string fileName, string nodeName)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (!NodeExists(fileName, nodeName)) throw new Exception("Node not found");
                string[] lines = FileTools.ReadLinesFromFile(fileName);
                return (from line in lines where line.Split('=').Contains(nodeName) select line.Substring(1)).FirstOrDefault();
            }
            public static void WriteNode(string fileName, string nodeName, string nodeProperty)
            {
                if (String.IsNullOrWhiteSpace(fileName) || String.IsNullOrWhiteSpace(nodeName)) throw new Exception("Cannon have null string");
                if (!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: \"{0}\" doesn't exist", fileName));
                if (!NodeExists(fileName, nodeName)) throw new Exception("Node not found");
                string[] lines = FileTools.ReadLinesFromFile(fileName);
                for (int i = 0; i < lines.Count(); i++)
                {
                    if (lines[i].Split('=').Contains(nodeName))
                        lines[i] = string.Format("{0} = {1}", nodeName, nodeProperty);
                    FileTools.WriteLineToFile(fileName, lines[i], false);
                }


            }


        }
    }
}
