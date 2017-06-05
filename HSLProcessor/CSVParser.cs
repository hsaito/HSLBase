using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace HSLProcessor
{
    public static class CSVParser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CSVParser));

        public enum operation_mode { Songlist, Series_Source };

        public class CSVContent
        {
            public Dictionary<string, string> entry;
        }

        /// <summary>
        /// Load and parse CSV file
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="hasHeader">Whether the file has a header</param>
        /// <returns>List of CSV content</returns>
        public static List<CSVContent> Load(FileInfo file, operation_mode op_mode = operation_mode.Songlist, bool hasHeader = true)
        {
            try
            {
                var list = new List<CSVContent>();
                var reader = new StreamReader(new FileStream(file.FullName, FileMode.Open));

                // If no header, grab one from index 0
                int i = 0;
                if (!hasHeader)
                    i = -1;

                // Begin loading
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (i > 0)
                    {
                        // Rudimentary split
                        var splitted = line.Split(',');

                        var content = new CSVContent();
                        content.entry = new Dictionary<string, string>();

                        switch (op_mode)
                        {
                            case operation_mode.Songlist:
                                {
                                    content.entry.Add("title", splitted[0]);
                                    content.entry.Add("artist", splitted[1]);

                                    // If third argument is present, that's source/reference
                                    if (splitted.Length == 3)
                                    {
                                        content.entry.Add("source", splitted[2]);
                                    }
                                    else
                                    {
                                        content.entry.Add("source", "");
                                    }
                                    break;

                                }
                            case operation_mode.Series_Source:
                                {
                                    content.entry.Add("source", splitted[0]);
                                    content.entry.Add("series", splitted[1]);
                                    break;
                                }

                                default:
                                {
                                    throw new Exception("Unsupported operation type.");
                                }

                        }

                        list.Add(content);
                    }
                    i++;
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.Error("Error encountered during parsing CSV.");
                Log.Debug(ex.Message);
                return null;
            }
        }
    }
}