using System;
using System.IO;
using log4net;

namespace HSLProcessor
{
    static class Importer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Importer));
        public enum ImportResult { Success, Failed }

        public static ImportResult Import(string filename)
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
                        Id = Guid.NewGuid(),
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
                Log.Error("Import process failed.");
                Log.Debug(ex.Message);
                return ImportResult.Failed;
            }
        }
    }
}