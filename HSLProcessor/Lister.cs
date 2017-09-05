using System;
using System.Collections.Generic;
using log4net;

namespace HSLProcessor
{
    internal static class Lister
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Lister));
        /// <summary>
        /// List DB items
        /// </summary>
        public static void List()
        {
            try
            {
                Log.Info("Listing contents of the database.");
                var context = new HSLContext();
                context.LoadRelations();

                // For counting
                var i = 0;

                // Display each item
                foreach (var item in context.Songs)
                {
                    Console.WriteLine(
                        string.Format("Id: {0} Title: {1} Artist: {2} Source: {3}",
                        item.TitleId, item.Title, item.Artist.Name, item.Source.Name));
                    i++;
                }

                // Display count
                Console.WriteLine(string.Format("Count: {0}", i));
                Log.Info("Listing completed");
            }
            catch (Exception ex)
            {
                Log.Error("Failed listing the content.");
                Log.Debug(ex.Message);
            }
        }

        /// <summary>
        /// List songs from the list of Song class
        /// </summary>
        /// <param name="songs">List of songs</param>
        public static void List(IEnumerable<Song> songs)
        {
            try
            {
                var i = 0;
                foreach (var item in songs)
                {
                    Console.WriteLine(
                        string.Format("Id: {0} Title: {1} Artist: {2} Source: {3}",
                        item.TitleId, item.Title, item.Artist.Name, item.Source.Name));
                    i++;
                }
                Console.WriteLine(string.Format("Count: {0}", i));
                Log.Info("Listing completed.");
            }
            catch (Exception ex)
            {
                Log.Error("Failed listing the content.");
                Log.Debug(ex.Message);
            }
        }
    }
}