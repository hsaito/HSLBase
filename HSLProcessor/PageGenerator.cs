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

        /// <summary>
        /// Export to static pages
        /// </summary>
        /// <param name="directory">Directory to export to</param>
        /// <returns>Result of the export</returns>
        public static GenerateResult Generate(DirectoryInfo directory)
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

                var songs_template_reader = new StreamReader(new FileStream(current_dir+@"/templates/songs.tpl", FileMode.Open));
                var songs_template = songs_template_reader.ReadToEnd();

                songs_template = songs_template.Replace("{{count}}", song_list.Count.ToString());
                var song_list_output = "<table id=\"content_table\"><tr class=\"row\"><th class=\"cell table_head\">Title</th><th class=\"cell table_head\">Artist</th><th class=\"cell table_head\">Source</th></tr>\r\n{{content}}</table>";
                var song_list_content = "";

                foreach(var item in song_list)
                {
                    song_list_content += string.Format("<tr class=\"row\"><td class=\"cell\"><a href=\"title/{3}.html\">{0}</a></td><td class=\"cell\"><a href=\"artist/{4}.html\">{1}</a></td><td class=\"cell\"><a href=\"source/{5}.html\">{2}</a></td></tr>\r\n"
                    ,item.Title,item.Artist.Name,item.Source.Name, item.SongId, item.Artist.ArtistId, item.Source.SourceId);
                }

                song_list_output = song_list_output.Replace("{{content}}",song_list_content);

                songs_template = songs_template.Replace("{{content}}",song_list_output);

                var writer = new StreamWriter(new FileStream(directory.FullName+"/songs.html",FileMode.Create),Encoding.UTF8);
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