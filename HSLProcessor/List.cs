using System;

namespace HSLProcessor
{
    static class List
    {
        public static void ListItems()
        {
            HSLContext context = new HSLContext();

            int i = 0;
            foreach (var item in context.Songs)
            {
                Console.WriteLine(
                    string.Format("Id: {0} Title: {1} Artist: {2} Source: {3}",
                    item.Id, item.Title, item.Artist, item.Reference));
                i++;
            }
            Console.WriteLine(string.Format("Count: {0}",i));
        }
    }
}