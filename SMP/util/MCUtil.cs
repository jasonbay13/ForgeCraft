using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace SMP
{
    /// <summary>
    /// Various utilities for Minecraft.
    /// </summary>
    public static class MCUtil
    {
        /// <summary>
        /// Utilities for working with certain data types in the Minecraft protocol.
        /// </summary>
        public static class Protocol
        {
            /// <summary>
            /// Gets an array of bytes for a string.
            /// </summary>
            /// <param name="source">The string to get the bytes of.</param>
            /// <returns>Byte array of string.</returns>
            public static byte[] GetBytes(string source)
            {
                if (source.Length > short.MaxValue) throw new ArgumentException("String too big.");

                byte[] bytes = new byte[2 + (source.Length * 2)];
                util.EndianBitConverter.Big.GetBytes((short)source.Length).CopyTo(bytes, 0);
                Encoding.BigEndianUnicode.GetBytes(source).CopyTo(bytes, 2);
                return bytes;
            }

            /// <summary>
            /// Gets the length of an array of bytes for a string.
            /// </summary>
            /// <param name="source">The string to get the bytes of.</param>
            /// <returns>Byte array of string.</returns>
            public static short GetBytesLength(string source)
            {
                if (source.Length > short.MaxValue) throw new ArgumentException("String too big.");
                return (short)(2 + source.Length * 2);
            }

            /// <summary>
            /// Gets a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <returns>String read from the byte array.</returns>
            public static string GetString(byte[] bytes, int index)
            {
                return GetString(bytes, index, short.MaxValue);
            }

            /// <summary>
            /// Gets a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <param name="maxLength">Maximum length of the string to read.</param>
            /// <returns>String read from the byte array.</returns>
            public static string GetString(byte[] bytes, int index, short maxLength)
            {
                short length = util.EndianBitConverter.Big.ToInt16(bytes, index);
                if (length > maxLength)
                    throw new Exception(
                        new StringBuilder("Recieved tring length is longer than maximum allowed. (").Append(length).
                            Append(" > ").Append(maxLength).Append(")").ToString());
                if (length < 0) throw new Exception("Received string length is less than zero! Weird string!");

                return Encoding.BigEndianUnicode.GetString(bytes, index + 2, length * 2);
            }

            /// <summary>
            /// Gets the length of a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <returns>String read from the byte array.</returns>
            public static short GetStringLength(byte[] bytes, int index)
            {
                return GetStringLength(bytes, index, short.MaxValue);
            }

            /// <summary>
            /// Gets the length of a string from a byte array starting at the specified index
            /// </summary>
            /// <param name="bytes">Byte array to get string from.</param>
            /// <param name="index">Index to starting read bytes at.</param>
            /// <param name="maxLength">Maximum length of the string to read.</param>
            /// <returns>String read from the byte array.</returns>
            public static short GetStringLength(byte[] bytes, int index, short maxLength)
            {
                short length = util.EndianBitConverter.Big.ToInt16(bytes, index);
                if (length > maxLength)
                    throw new Exception(
                        new StringBuilder("Recieved tring length is longer than maximum allowed. (").Append(length).
                            Append(" > ").Append(maxLength).Append(")").ToString());
                if (length < 0) throw new Exception("Received string length is less than zero! Weird string!");

                return (short)(2 + length * 2);
            }
        }

        public static class Entities
        {
            public static byte[] GetMetaBytes(object[] data)
            {
                List<byte> bytes = new List<byte>();
                object obj;
                for (int i = 0; i < data.Length; i++)
                {
                    if (i > 31) break; // Maximum index is 31 due to the index being 5 bits.
                    obj = data[i];
                    if (obj == null) continue;
                    if (obj is byte)
                    {
                        bytes.Add((byte)(i & 0x1F));
                        bytes.Add((byte)obj);
                    }
                    else if (obj is short)
                    {
                        bytes.Add((byte)(0x01 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((short)obj));
                    }
                    else if (obj is int)
                    {
                        bytes.Add((byte)(0x02 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)obj));
                    }
                    else if (obj is float)
                    {
                        bytes.Add((byte)(0x03 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((float)obj));
                    }
                    else if (obj is string)
                    {
                        bytes.Add((byte)(0x04 << 5 | i & 0x1F));
                        bytes.AddRange(MCUtil.Protocol.GetBytes((string)obj));
                    }
                    else if (obj.GetType() == typeof(Item))
                    {
                        Item item = (Item)obj;
                        bytes.Add((byte)(0x05 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes(item.id));
                        bytes.Add(item.count);
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes(item.meta));
                    }
                    else if (obj is Point3)
                    {
                        Point3 point = (Point3)obj;
                        bytes.Add((byte)(0x06 << 5 | i & 0x1F));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.x));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.y));
                        bytes.AddRange(util.EndianBitConverter.Big.GetBytes((int)point.z));
                    }
                }
                bytes.Add(0x7F);
                return bytes.ToArray();
            }
        }

        public static class DeveloperUtils
        {
            public static string ReadLineFromFile(string fileName, int lineNumber = 0)
            {
                if (String.IsNullOrWhiteSpace(fileName)) throw new Exception("File name is null");
                if(!File.Exists(fileName)) throw new FileNotFoundException(string.Format("File: {0} doesn't exist", fileName));
                using(var sr = new StreamReader(fileName))
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
                using(var sw = append ? File.AppendText(fileName) : new StreamWriter(fileName))
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
            public static bool CreateEmptyTextFile(string fileName)
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
                string[] lines = new string[] {};
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
    }
}
