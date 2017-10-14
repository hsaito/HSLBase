using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using HSLProto;
using log4net;

// ReSharper disable UnusedMethodReturnValue.Global

namespace HSLProcessor
{
    internal static class Importer
    {
        public enum ImportResult
        {
            Success,
            Failed
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(Importer));

        /// <summary>
        ///     Synchronous wrapper for importing CSV file
        /// </summary>
        /// <param name="file">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportCsv(FileInfo file)
        {
            return ImportCsvAsync(file).Result;
        }

        /// <summary>
        ///     Import CSV file
        /// </summary>
        /// <param name="file">CSV file to import</param>
        /// <returns>Result of the import</returns>
        private static async Task<ImportResult> ImportCsvAsync(FileSystemInfo file)
        {
            try
            {
                Log.Info("Importing a CSV file " + file.FullName);
                var context = new HSLContext();

                // Check to see the file exists
                if (!file.Exists)
                {
                    Log.Error("No such file.");
                    return ImportResult.Failed;
                }

                // Parse CSV
                var result = CsvParser.Load(new FileInfo(file.FullName));

                // Begin import
                Console.Write("Importing");
                foreach (var item in result)
                {
                    Console.Write(".");
                    var song = new Song {Title = item.Entry["title"]};


                    var artist = new Artist {Name = item.Entry["artist"]};
                    var source = new Source {Name = item.Entry["source"]};

                    var artistItem = Utils.GetOrAddArtist(artist, ref context).ArtistId;
                    var sourceItem = Utils.GetOrAddSource(source, ref context).SourceId;

                    song.ArtistId = artistItem;
                    song.SourceId = sourceItem;

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
        ///     Synchronous wrapper for importing XML file
        /// </summary>
        /// <param name="file">XML file to import</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportXml(FileInfo file)
        {
            return ImportXmlAsync(file).Result;
        }

        /// <summary>
        /// Import protocol buffer file
        /// </summary>
        /// <param name="file">File name to protocol buffer file</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportProtocolBuffer(FileInfo file)
        {
            return ImportProtocolBufferAsync(file).Result;
        }
        
        
        /// <summary>
        /// Import protocol buffer file
        /// </summary>
        /// <param name="file">File name to protocol buffer file</param>
        /// <returns>Result of the import</returns>
        public static async Task<ImportResult> ImportProtocolBufferAsync(FileSystemInfo file)
        {
            try
            {
                HSL proto;
                using (var input = File.OpenRead(file.FullName))
                {
                    proto = HSL.Parser.ParseFrom(input);
                }
                var context = new HSLContext();

                foreach (var item in proto.Songs)
                {
                    var entry = new Song
                    {
                        TitleId = new Guid(item.Id),
                        Title = item.Name
                    };
                    
                    var artist = new Artist();
                    if (item.Artist != -1)
                    {
                        var a = proto.Artists.First(q => q.SerialNumber == item.Artist);
                        entry.ArtistId = new Guid(a.Id);

                        artist.ArtistId = new Guid(a.Id);
                        artist.Name = a.Name;
                        Utils.GetOrAddArtist(artist, ref context);
                    }
                    else
                    {
                        artist.Name = "";
                        Utils.GetOrAddArtist(artist, ref context);
                    }

                    var source = new Source();
                    if (item.Source != -1)
                    {
                        var s = proto.Sources.First(q => q.SerialNumber == item.Source);
                        source.SourceId = new Guid(s.Id);
                        source.Name = s.Name;
                        Utils.GetOrAddSource(source, ref context);

                        var series = new Series();
                        if (s.Series != -1)
                        {
                            var s2 = proto.Series.First(q => q.SerialNumber == s.Series);
                            series.Name = s2.Name;
                            series.SeriesId = new Guid(s2.Id);
                            Utils.GetOrAddSeries(series, ref context);
                        }
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
                return ImportResult.Success;

            }
            catch (Exception ex)
            {
                Log.Error("Failed importing Protocol Buffer.");
                Log.Debug(ex.Message);
                Log.Debug(ex.StackTrace);
                if (ex.InnerException != null)
                    Log.Error(ex.InnerException.Message);
                return ImportResult.Failed;
            }
        }
        
        /// <summary>
        ///     Import XML file
        /// </summary>
        /// <param name="file">XML file to import</param>
        /// <returns>Result of the import</returns>
        private static async Task<ImportResult> ImportXmlAsync(FileSystemInfo file)
        {
            try
            {
                Log.Info("Importing XML file " + file.Name);
                var context = new HSLContext();

                // Load the file
                var xl = XElement.Load(file.FullName);

                Log.Info("Importing songs element...");

                // Process each entry
                // ReSharper disable once PossibleNullReferenceException
                foreach (var item in xl.Element("songs")?.Elements("entry"))
                {
                    var entry = new Song
                    {
                        TitleId = new Guid(item.Attribute("id")?.Value),
                        Title = item.Element("title")?.Value,
                        ArtistId = new Guid(item.Element("artist")?.Attribute("id")?.Value),
                        SourceId = new Guid(item.Element("source")?.Attribute("id")?.Value)
                    };

                    // Create and add artist entry
                    var artist = new Artist();
                    if (item.Element("artist") != null)
                    {
                        artist.ArtistId = entry.ArtistId;
                        artist.Name = item.Element("artist")?.Value;
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
                        source.Name = item.Element("source")?.Value;
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

                // ReSharper disable once PossibleNullReferenceException
                foreach (var item in xl.Element("series")?.Elements("entry"))
                {
                    var series = new Series
                    {
                        SeriesId = new Guid(item.Attribute("id")?.Value),
                        Name = item.Element("name")?.Value
                    };
                    Utils.GetOrAddSeries(series, ref context);

                    // Add to DB
                    await context.SaveChangesAsync();
                }

                Log.Info("Importing sources element...");

                // ReSharper disable once PossibleNullReferenceException
                foreach (var item in xl.Element("sources")?.Elements("entry"))
                {
                    if (item.Element("series") == null) continue;
                    var seriesId = new Guid(item.Element("series")?.Attribute("id")?.Value);
                    var sourceId = new Guid(item.Attribute("id")?.Value);
                    var sourceItem = context.Sources.Find(sourceId);
                    if (sourceItem == null) continue;
                    sourceItem.SeriesId = seriesId;
                    context.Sources.Update(sourceItem);
                    await context.SaveChangesAsync();
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
        ///     Synchronous wrapper for importing Series CSV file
        /// </summary>
        /// <param name="file">CSV file to import</param>
        /// <returns>Result of the import</returns>
        public static ImportResult ImportSourceSeriesCsv(FileInfo file)
        {
            return ImportSourceSeriesCsvAsync(file).Result;
        }

        /// <summary>
        ///     Import reference CSV file
        /// </summary>
        /// <param name="file">CSV file to import</param>
        /// <returns>Result of the import</returns>
        private static async Task<ImportResult> ImportSourceSeriesCsvAsync(FileSystemInfo file)
        {
            try
            {
                Log.Info("Importing a CSV file " + file.FullName);
                var context = new HSLContext();

                // Check to see the file exists
                if (!file.Exists)
                {
                    Log.Error("No such file.");
                    return ImportResult.Failed;
                }

                // Parse CSV
                var result = CsvParser.Load(new FileInfo(file.FullName), CsvParser.OperationMode.SeriesSource);

                // Begin import
                Console.Write("Importing");
                var series = new Series();
                foreach (var item in result)
                {
                    Console.Write(".");
                    var source = new Source {Name = item.Entry["source"]};
                    series.Name = item.Entry["series"];

                    var seriesItem = Utils.GetOrAddSeries(series, ref context).SeriesId;
                    var sourceItem = Utils.GetOrAddSource(source, ref context).SourceId;

                    var dbSourceList = context.Sources.Where(entry => entry.SourceId == sourceItem).ToList();

                    foreach (var sourceEntry in dbSourceList)
                        sourceEntry.SeriesId = seriesItem;

                    context.Sources.UpdateRange(dbSourceList);
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