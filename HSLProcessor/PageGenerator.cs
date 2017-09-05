using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using log4net;
// ReSharper disable UnusedMethodReturnValue.Local
// ReSharper disable UnusedMethodReturnValue.Global

namespace HSLProcessor
{
    internal static class PageGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PageGenerator));
        public enum GenerateResult { Success, Failed }

        /// <summary>
        ///  Generate HTML pages based on DB entries
        /// </summary>
        /// <param name="template">Location of the template</param>
        /// <param name="directory">Directory (root) to export files</param>
        /// <returns>Result of the process</returns>
        public static GenerateResult Generate(DirectoryInfo template, DirectoryInfo directory)
        {
            try
            {
                Log.Info("Generate top page...");
                GenerateListing(template, directory);
                Log.Info("Generating title details page...");
                GenerateTitleDetail(template, new DirectoryInfo(directory.FullName + "/title"));
                Log.Info("Generating artist details page...");
                GenerateArtistDetail(template, new DirectoryInfo(directory.FullName + "/artist"));
                Log.Info("Generating source details page...");
                GenerateSourceDetail(template, new DirectoryInfo(directory.FullName + "/source"));
                Log.Info("Generation completed.");
                GenerateSeriesDetail(template, new DirectoryInfo(directory.FullName + "/series"));
                Log.Info("Generation completed.");
                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("HTML generation failed.");
                Log.Debug(ex.Message);
                return GenerateResult.Failed;
            }
        }

        /// <summary>
        /// Generate title details listing
        /// </summary>
        /// <param name="template">Location of the template</param>
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        private static GenerateResult GenerateTitleDetail(DirectoryInfo template, DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                var context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var songList = context.Songs.ToList();

                var titleTemplateReader = new StreamReader(new FileStream(template + @"/title.tpl", FileMode.Open));
                var titleTemplate = titleTemplateReader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacementCode = Guid.NewGuid();
                titleTemplate = Regex.Replace(titleTemplate, "{{(.*?)}}", "{{$1-" + replacementCode + "}}");

                foreach (var item in songList)
                {
                    var itemTemplate = titleTemplate;
                    var titleDetail = string.Format("<ul>\r\n<li>Title: {0}</li>\r\n", item.Title);
                    var artistDetail = "";
                    if (item.Artist.Name != "")
                        artistDetail = string.Format("<li>Artist: <a href=\"../../artist/{2}/{1}.html\">{0}</a></li>\r\n", item.Artist.Name, item.Artist.ArtistId, Utils.GetGuidPrefix(item.Artist.ArtistId));
                    var sourceDetail = "";
                    if (item.Source.Name != "")
                        sourceDetail = string.Format("<li>Source: <a href=\"../../source/{2}/{1}.html\">{0}</a></li>\r\n", item.Source.Name, item.Source.SourceId, Utils.GetGuidPrefix(item.Source.SourceId));

                    titleDetail += artistDetail + sourceDetail + "</ul>";

                    itemTemplate = itemTemplate.Replace("{{title-" + replacementCode + "}}", item.Title);
                    itemTemplate = itemTemplate.Replace("{{content-" + replacementCode + "}}", titleDetail);
                    itemTemplate = itemTemplate.Replace("{{title_urlencoded-" + replacementCode + "}}", WebUtility.HtmlEncode(item.Title));
                    itemTemplate = itemTemplate.Replace("{{generated-" + replacementCode + "}}", DateTime.UtcNow.ToString("u"));

                    var subbase = new DirectoryInfo(directory.FullName + "/" + Utils.GetGuidPrefix(item.TitleId));
                    if (!subbase.Exists)
                        subbase.Create();

                    var writer = new StreamWriter(new FileStream(subbase.FullName + "/" + item.TitleId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(itemTemplate);
                    writer.Flush();
                }

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed generating title detail.");
                Log.Debug(ex.Message);
                return GenerateResult.Failed;
            }
        }

        /// <summary>
        /// Generate artist details listing
        /// </summary>
        /// <param name="template">Location of the template</param>
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        private static GenerateResult GenerateArtistDetail(DirectoryInfo template, DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                var context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var artistList = context.Artists.ToList();

                var artistTemplateReader = new StreamReader(new FileStream(template + @"/artist.tpl", FileMode.Open));
                var artistTemplate = artistTemplateReader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacementCode = Guid.NewGuid();
                artistTemplate = Regex.Replace(artistTemplate, "{{(.*?)}}", "{{$1-" + replacementCode + "}}");

                foreach (var item in artistList)
                {
                    var itemTemplate = artistTemplate;

                    itemTemplate = itemTemplate.Replace("{{artist-" + replacementCode + "}}", item.Name);

                    var (_, list) = Searcher.Search(item.ArtistId, Searcher.SearchType.Artist);

                    // Sort the list
                    list = list.OrderBy(o => o.Title).ToList();

                    var songListContent = list.Aggregate("<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Source</th></tr>\r\n", (current, titleItem) => current + string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"../../title/{4}/{2}.html\">{0}</a></td><td class=\"cell\"><a href=\"../../source/{5}/{3}.html\">{1}</a></td></tr>\r\n", titleItem.Title, titleItem.Source.Name, titleItem.TitleId, titleItem.Source.SourceId, Utils.GetGuidPrefix(titleItem.TitleId), Utils.GetGuidPrefix(titleItem.Source.SourceId)));
                    songListContent += "</table>";

                    itemTemplate = itemTemplate.Replace("{{content-" + replacementCode + "}}", songListContent);
                    itemTemplate = itemTemplate.Replace("{{artist_urlencoded-" + replacementCode + "}}", WebUtility.HtmlEncode(item.Name));
                    itemTemplate = itemTemplate.Replace("{{generated-" + replacementCode + "}}", DateTime.UtcNow.ToString("u"));

                    var subbase = new DirectoryInfo(directory.FullName + "/" + Utils.GetGuidPrefix(item.ArtistId));
                    if (!subbase.Exists)
                        subbase.Create();

                    var writer = new StreamWriter(new FileStream(subbase.FullName + "/" + item.ArtistId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(itemTemplate);
                    writer.Flush();
                }

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed generating artist detail.");
                Log.Debug(ex.Message);
                return GenerateResult.Failed;
            }
        }

        /// <summary>
        /// Generate source details listing
        /// </summary>
        /// <param name="template">Location of the template</param>
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        private static GenerateResult GenerateSourceDetail(DirectoryInfo template, DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                var context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var sourceList = context.Sources.ToList();

                var sourceTemplateReader = new StreamReader(new FileStream(template + @"/source.tpl", FileMode.Open));
                var sourceTemplate = sourceTemplateReader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacementCode = Guid.NewGuid();
                sourceTemplate = Regex.Replace(sourceTemplate, "{{(.*?)}}", "{{$1-" + replacementCode + "}}");

                foreach (var item in sourceList)
                {
                    var itemTemplate = sourceTemplate;

                    itemTemplate = itemTemplate.Replace("{{source-" + replacementCode + "}}", item.Name);

                    if (item.Series != null)
                    {
                        if (item.SeriesId != null)
                        {
                            var seriesLink = string.Format("<a href=\"../../series/{2}/{0}.html\">{1}</a>", item.SeriesId, item.Series
                                .Name, Utils.GetGuidPrefix((Guid)item.SeriesId));
                            itemTemplate = itemTemplate.Replace("{{series-" + replacementCode + "}}", seriesLink);
                        }
                    }

                    var (_, list) = Searcher.Search(item.SourceId, Searcher.SearchType.Source);

                    // Sort the list
                    list = list.OrderBy(o => o.Title).ToList();

                    var songListContent = list.Aggregate("<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th></tr>\r\n", (current, titleItem) => current + string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"../../title/{4}/{2}.html\">{0}</a></td><td class=\"cell\"><a href=\"../../artist/{5}/{3}.html\">{1}</a></td></tr>\r\n", titleItem.Title, titleItem.Artist.Name, titleItem.TitleId, titleItem.Artist.ArtistId, Utils.GetGuidPrefix(titleItem.TitleId), Utils.GetGuidPrefix(titleItem.Artist.ArtistId)));
                    songListContent += "</table>";

                    itemTemplate = itemTemplate.Replace("{{content-" + replacementCode + "}}", songListContent);
                    itemTemplate = itemTemplate.Replace("{{source_urlencoded-" + replacementCode + "}}", WebUtility.HtmlEncode(item.Name));
                    itemTemplate = itemTemplate.Replace("{{generated-" + replacementCode + "}}", DateTime.UtcNow.ToString("u"));

                    var subbase = new DirectoryInfo(directory.FullName + "/" + Utils.GetGuidPrefix(item.SourceId));
                    if (!subbase.Exists)
                        subbase.Create();

                    var writer = new StreamWriter(new FileStream(subbase.FullName + "/" + item.SourceId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(itemTemplate);
                    writer.Flush();
                }

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed generating source detail.");
                Log.Debug(ex.Message);
                return GenerateResult.Failed;
            }
        }

        /// <summary>
        /// Generate series details listing
        /// </summary>
        /// <param name="template">Location of the template</param>
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        private static GenerateResult GenerateSeriesDetail(DirectoryInfo template, DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                var context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var seriesList = context.Series.ToList();

                var sourceTemplateReader = new StreamReader(new FileStream(template + @"/series.tpl", FileMode.Open));
                var sourceTemplate = sourceTemplateReader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacementCode = Guid.NewGuid();
                sourceTemplate = Regex.Replace(sourceTemplate, "{{(.*?)}}", "{{$1-" + replacementCode + "}}");

                foreach (var item in seriesList)
                {
                    var itemTemplate = sourceTemplate;

                    itemTemplate = itemTemplate.Replace("{{series-" + replacementCode + "}}", item.Name);

                    var (_, list) = Searcher.Search(item.SeriesId, Searcher.SearchType.Series);

                    // Sort the list
                    list = list.OrderBy(o => o.Title).ToList();

                    var songListContent = list.Aggregate("<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th><th class=\"cell table_head\">Source</th></tr>\r\n", (current, titleItem) => current + string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"../../title/{6}/{2}.html\">{0}</a></td><td class=\"cell\"><a href=\"../../artist/{7}/{3}.html\">{1}</a></td><td class=\"cell\"><a href=\"../../source/{8}/{5}.html\">{4}</a></td></tr>\r\n", titleItem.Title, titleItem.Artist.Name, titleItem.TitleId, titleItem.Artist.ArtistId, titleItem.Source.Name, titleItem.SourceId, Utils.GetGuidPrefix(titleItem.TitleId), Utils.GetGuidPrefix(titleItem.Artist.ArtistId), Utils.GetGuidPrefix(titleItem.Source.SourceId)));
                    songListContent += "</table>";

                    itemTemplate = itemTemplate.Replace("{{content-" + replacementCode + "}}", songListContent);
                    itemTemplate = itemTemplate.Replace("{{series_urlencoded-" + replacementCode + "}}", WebUtility.HtmlEncode(item.Name));
                    itemTemplate = itemTemplate.Replace("{{generated-" + replacementCode + "}}", DateTime.UtcNow.ToString("u"));

                    var subbase = new DirectoryInfo(directory.FullName + "/" + Utils.GetGuidPrefix(item.SeriesId));
                    if (!subbase.Exists)
                        subbase.Create();

                    var writer = new StreamWriter(new FileStream(subbase + "/" + item.SeriesId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(itemTemplate);
                    writer.Flush();
                }

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed generating source detail.");
                Log.Debug(ex.Message);
                return GenerateResult.Failed;
            }
        }


        /// <summary>
        /// Generate title main listing
        /// </summary>
        /// <param name="template">Location of the template</param>
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        private static GenerateResult GenerateListing(DirectoryInfo template, DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                var context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var songList = context.Songs.ToList();

                // Sort the list
                songList = songList.OrderBy(o => o.Title).ToList();

                var songsTemplateReader = new StreamReader(new FileStream(template + @"/songs.tpl", FileMode.Open));
                var songsTemplate = songsTemplateReader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacementCode = Guid.NewGuid();
                songsTemplate = Regex.Replace(songsTemplate, "{{(.*?)}}", "{{$1-" + replacementCode + "}}");

                songsTemplate = songsTemplate.Replace("{{title_count-" + replacementCode + "}}", context.Songs.Count().ToString());
                songsTemplate = songsTemplate.Replace("{{artist_count-" + replacementCode + "}}", context.Artists.Count().ToString());
                songsTemplate = songsTemplate.Replace("{{source_count-" + replacementCode + "}}", context.Sources.Count().ToString());
                songsTemplate = songsTemplate.Replace("{{series_count-" + replacementCode + "}}", context.Series.Count().ToString());

                var songListOutput = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th><th class=\"cell table_head\">Source</th></tr>\r\n{{content-" + replacementCode + "}}</table>";
                var songListContent = songList.Aggregate("", (current, item) => current + string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"title/{6}/{3}.html\">{0}</a></td><td class=\"cell\"><a href=\"artist/{7}/{4}.html\">{1}</a></td><td class=\"cell\"><a href=\"source/{8}/{5}.html\">{2}</a></td></tr>\r\n", item.Title, item.Artist.Name, item.Source.Name, item.TitleId, item.Artist.ArtistId, item.Source.SourceId, Utils.GetGuidPrefix(item.TitleId), Utils.GetGuidPrefix(item.ArtistId), Utils.GetGuidPrefix(item.SourceId)));

                songListOutput = songListOutput.Replace("{{content-" + replacementCode + "}}", songListContent);

                songsTemplate = songsTemplate.Replace("{{content-" + replacementCode + "}}", songListOutput);

                songsTemplate = songsTemplate.Replace("{{generated-" + replacementCode + "}}", DateTime.UtcNow.ToString("u"));

                var writer = new StreamWriter(new FileStream(directory.FullName + "/songs.html", FileMode.Create), Encoding.UTF8);
                writer.Write(songsTemplate);

                writer.Flush();

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed generating top listing.");
                Log.Debug(ex.Message);
                return GenerateResult.Failed;
            }
        }

        /// <summary>
        /// Export sitemap
        /// </summary>
        /// <param name="file">XML file to export to</param>
        /// <param name="urlbase">Base of the URL to append to the sitemap</param>
        /// <returns>Result of the export</returns>
        public static GenerateResult ExportSitemap(FileInfo file, string urlbase)
        {
            try
            {
                var doc = new XDocument(new XDeclaration("1.0", "UTF-8", "no"));
                var ns = XNamespace.Get(@"http://www.sitemaps.org/schemas/sitemap/0.9");
                var xl = new XElement(ns+"urlset");
                xl.Add(new XAttribute("xmlns",ns.NamespaceName));

                if (urlbase[urlbase.Length - 1] != '/')
                {
                    urlbase = urlbase + "/";
                }

                var context = new HSLContext();

                // Songs
                foreach (var item in context.Songs)
                {
                    var songitem = new XElement(ns+"url");
                    songitem.Add(new XElement(ns+"loc", urlbase + "title/" + Utils.GetGuidPrefix(item.TitleId) + "/" + item.TitleId + ".html"));
                    xl.Add(songitem);
                }

                // Artists
                foreach (var item in context.Artists)
                {

                    var songitem = new XElement(ns+"url");
                    songitem.Add(new XElement(ns+"loc", urlbase + "artist/" + Utils.GetGuidPrefix(item.ArtistId) + "/" + item.ArtistId + ".html"));
                    xl.Add(songitem);
                }

                // Sources
                foreach (var item in context.Sources)
                {

                    var songitem = new XElement(ns+"url");
                    songitem.Add(new XElement(ns+"loc", urlbase + "source/" + Utils.GetGuidPrefix(item.SourceId) + "/" + item.SourceId + ".html"));
                    xl.Add(songitem);
                }

                // Series
                foreach (var item in context.Series)
                {

                    var songitem = new XElement(ns+"url");
                    songitem.Add(new XElement(ns+"loc", urlbase + "series/" + Utils.GetGuidPrefix(item.SeriesId) + "/" + item.SeriesId + ".html"));
                    xl.Add(songitem);
                }

                doc.Add(xl);

                var writer = new FileStream(file.FullName, FileMode.Create);
                xl.Save(writer);
                writer.Flush();

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed generating the sitemap.");
                Log.Error(ex.Message);
                return GenerateResult.Failed;
            }
        }
    }
}