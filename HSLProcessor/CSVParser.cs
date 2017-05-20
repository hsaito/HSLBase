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

        public static List<CSVContent> Load(FileInfo file)
        {
            try
            {
                var list = new List<CSVContent>();
                var reader = new StreamReader(new FileStream(file.FullName, FileMode.Open));

                int i = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (i > 0)
                    {
                        var splitted = line.Split(',');

                        var content = new CSVContent();
                        content.title = splitted[0];
                        content.artist = splitted[1];

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