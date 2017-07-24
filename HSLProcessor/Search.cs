using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace HSLProcessor
{
    internal static class Searcher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Searcher));
        public enum SearchResult { Success, Failed }
        public enum SearchType { Title, Artist, Source, Series }

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
                Log.Info("Beginning search.");
                var context = new HSLContext();
                context.LoadRelations();
                List<Song> result;
                switch (type)
                {
                    case SearchType.Title:
                        {
                            Log.Info("Searching title for " + query);
                            result = context.Songs.Where((item) => item.Title.Contains(query)).ToList();
                            break;
                        }

                    case SearchType.Artist:
                        {
                            Log.Info("Searching artist for " + query);
                            result = null;
                            result = context.Songs.Where((item) => item.Artist.Name.Contains(query)).ToList();
                            break;
                        }

                    case SearchType.Source:
                        {
                            Log.Info("Searching source for " + query);
                            result = null;
                            result = context.Songs.Where((item) => item.Source.Name.Contains(query)).ToList();
                            break;
                        }

                    case SearchType.Series:
                        {
                            Log.Info("Searching series for " + query);
                            result = null;

                            var affTitleList = context.Sources.Where((item) => item.Series.Name == query).ToList();

                            result = new List<Song>();

                            foreach (var entry in affTitleList)
                            {
                                var sourceList = context.Songs.Where((item) => item.Source.Name.Contains(entry.Name));
                                result.AddRange(sourceList);
                            }

                            break;
                        }

                    default:
                        {
                            throw new Exception("Unsupported search type.");
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

        /// <summary>
        /// Search the database
        /// </summary>
        /// <param name="query">Guid to search</param>
        /// <param name="type">Search field</param>
        /// <returns>List of the result as Song class</returns>
        public static (SearchResult result, List<Song> hit) Search(Guid query, SearchType type)
        {
            var list = new List<Song>();
            try
            {
                var context = new HSLContext();
                context.LoadRelations();
                List<Song> result;
                switch (type)
                {
                    case SearchType.Title:
                        {
                            Log.Info("Searching title for " + query);
                            result = new List<Song>();
                            result = context.Songs.Where((item) => item.TitleId == query).ToList();
                            break;
                        }

                    case SearchType.Artist:
                        {
                            Log.Info("Searching artist for " + query);
                            result = null;
                            result = context.Songs.Where((item) => item.Artist.ArtistId == query).ToList();
                            break;
                        }

                    case SearchType.Source:
                        {
                            Log.Info("Searching source for " + query);
                            result = null;
                            result = context.Songs.Where((item) => item.Source.SourceId == query).ToList();
                            break;
                        }

                    case SearchType.Series:
                        {
                            Log.Info("Searching series for " + query);
                            result = null;

                            var affTitleList = context.Sources.Where((item) => item.Series.SeriesId == query).ToList();

                            result = new List<Song>();

                            foreach (var entry in affTitleList)
                            {
                                var sourceList = context.Songs.Where((item) => item.Source.Name.Contains(entry.Name));
                                result.AddRange(sourceList);
                            }

                            result = result.Distinct().ToList();

                            break;
                        }

                    default:
                        {
                            throw new Exception("Unsupported search type.");
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