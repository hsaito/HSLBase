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
        public static Artist GetOrAddArtist(Artist artist)
        {
            HSLContext context = new HSLContext();
            artist.Id = Guid.NewGuid();
            var result = context.Artists.Where((item) => item.Name == artist.Name);
            if (result.Count() == 0)
            {
                context.Artists.Add(artist);
            }
            return artist;
        }

                /// <summary>
        /// Get artist, if doesn't exist, create new
        /// </summary>
        /// <param name="artist">Artist class</param>
        /// <returns>Result of the artist</returns>
        public static Source GetOrAddSource(Source source)
        {
            HSLContext context = new HSLContext();
            source.Id = Guid.NewGuid();
            var result = context.Sources.Where((item) => item.Name == source.Name);
            if (result.Count() == 0)
            {
                context.Sources.Add(source);
            }
            return source;
        }
    }
}