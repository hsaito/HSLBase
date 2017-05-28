using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class PageGenerator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PageGenerator));
        public enum GenerateResult { Success, Failed }


        public static GenerateResult Generate(DirectoryInfo directory)
        {
            try
            {
                GenerateListing(directory);
                GenerateTitleDetail(new DirectoryInfo(directory.FullName + "/title"));
                GenerateArtistDetail(new DirectoryInfo(directory.FullName + "/artist"));
                GenerateSourceDetail(new DirectoryInfo(directory.FullName + "/source"));
                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML.");
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

                foreach (var item in song_list)
                {
                    var item_template = title_template;
                    var title_detail = string.Format("<li>Title: {0}</li>\r\n", item.Title);
                    var artist_detail = "";
                    if (item.Artist.Name != "")
                        artist_detail = string.Format("<li>Artist: {0}</li>\r\n", item.Artist.Name);
                    var source_detail = "";
                    if (item.Source.Name != "")
                        source_detail = string.Format("<li>Source: {0}", item.Source.Name);

                    title_detail += artist_detail + source_detail;

                    item_template = item_template.Replace("{{title}}", item.Title);
                    item_template = item_template.Replace("{{content}}", title_detail);

                    var writer = new StreamWriter(new FileStream(directory.FullName + "/" + item.SongId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(item_template);
                    writer.Flush();
                }

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML.");
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

                foreach (var item in artist_list)
                {
                    var item_template = artist_template;

                    item_template = item_template.Replace("{{artist}}", item.Name);

                    var (_, list) = Searcher.Search(item.ArtistId, Searcher.SearchType.Artist);

                    var song_list_content = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Source</th></tr>\r\n";
                    foreach (var title_item in list)
                    {
                        song_list_content += string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"../title/{2}.html\">{0}</a></td></td><td class=\"cell\"><a href=\"../source/{3}.html\">{1}</a></td></tr>\r\n"
                        , title_item.Title, title_item.Source.Name, title_item.SongId, title_item.Source.SourceId);
                    }
                    song_list_content += "</table>";

                    item_template = item_template.Replace("{{content}}",song_list_content);
                    
                    var writer = new StreamWriter(new FileStream(directory.FullName + "/" + item.ArtistId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(item_template);
                    writer.Flush();
                }

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML.");
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

                foreach (var item in source_list)
                {
                    var item_template = source_template;

                    item_template = item_template.Replace("{{source}}", item.Name);

                    var (_, list) = Searcher.Search(item.SourceId, Searcher.SearchType.Source);

                    var song_list_content = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th></tr>\r\n";
                    foreach (var title_item in list)
                    {
                        song_list_content += string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"../title/{2}.html\">{0}</a></td></td><td class=\"cell\"><a href=\"../artist/{3}.html\">{1}</a></td></tr>\r\n"
                        , title_item.Title, title_item.Artist.Name, title_item.SongId, title_item.Artist.ArtistId);
                    }
                    song_list_content += "</table>";

                    item_template = item_template.Replace("{{content}}",song_list_content);
                    
                    var writer = new StreamWriter(new FileStream(directory.FullName + "/" + item.SourceId + ".html", FileMode.Create), Encoding.UTF8);
                    writer.Write(item_template);
                    writer.Flush();
                }

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML.");
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

                var current_dir = System.IO.Path.GetDirectoryName(
                  System.Reflection.Assembly.GetEntryAssembly().Location);

                var songs_template_reader = new StreamReader(new FileStream(current_dir + @"/templates/songs.tpl", FileMode.Open));
                var songs_template = songs_template_reader.ReadToEnd();

                songs_template = songs_template.Replace("{{count}}", song_list.Count.ToString());
                var song_list_output = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th><th class=\"cell table_head\">Source</th></tr>\r\n{{content}}</table>";
                var song_list_content = "";

                foreach (var item in song_list)
                {
                    song_list_content += string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"title/{3}.html\">{0}</a></td><td class=\"cell\"><a href=\"artist/{4}.html\">{1}</a></td><td class=\"cell\"><a href=\"source/{5}.html\">{2}</a></td></tr>\r\n"
                    , item.Title, item.Artist.Name, item.Source.Name, item.SongId, item.Artist.ArtistId, item.Source.SourceId);
                }

                song_list_output = song_list_output.Replace("{{content}}", song_list_content);

                songs_template = songs_template.Replace("{{content}}", song_list_output);

                var writer = new StreamWriter(new FileStream(directory.FullName + "/songs.html", FileMode.Create), Encoding.UTF8);
                writer.Write(songs_template);

                writer.Flush();

                return GenerateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML.");
                Log.Debug(ex.Message);
                return GenerateResult.Failed;
            }
        }
    }
}