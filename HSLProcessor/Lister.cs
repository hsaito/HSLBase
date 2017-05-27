using System;
using System.Collections.Generic;

namespace HSLProcessor
{
    static class Lister
    {
        /// <summary>
        /// List DB items
        /// </summary>
        public static void List()
        {
            HSLContext context = new HSLContext();

            // For counting
            int i = 0;

            // Display each item
            foreach (var item in context.Songs)
            {
                Console.WriteLine(
                    string.Format("Id: {0} Title: {1} Artist: {2} Source: {3}",
                    item.SongId, item.Title, item.Artist, item.Source));
                i++;
            }

            // Display count
            Console.WriteLine(string.Format("Count: {0}", i));
        }

        /// <summary>
        /// List songs from the list of Song class
        /// </summary>
        /// <param name="songs">List of songs</param>
        public static void List(List<Song> songs)
        {
            int i = 0;
            foreach (var item in songs)
            {
                Console.WriteLine(
                    string.Format("Id: {0} Title: {1} Artist: {2} Source: {3}",
                    item.SongId, item.Title, item.Artist, item.Source));
                i++;
            }
            Console.WriteLine(string.Format("Count: {0}", i));
        }
    }
}