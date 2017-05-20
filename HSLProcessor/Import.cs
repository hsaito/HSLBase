using System;
using System.IO;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class Importer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Importer));
        public enum ImportResult { Success, Failed }

        public static ImportResult ImportCsv(string filename)
        {
            try
            {
                HSLContext context = new HSLContext();
                var file = new FileInfo(filename);

                if (!file.Exists)
                {
                    Log.Error("No such file.");
                    return ImportResult.Failed;
                }

                var result = CSVParser.Load(new FileInfo(file.FullName));

                Console.Write("Importing");
                foreach (var item in result)
                {
                    Console.Write(".");
                    var song = new Song
                    {
                        //Id = Guid.NewGuid(),
                        Title = item.title,
                        Artist = item.artist,
                        Reference = item.source,
                    };

                    context.Songs.Add(song);
                }
                Console.WriteLine("Done");
                context.SaveChanges();
                return ImportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed importing CSV.");
                Log.Debug(ex.Message);
                return ImportResult.Failed;
            }
        }

        public static ImportResult ImportXml(FileInfo file)
        {
            try
            {
                HSLContext context = new HSLContext();
                XElement xl = XElement.Load(file.FullName);

                foreach(var item in xl.Elements("entry"))
                {
                    var entry = new Song();
                    entry.Id = new Guid(item.Attribute("id").Value);
                    entry.Title = item.Element("title").Value;
                    entry.Artist = item.Element("artist").Value;
                    entry.Reference = item.Element("source").Value;
                    context.Add(entry);
                }
                context.SaveChanges();
                return ImportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed importing XML.");
                Log.Debug(ex.Message);
                return ImportResult.Failed;
            }
        }

    }
}