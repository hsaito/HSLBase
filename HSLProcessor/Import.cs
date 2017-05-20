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

        /// <summary>
        /// Import CSV file
        /// </summary>
        /// <param name="filename">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportCsv(string filename)
        {
            try
            {
                HSLContext context = new HSLContext();
                var file = new FileInfo(filename);

                // Check to see the file exists
                if (!file.Exists)
                {
                    Log.Error("No such file.");
                    return ImportResult.Failed;
                }

                // Parse CSV
                var result = CSVParser.Load(new FileInfo(file.FullName));

                // Begin import
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

                    // Add to the DB
                    context.Songs.Add(song);
                }
                Console.WriteLine("Done");

                // Save to DB
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

        /// <summary>
        /// Import XML file
        /// </summary>
        /// <param name="file">XML file to import</param>
        /// <returns>Result of the import<</returns>
        public static ImportResult ImportXml(FileInfo file)
        {
            try
            {
                HSLContext context = new HSLContext();

                // Load the file
                XElement xl = XElement.Load(file.FullName);

                // Process each entry
                foreach (var item in xl.Elements("entry"))
                {
                    var entry = new Song();
                    entry.Id = new Guid(item.Attribute("id").Value);
                    entry.Title = item.Element("title").Value;
                    entry.Artist = item.Element("artist").Value;
                    entry.Reference = item.Element("source").Value;
                    // Add to DB
                    context.Add(entry);
                }
                // Save to DB
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