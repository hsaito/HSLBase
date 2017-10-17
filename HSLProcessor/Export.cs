using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using Google.Protobuf;
using log4net;
using Newtonsoft.Json;

// ReSharper disable UnusedMethodReturnValue.Global

namespace HSLProcessor
{
    internal static class Exporter
    {
        public enum ExportResult
        {
            Success,
            Failed
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(Exporter));

        /// <summary>
        ///     Export to JSON
        /// </summary>
        /// <param name="file">JSON file to export to</param>
        /// <returns>Result of the export</returns>
        public static ExportResult ExportJson(FileInfo file)
        {
            try
            {
                var entry = new List<SongEntry>();
                var context = new HSLContext();
                context.LoadRelations();

                foreach (var item in context.Songs)
                {
                    var song = new SongEntry
                    {
                        Title = item.Title,
                        TitleId = item.TitleId.ToString(),
                        Artist = item.Artist.Name,
                        ArtistId = item.Artist.ArtistId.ToString()
                    };


                    if (item.Source != null)
                    {
                        song.Source = item.Source.Name;
                        song.SourceId = item.Source.SourceId.ToString();

                        if (item.Source.Series != null)
                        {
                            song.Series = item.Source.Series.Name;
                            song.SeriesId = item.Source.Series.SeriesId.ToString();
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
        ///     Export to XML
        /// </summary>
        /// <param name="file">XML file to export to</param>
        /// <returns>Result of the export</returns>
        public static ExportResult ExportXml(FileInfo file)
        {
            try
            {
                var context = new HSLContext();

                context.LoadRelations();

                // Create a root element
                var xl = new XElement("hsl");
                var xlSongs = new XElement("songs");
                foreach (var item in context.Songs)
                {
                    var xlItem = new XElement("entry");
                    xlItem.SetAttributeValue("id", item.TitleId);
                    xlItem.SetElementValue("title", item.Title);

                    var xlArtist = new XElement("artist");
                    xlArtist.SetAttributeValue("id", item.Artist.ArtistId);
                    xlArtist.Value = item.Artist.Name;
                    xlItem.Add(xlArtist);

                    var xlSource = new XElement("source");
                    xlSource.SetAttributeValue("id", item.Source.SourceId);
                    xlSource.Value = item.Source.Name;
                    xlItem.Add(xlSource);
                    xlSongs.Add(xlItem);
                }
                xl.Add(xlSongs);

                var xlArtists = new XElement("artists");
                foreach (var item in context.Artists)
                {
                    var xlItem = new XElement("entry");
                    xlItem.SetAttributeValue("id", item.ArtistId);
                    xlItem.SetElementValue("name", item.Name);
                    xlArtists.Add(xlItem);
                }
                xl.Add(xlArtists);

                var xlSources = new XElement("sources");
                foreach (var item in context.Sources)
                {
                    var xlItem = new XElement("entry");
                    xlItem.SetAttributeValue("id", item.SourceId);
                    xlItem.SetElementValue("name", item.Name);

                    if (item.Series != null)
                    {
                        xlItem.SetElementValue("series", item.Series.Name);
                        xlItem.Element("series")?.SetAttributeValue("id", item.Series.SeriesId);
                    }

                    xlSources.Add(xlItem);
                }

                xl.Add(xlSources);

                var xlSeries = new XElement("series");
                foreach (var item in context.Series)
                {
                    var xlItem = new XElement("entry");
                    xlItem.SetAttributeValue("id", item.SeriesId);
                    xlItem.SetElementValue("name", item.Name);
                    xlSeries.Add(xlItem);
                }

                xl.Add(xlSeries);


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

        /// <summary>
        /// Export to Protocol Buffer
        /// </summary>
        /// <param name="file">File name to export to</param>
        /// <returns>Export result</returns>
        public static ExportResult ExportProtoBuffer(FileInfo file)
        {
            try
            {
                var context = new HSLContext();

                context.LoadRelations();

                var proto = new HSLProto.HSL();

                var i = 0;
                foreach (var item in context.Series)
                {
                    var series = new HSLProto.Series
                    {
                        SerialNumber = i,
                        Id = Utils.Guid2Int(item.SeriesId),
                        Name = item.Name
                    };
                    proto.Series.Add(series);
                    i++;
                }

                i = 0;

                foreach (var item in context.Sources)
                {
                    var artist = new HSLProto.Source
                    {
                        SerialNumber = i,
                        Id = Utils.Guid2Int(item.SourceId),
                        Name = item.Name,
                    };

                    
                    if (proto.Series.Any(q => q.Id == Utils.Guid2Int(item.SeriesId ?? Guid.Empty)))
                        artist.Series = proto.Series.First(q => q.Id == Utils.Guid2Int(item.SeriesId ?? Guid.Empty)).SerialNumber;
                    else
                    {
                        artist.Series = -1;
                    }
                    
                    proto.Sources.Add(artist);
                    i++;
                }

                i = 0;

                foreach (var item in context.Artists)
                {
                    var artist = new HSLProto.Artist
                    {
                        SerialNumber = i,
                        Id = Utils.Guid2Int(item.ArtistId),
                        Name = item.Name
                    };
                    proto.Artists.Add(artist);
                    i++;
                }

                i = 0;

                foreach (var item in context.Songs)
                {
                    var song = new HSLProto.Song
                    {
                        SerialNumber = i,
                        Id = Utils.Guid2Int(item.TitleId),
                        Name = item.Title
                    };
                    
                    if(proto.Artists.Any(q => q.Id == Utils.Guid2Int(item.ArtistId)))
                        song.Artist = proto.Artists.First(q => q.Id == Utils.Guid2Int(item.ArtistId)).SerialNumber;
                    else
                    {
                        song.Artist = -1;
                    }
                    if (proto.Sources.Any(q => q.Id == Utils.Guid2Int(item.SourceId)))
                        song.Source = proto.Sources.First(q => q.Id == Utils.Guid2Int(item.SourceId)).SerialNumber;
                    else
                    {
                        song.Source = -1;
                    }
                    proto.Songs.Add(song);
                    i++;
                }

                using (var output = File.Create(file.FullName))
                {
                    proto.WriteTo(output);
                }

                return ExportResult.Success;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                Log.Debug(e.StackTrace);
                return ExportResult.Failed;
            }
        }

        [SuppressMessage("ReSharper", "NotAccessedField.Local")]
        private struct SongEntry
        {
#pragma warning disable 414
            public string TitleId;
            public string Title;
            public string ArtistId;
            public string Artist;
            public string SourceId;
            public string Source;
            public string SeriesId;
            public string Series;
#pragma warning restore 414
        }
    }
}