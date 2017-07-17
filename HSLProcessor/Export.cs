using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using log4net;
using Newtonsoft.Json;

namespace HSLProcessor
{
    static class Exporter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Exporter));
        public enum ExportResult { Success, Failed }

        struct SongEntry
        {
            public string title_id;
            public string title;
            public string artist_id;
            public string artist;
            public string source_id;
            public string source;
            public string series_id;
            public string series;
        }

        /// <summary>
        /// Export to JSON
        /// </summary>
        /// <param name="file">JSON file to export to</param>
        /// <returns>Result of the export</returns>
        public static ExportResult ExportJson(FileInfo file)
        {
            try
            {
                var entry = new List<SongEntry>();
                HSLContext context = new HSLContext();
                context.LoadRelations();

                foreach (var item in context.Songs)
                {
                    var song = new SongEntry();

                    song.title = item.Title;
                    song.title_id = item.TitleId.ToString();
                    song.artist = item.Artist.Name;
                    song.artist_id = item.Artist.ArtistId.ToString();

                    if (item.Source != null)
                    {
                        song.source = item.Source.Name;
                        song.source_id = item.Source.SourceId.ToString();

                        if (item.Source.Series != null)
                        {
                            song.series = item.Source.Series.Name;
                            song.series_id = item.Source.Series.SeriesId.ToString();
                        }
                    }
                    entry.Add(song);
                }
                
                var output = JsonConvert.SerializeObject(entry);

                var writer = new StreamWriter(new FileStream(file.FullName, FileMode.Create));
                writer.Write(output);
                writer.Flush();
                return ExportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to JSON.");
                Log.Debug(ex.Message);
                return ExportResult.Failed;
            }
        }

        /// <summary>
        /// Export to XML
        /// </summary>
        /// <param name="file">XML file to export to</param>
        /// <returns>Result of the export</returns>
        public static ExportResult ExportXml(FileInfo file)
        {
            try
            {
                HSLContext context = new HSLContext();

                context.LoadRelations();

                // Create a root element
                XElement xl = new XElement("hsl");
                XElement xl_songs = new XElement("songs");
                foreach (var item in context.Songs)
                {
                    XElement xl_item = new XElement("entry");
                    xl_item.SetAttributeValue("id", item.TitleId);
                    xl_item.SetElementValue("title", item.Title);

                    XElement xl_artist = new XElement("artist");
                    xl_artist.SetAttributeValue("id", item.Artist.ArtistId);
                    xl_artist.Value = item.Artist.Name;
                    xl_item.Add(xl_artist);

                    XElement xl_source = new XElement("source");
                    xl_source.SetAttributeValue("id", item.Source.SourceId);
                    xl_source.Value = item.Source.Name;
                    xl_item.Add(xl_source);
                    xl_songs.Add(xl_item);
                }
                xl.Add(xl_songs);

                XElement xl_artists = new XElement("artists");
                foreach (var item in context.Artists)
                {
                    XElement xl_item = new XElement("entry");
                    xl_item.SetAttributeValue("id", item.ArtistId);
                    xl_item.SetElementValue("name", item.Name);
                    xl_artists.Add(xl_item);
                }
                xl.Add(xl_artists);

                XElement xl_sources = new XElement("sources");
                foreach (var item in context.Sources)
                {
                    XElement xl_item = new XElement("entry");
                    xl_item.SetAttributeValue("id", item.SourceId);
                    xl_item.SetElementValue("name", item.Name);

                    if (item.Series != null)
                    {
                        xl_item.SetElementValue("series", item.Series.Name);
                        xl_item.Element("series").SetAttributeValue("id", item.Series.SeriesId);
                    }

                    xl_sources.Add(xl_item);
                }

                xl.Add(xl_sources);

                XElement xl_series = new XElement("series");
                foreach (var item in context.Series)
                {
                    XElement xl_item = new XElement("entry");
                    xl_item.SetAttributeValue("id", item.SeriesId);
                    xl_item.SetElementValue("name", item.Name);
                    xl_series.Add(xl_item);
                }

                xl.Add(xl_series);


                // Save to the file
                xl.Save(new StreamWriter(new FileStream(file.FullName, FileMode.Create)));
                return ExportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML.");
                Log.Debug(ex.Message);
                return ExportResult.Failed;
            }
        }
    }
}