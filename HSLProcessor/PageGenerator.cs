using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class PageGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PageGenerator));
        public enum GenerateResult { Success, Failed }

        /// <summary>
        ///  Generate HTML pages based on DB entries
        /// </summary>
        /// <param name="directory">Directory (root) to export files</param>
        /// <returns>Result of the process</returns>
        public static GenerateResult Generate(DirectoryInfo directory)
        {
            try
            {
                Log.Info("Generate top page...");
                GenerateListing(directory);
                Log.Info("Generating title details page...");
                GenerateTitleDetail(new DirectoryInfo(directory.FullName + "/title"));
                Log.Info("Generating artist details page...");
                GenerateArtistDetail(new DirectoryInfo(directory.FullName + "/artist"));
                Log.Info("Generating source details page...");
                GenerateSourceDetail(new DirectoryInfo(directory.FullName + "/source"));
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
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        public static GenerateResult GenerateTitleDetail(DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                HSLContext context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var song_list = new List<Song>();
                foreach (var item in context.Songs)
                {
                    song_list.Add(item);
                }

                var current_dir = System.IO.Path.GetDirectoryName(
                  System.Reflection.Assembly.GetEntryAssembly().Location);

                var title_template_reader = new StreamReader(new FileStream(current_dir + @"/templates/title.tpl", FileMode.Open));
                var title_template = title_template_reader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacement_code = Guid.NewGuid();
                title_template = Regex.Replace(title_template,"{{(.*?)}}","{{$1-"+replacement_code.ToString()+"}}");

                foreach (var item in song_list)
                {
                    var item_template = title_template;
                    var title_detail = string.Format("<ul>\r\n<li>Title: {0}</li>\r\n", item.Title);
                    var artist_detail = "";
                    if (item.Artist.Name != "")
                        artist_detail = string.Format("<li>Artist: <a href=\"../artist/{1}.html\">{0}</a></li>\r\n", item.Artist.Name, item.Artist.ArtistId);
                    var source_detail = "";
                    if (item.Source.Name != "")
                        source_detail = string.Format("<li>Source: <a href=\"../source/{1}.html\">{0}</a></li>\r\n", item.Source.Name, item.Source.SourceId);

                    title_detail += artist_detail + source_detail + "</ul>";

                    item_template = item_template.Replace("{{title-"+replacement_code.ToString()+"}}", item.Title);
                    item_template = item_template.Replace("{{content-"+replacement_code.ToString()+"}}", title_detail);
                    item_template = item_template.Replace("{{title_urlencoded-"+replacement_code.ToString()+"}}", WebUtility.HtmlEncode(item.Title));
                    item_template = item_template.Replace("{{generated-"+replacement_code.ToString()+"}}", DateTime.UtcNow.ToString("u"));

                    var writer = new StreamWriter(new FileStream(directory.FullName + "/" + item.TitleId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(item_template);
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
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        public static GenerateResult GenerateArtistDetail(DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                HSLContext context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var artist_list = new List<Artist>();
                foreach (var item in context.Artists)
                {
                    artist_list.Add(item);
                }

                var current_dir = System.IO.Path.GetDirectoryName(
                  System.Reflection.Assembly.GetEntryAssembly().Location);

                var artist_template_reader = new StreamReader(new FileStream(current_dir + @"/templates/artist.tpl", FileMode.Open));
                var artist_template = artist_template_reader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacement_code = Guid.NewGuid();
                artist_template = Regex.Replace(artist_template,"{{(.*?)}}","{{$1-"+replacement_code.ToString()+"}}");

                foreach (var item in artist_list)
                {
                    var item_template = artist_template;

                    item_template = item_template.Replace("{{artist-"+replacement_code.ToString()+"}}", item.Name);

                    var (_, list) = Searcher.Search(item.ArtistId, Searcher.SearchType.Artist);

                    // Sort the list
                    list = list.OrderBy(o => o.Title).ToList();

                    var song_list_content = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Source</th></tr>\r\n";
                    foreach (var title_item in list)
                    {
                        song_list_content += string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"../title/{2}.html\">{0}</a></td></td><td class=\"cell\"><a href=\"../source/{3}.html\">{1}</a></td></tr>\r\n"
                        , title_item.Title, title_item.Source.Name, title_item.TitleId, title_item.Source.SourceId);
                    }
                    song_list_content += "</table>";

                    item_template = item_template.Replace("{{content-"+replacement_code.ToString()+"}}", song_list_content);
                    item_template = item_template.Replace("{{artist_urlencoded-"+replacement_code.ToString()+"}}", WebUtility.HtmlEncode(item.Name));
                    item_template = item_template.Replace("{{generated-"+replacement_code.ToString()+"}}", DateTime.UtcNow.ToString("u"));

                    var writer = new StreamWriter(new FileStream(directory.FullName + "/" + item.ArtistId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(item_template);
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
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        public static GenerateResult GenerateSourceDetail(DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                HSLContext context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var source_list = new List<Source>();
                foreach (var item in context.Sources)
                {
                    source_list.Add(item);
                }

                var current_dir = System.IO.Path.GetDirectoryName(
                  System.Reflection.Assembly.GetEntryAssembly().Location);

                var source_template_reader = new StreamReader(new FileStream(current_dir + @"/templates/source.tpl", FileMode.Open));
                var source_template = source_template_reader.ReadToEnd();

                // Replacement code to prevent "over-replacement"
                var replacement_code = Guid.NewGuid();
                source_template = Regex.Replace(source_template,"{{(.*?)}}","{{$1-"+replacement_code.ToString()+"}}");

                foreach (var item in source_list)
                {
                    var item_template = source_template;

                    item_template = item_template.Replace("{{source-"+replacement_code.ToString()+"}}", item.Name);

                    var (_, list) = Searcher.Search(item.SourceId, Searcher.SearchType.Source);

                    // Sort the list
                    list = list.OrderBy(o => o.Title).ToList();

                    var song_list_content = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th></tr>\r\n";
                    foreach (var title_item in list)
                    {
                        song_list_content += string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"../title/{2}.html\">{0}</a></td></td><td class=\"cell\"><a href=\"../artist/{3}.html\">{1}</a></td></tr>\r\n"
                        , title_item.Title, title_item.Artist.Name, title_item.TitleId, title_item.Artist.ArtistId);
                    }
                    song_list_content += "</table>";

                    item_template = item_template.Replace("{{content-"+replacement_code.ToString()+"}}", song_list_content);
                    item_template = item_template.Replace("{{source_urlencoded-"+replacement_code.ToString()+"}}", WebUtility.HtmlEncode(item.Name));
                    item_template = item_template.Replace("{{generated-"+replacement_code.ToString()+"}}", DateTime.UtcNow.ToString("u"));

                    var writer = new StreamWriter(new FileStream(directory.FullName + "/" + item.SourceId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(item_template);
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
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        public static GenerateResult GenerateListing(DirectoryInfo directory)
        {
            try
            {
                if (!directory.Exists)
                    directory.Create();

                HSLContext context = new HSLContext();

                context.LoadRelations();

                // Create a song list
                var song_list = new List<Song>();
                foreach (var item in context.Songs)
                {
                    song_list.Add(item);
                }

                // Sort the list
                song_list = song_list.OrderBy(o => o.Title).ToList();

                var current_dir = System.IO.Path.GetDirectoryName(
                  System.Reflection.Assembly.GetEntryAssembly().Location);

                var songs_template_reader = new StreamReader(new FileStream(current_dir + @"/templates/songs.tpl", FileMode.Open));
                var songs_template = songs_template_reader.ReadToEnd();

                songs_template = songs_template.Replace("{{title_count}}", context.Songs.Count().ToString());
                songs_template = songs_template.Replace("{{artist_count}}", context.Artists.Count().ToString());
                songs_template = songs_template.Replace("{{source_count}}", context.Sources.Count().ToString());

                var song_list_output = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th><th class=\"cell table_head\">Source</th></tr>\r\n{{content}}</table>";
                var song_list_content = "";

                foreach (var item in song_list)
                {
                    song_list_content += string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"title/{3}.html\">{0}</a></td><td class=\"cell\"><a href=\"artist/{4}.html\">{1}</a></td><td class=\"cell\"><a href=\"source/{5}.html\">{2}</a></td></tr>\r\n"
                    , item.Title, item.Artist.Name, item.Source.Name, item.TitleId, item.Artist.ArtistId, item.Source.SourceId);
                }

                song_list_output = song_list_output.Replace("{{content}}", song_list_content);

                songs_template = songs_template.Replace("{{content}}", song_list_output);

                songs_template = songs_template.Replace("{{generated}}", DateTime.UtcNow.ToString("u"));

                var writer = new StreamWriter(new FileStream(directory.FullName + "/songs.html", FileMode.Create), Encoding.UTF8);
                writer.Write(songs_template);

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
    }
}