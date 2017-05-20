using System;

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
                    item.Id, item.Title, item.Artist, item.Reference));
                i++;
            }

            // Display count
            Console.WriteLine(string.Format("Count: {0}",i));
        }
    }
}