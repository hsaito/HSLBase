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
                return context.Artists.Find(result.First().Id);
            }
        }

                /// <summary>
        /// Get artist, if doesn't exist, create new
        /// </summary>
        /// <param name="artist">Artist class</param>
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
                return context.Sources.Find(result.First().Id);
            }

        }
    }
}