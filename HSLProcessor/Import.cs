using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class Importer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Importer));
        public enum ImportResult { Success, Failed }

        /// <summary>
        /// Synchronous wrapper for importing CSV file
        /// </summary>
        /// <param name="filename">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportCsv(string filename)
        {
            return ImportCsvAsync(filename).Result;
        }

        /// <summary>
        /// Import CSV file
        /// </summary>
        /// <param name="filename">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static async Task<ImportResult> ImportCsvAsync(string filename)
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
                    var song = new Song();

                    song.Title = item.title;

                    var artist = new Artist();
                    artist.Name = item.artist;
                    var source = new Source();
                    source.Name = item.source;

                    var artist_item = Utils.GetOrAddArtist(artist).Id;
                    var source_item = Utils.GetOrAddSource(source).Id;

                    song.ArtistForeignKey = artist_item;
                    song.SourceForeignKey = source_item;

                    // Add to the DB
                    await context.Songs.AddAsync(song);
                }
                Console.WriteLine("Done");

                Console.Write("Saving...");
                // Save to DB
                await context.SaveChangesAsync();
                Console.WriteLine("Done");
                return ImportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed importing CSV.");
                Log.Debug(ex.Message);
                if(ex.InnerException != null)
                    Log.Debug(ex.InnerException.Message);
                return ImportResult.Failed;
            }
        }

        /// <summary>
        /// Synchronous wrapper for importing XML file
        /// </summary>
        /// <param name="file">XML file to import</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportXml(FileInfo file)
        {
            return ImportXmlAsync(file).Result;
        }

        /// <summary>
        /// Import XML file
        /// </summary>
        /// <param name="file">XML file to import</param>
        /// <returns>Result of the import<</returns>
        public static async Task<ImportResult> ImportXmlAsync(FileInfo file)
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
                    //entry.Artist = item.Element("artist").Value;
                    //entry.Source = item.Element("source").Value;
                    // Add to DB
                    await context.AddAsync(entry);
                }
                // Save to DB
                await context.SaveChangesAsync();
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