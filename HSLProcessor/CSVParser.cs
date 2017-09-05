using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace HSLProcessor
{
    public static class CsvParser
    {
        public enum OperationMode
        {
            Songlist,
            SeriesSource
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(CsvParser));

        /// <summary>
        ///     Load and parse CSV file
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="hasHeader">Whether the file has a header</param>
        /// <param name="opMode">Operation mode to use</param>
        /// <returns>List of CSV content</returns>
        public static IEnumerable<CsvContent> Load(FileInfo file, OperationMode opMode = OperationMode.Songlist,
            bool hasHeader = true)
        {
            try
            {
                var list = new List<CsvContent>();
                var reader = new StreamReader(new FileStream(file.FullName, FileMode.Open));

                // If no header, grab one from index 0
                var i = 0;
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

                        var content = new CsvContent {Entry = new Dictionary<string, string>()};

                        switch (opMode)
                        {
                            case OperationMode.Songlist:
                            {
                                content.Entry.Add("title", splitted[0]);
                                content.Entry.Add("artist", splitted[1]);

                                // If third argument is present, that's source/reference
                                content.Entry.Add("source", splitted.Length == 3 ? splitted[2] : "");
                                break;
                            }
                            case OperationMode.SeriesSource:
                            {
                                content.Entry.Add("source", splitted[0]);
                                content.Entry.Add("series", splitted[1]);
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

        public class CsvContent
        {
            public Dictionary<string, string> Entry;
        }
    }
}