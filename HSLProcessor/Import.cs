using System;
using System.IO;
using System.Linq;
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
        public static ImportResult ImportCsv(FileInfo file)
        {
            return ImportCsvAsync(file).Result;
        }

        /// <summary>
        /// Import CSV file
        /// </summary>
        /// <param name="filename">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static async Task<ImportResult> ImportCsvAsync(FileInfo file)
        {
            try
            {
                Log.Info("Importing a CSV file " + file.FullName);
                HSLContext context = new HSLContext();

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

                    song.Title = item.entry["title"];

                    var artist = new Artist();
                    artist.Name = item.entry["artist"];
                    var source = new Source();
                    source.Name = item.entry["item.source"];

                    var artist_item = Utils.GetOrAddArtist(artist, ref context).ArtistId;
                    var source_item = Utils.GetOrAddSource(source, ref context).SourceId;

                    song.ArtistId = artist_item;
                    song.SourceId = source_item;

                    // Add to the DB
                    await context.Songs.AddAsync(song);
                }
                Console.WriteLine("Done");

                Console.Write("Saving...");
                // Save to DB
                await context.SaveChangesAsync(true);
                Console.WriteLine(" Done");
                Log.Info("Import process completed.");
                return ImportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed importing CSV.");
                Log.Debug(ex.Message);
                Log.Debug(ex.StackTrace);
                if (ex.InnerException != null)
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
                Log.Info("Importing XML file " + file.Name);
                HSLContext context = new HSLContext();

                // Load the file
                XElement xl = XElement.Load(file.FullName);

                Log.Info("Importing songs element...");

                // Process each entry
                foreach (var item in xl.Element("songs").Elements("entry"))
                {
                    var entry = new Song();
                    entry.TitleId = new Guid(item.Attribute("id").Value);
                    entry.Title = item.Element("title").Value;
                    entry.ArtistId = new Guid(item.Element("artist").Attribute("id").Value);
                    entry.SourceId = new Guid(item.Element("source").Attribute("id").Value);

                    // Create and add artist entry
                    var artist = new Artist();
                    if (item.Element("artist") != null)
                    {
                        artist.ArtistId = entry.ArtistId;
                        artist.Name = item.Element("artist").Value;
                        Utils.GetOrAddArtist(artist, ref context);
                    }
                    else
                    {
                        artist.Name = "";
                        Utils.GetOrAddArtist(artist, ref context);
                    }

                    // Create and add source entry
                    var source = new Source();
                    if (item.Element("source") != null)
                    {
                        source.SourceId = entry.SourceId;
                        source.Name = item.Element("source").Value;
                        Utils.GetOrAddSource(source, ref context);
                    }
                    else
                    {
                        source.Name = "";
                        Utils.GetOrAddSource(source, ref context);
                    }


                    // Add to DB
                    await context.AddAsync(entry);
                    await context.SaveChangesAsync();
                }

                Log.Info("Importing series element...");

                foreach (var item in xl.Element("series").Elements("entry"))
                {
                    var series = new Series();
                    series.SeriesId = new Guid(item.Attribute("id").Value);
                    series.Name = item.Element("name").Value;
                    Utils.GetOrAddSeries(series, ref context);

                    // Add to DB
                    await context.SaveChangesAsync();
                }

                Log.Info("Importing sources element...");

                foreach (var item in xl.Element("sources").Elements("entry"))
                {
                    var source = new Source();
                    if(item.Element("series") != null)
                    {
                        var series_id = new Guid(item.Element("series").Attribute("id").Value);
                        var source_id = new Guid(item.Attribute("id").Value);
                        var source_item = context.Sources.Find(source_id);
                        if(source_item != null)
                        {
                            source_item.SeriesId = series_id;
                            context.Sources.Update(source_item);
                            await context.SaveChangesAsync();
                        }
                    }
                }

                // Save to DB
                context.LoadRelations();
                await context.SaveChangesAsync();
                Log.Info("Import completed.");
                return ImportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed importing XML.");
                Log.Debug(ex.Message);
                Log.Debug(ex.StackTrace);
                if (ex.InnerException != null)
                    Log.Error(ex.InnerException.Message);
                return ImportResult.Failed;
            }
        }

        /// <summary>
        /// Synchronous wrapper for importing Series CSV file
        /// </summary>
        /// <param name="filename">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportSourceSeriesCsv(FileInfo file)
        {
            return ImportSourceSeriesCsvAsync(file).Result;
        }

        /// <summary>
        /// Import reference CSV file
        /// </summary>
        /// <param name="filename">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static async Task<ImportResult> ImportSourceSeriesCsvAsync(FileInfo file)
        {
            try
            {
                Log.Info("Importing a CSV file " + file.FullName);
                HSLContext context = new HSLContext();

                // Check to see the file exists
                if (!file.Exists)
                {
                    Log.Error("No such file.");
                    return ImportResult.Failed;
                }

                // Parse CSV
                var result = CSVParser.Load(new FileInfo(file.FullName), CSVParser.operation_mode.Series_Source);

                // Begin import
                Console.Write("Importing");
                foreach (var item in result)
                {
                    Console.Write(".");
                    var source_series = new Series();

                    source_series.Name = item.entry["source"];

                    var series = new Series();
                    var source = new Source();
                    source.Name = item.entry["source"];
                    series.Name = item.entry["series"];

                    var series_item = Utils.GetOrAddSeries(series, ref context).SeriesId;
                    var source_item = Utils.GetOrAddSource(source, ref context).SourceId;

                    var db_source_list = context.Sources.Where((entry) => entry.SourceId == source_item).ToList();

                    foreach (var source_entry in db_source_list)
                    {
                        source_entry.SeriesId = series_item;
                    }

                    context.Sources.UpdateRange(db_source_list);
                }
                Console.WriteLine("Done");

                Console.Write("Saving...");
                // Save to DB
                await context.SaveChangesAsync(true);
                Console.WriteLine(" Done");
                Log.Info("Import process completed.");
                return ImportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed importing CSV.");
                Log.Debug(ex.Message);
                Log.Debug(ex.StackTrace);
                if (ex.InnerException != null)
                    Log.Debug(ex.InnerException.Message);
                return ImportResult.Failed;
            }
        }

    }
}