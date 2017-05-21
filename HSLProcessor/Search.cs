using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class Searcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Searcher));
        public enum SearchResult { Success, Failed }
        public enum SearchType { Title, Artist, Source }

        /// <summary>
        /// Search the database
        /// </summary>
        /// <param name="query">Text to search</param>
        /// <param name="type">Search field</param>
        /// <returns>List of the result as Song class</returns>
        public static (SearchResult result, List<Song> hit) Search(string query, SearchType type)
        {
            var list = new List<Song>();
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                Log.Error("Failed searching the database");
                Log.Debug(ex.Message);
                return (SearchResult.Failed, list);
            }
        }
    }
}