using System;
using System.Linq;

namespace HSLProcessor
{
    class Utils
    {
        /// <summary>
        /// Get artist, if doesn't exist, create new
        /// </summary>
        /// <param name="artist">Artist class</param>
        /// <param name="context">Database context to use</param>
        /// <returns>Result of the artist</returns>
        public static Artist GetOrAddArtist(Artist artist, ref HSLContext context)
        {
            var result = context.Artists.Where((item) => item.Name == artist.Name);
            if (result.Count() == 0)
            {
                context.Artists.Add(artist);
                context.SaveChanges();
                return artist;
            }
            else
            {
                return context.Artists.Find(result.First().ArtistId);
            }
        }

                /// <summary>
        /// Get artist, if doesn't exist, create new
        /// </summary>
        /// <param name="source">Artist class</param>
        /// <param name="context">Database context to use</param>
        /// <returns>Result of the artist</returns>
        public static Source GetOrAddSource(Source source, ref HSLContext context)
        {
            var result = context.Sources.Where((item) => item.Name == source.Name);
            if (result.Count() == 0)
            {
                context.Sources.Add(source);
                context.SaveChanges();
                return source;
            }
            else
            {
                return context.Sources.Find(result.First().SourceId);
            }

        }

        /// <summary>
        /// Get artist, if doesn't exist, create new
        /// </summary>
        /// <param name="series">Series class</param>
        /// <param name="context">Database context to use</param>
        /// <returns>Result of the artist</returns>
        public static Series GetOrAddSeries(Series series, ref HSLContext context)
        {
            var result = context.Series.Where((item) => item.Name == series.Name);
            if (result.Count() == 0)
            {
                context.Series.Add(series);
                context.SaveChanges();
                return series;
            }
            else
            {
                return context.Series.Find(result.First().SeriesId);
            }
        }

        /// <summary>
        /// Get Guid prefix
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Prefix (first three characters) of Guid in string</returns>
        public static string GetGuidPrefix(Guid guid)
        {
            return guid.ToString().Substring(0,3);
        }


    }
}