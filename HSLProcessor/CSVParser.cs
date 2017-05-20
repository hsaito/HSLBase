using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace HSLProcessor
{
    public static class CSVParser
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(CSVParser));
        public class CSVContent
        {
            public string title;
            public string artist;
            public string source;
        }

        /// <summary>
        /// Load and parse CSV file
        /// </summary>
        /// <param name="file">CSV file</param>
        /// <param name="hasHeader">Whether the file has a header</param>
        /// <returns>List of CSV content</returns>
        public static List<CSVContent> Load(FileInfo file, bool hasHeader = true)
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
                        content.title = splitted[0];
                        content.artist = splitted[1];

                        // If third argument is present, that's source/reference
                        if (splitted.Length == 3)
                        {
                            content.source = splitted[2];
                        }
                        else
                        {
                            content.source = "";
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