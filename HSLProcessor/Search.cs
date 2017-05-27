using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
                HSLContext context = new HSLContext();
                context.LoadRelations();
                List<Song> result;
                switch (type)
                {
                    case SearchType.Title:
                        {
                            result = context.Songs.Where((item) => item.Title.Contains(query)).ToList();
                            break;
                        }

                    case SearchType.Artist:
                        {
                            result = null;
                            result = context.Songs.Where((item) => item.Artist.Name.Contains(query)).ToList();
                            break;
                        }

                    case SearchType.Source:
                        {
                            result = null;
                            result = context.Songs.Where((item) => item.Source.Name.Contains(query)).ToList();
                            break;
                        }

                    default:
                        {
                            throw new Exception("Invalid query type");
                        }
                }
                return (SearchResult.Success, result);
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