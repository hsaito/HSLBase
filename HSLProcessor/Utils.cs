using System;
using System.Linq;

namespace HSLProcessor
{
    internal static class Utils
    {
        /// <summary>
        ///     Get artist, if doesn't exist, create new
        /// </summary>
        /// <param name="artist">Artist class</param>
        /// <param name="context">Reference to the database context</param>
        /// <returns>Result of the artist</returns>
        public static Artist GetOrAddArtist(Artist artist, ref HSLContext context)
        {
            var result = context.Artists.Where(item => item.Name == artist.Name);
            if (result.Any()) return context.Artists.Find(result.First().ArtistId);
            context.Artists.Add(artist);
            context.SaveChanges();
            return artist;
        }

        /// <summary>
        ///     Get source, if doesn't exist, create new
        /// </summary>
        /// <param name="source">Source class</param>
        /// <param name="context">Reference to the database context</param>
        /// <returns>Result of the source</returns>
        public static Source GetOrAddSource(Source source, ref HSLContext context)
        {
            var result = context.Sources.Where(item => item.Name == source.Name);
            if (result.Any()) return context.Sources.Find(result.First().SourceId);
            context.Sources.Add(source);
            context.SaveChanges();
            return source;
        }

        /// <summary>
        ///     Get series, if doesn't exist, create new
        /// </summary>
        /// <param name="series">Series class</param>
        /// <param name="context">Reference to the database context</param>
        /// <returns>Result of the artist</returns>
        public static Series GetOrAddSeries(Series series, ref HSLContext context)
        {
            var result = context.Series.Where(item => item.Name == series.Name);
            if (result.Any()) return context.Series.Find(result.First().SeriesId);
            context.Series.Add(series);
            context.SaveChanges();
            return series;
        }

        /// <summary>
        ///     Get Guid prefix
        /// </summary>
        /// <param name="guid">Guid</param>
        /// <returns>Prefix (first three characters) of Guid in string</returns>
        public static string GetGuidPrefix(Guid guid)
        {
            return guid.ToString().Substring(0, 3);
        }
        
        /// <summary>
        /// Convert Integer to Guid
        /// </summary>
        /// <param name="value">Integer to convert</param>
        /// <returns>Guid of the value</returns>
        public static Guid Int2Guid(int value)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        /// <summary>
        /// Convert Guid to Integer
        /// </summary>
        /// <param name="value">Guid to convert</param>
        /// <returns>Integer of the value</returns>
        public static int Guid2Int(Guid value)
        {
            var b = value.ToByteArray();
            var bit = BitConverter.ToInt32(b, 0);
            return bit;
        }
        
    }
}