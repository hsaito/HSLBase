using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class Updater
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Updater));
        public enum UpdateResult { Success, Failed }

        public enum UpdateType { Title, Artist, Source }

        /// <summary>
        /// Delete entry by Guid
        /// </summary>
        /// <param name="item">Guid of the item to delete</param>
        /// <param name="type">Type of the item</param>
        /// <returns>Result of the operation</returns>
        public static UpdateResult Delete(Guid item, UpdateType type)
        {
            try
            {
                var context = new HSLContext();
                switch (type)
                {
                    case UpdateType.Title:
                        {
                            Log.Info(string.Format("Finding and deleting {0} for title.",item));
                            var entry = context.Songs.Find(item);
                            if (entry == null)
                                return UpdateResult.Failed;
                            context.Songs.Remove(entry);
                            context.SaveChanges();
                            break;
                        }

                    case UpdateType.Artist:
                        {
                            Log.Info(string.Format("Finding and deleting {0} for artist.",item));
                            var entry = context.Artists.Find(item);
                            if (entry == null)
                                return UpdateResult.Failed;
                            context.Artists.Remove(entry);
                            context.SaveChanges();
                            break;
                        }

                    case UpdateType.Source:
                        {
                            Log.Info(string.Format("Finding and deleting {0} for source.",item));
                            var entry = context.Sources.Find(item);
                            if (entry == null)
                                return UpdateResult.Failed;
                            context.Sources.Remove(entry);
                            context.SaveChanges();
                            break;
                        }
                }

                return UpdateResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed searching the database");
                Log.Debug(ex.Message);
                return UpdateResult.Failed;
            }
        }

        /// <summary>
        /// Delete entry by Guid
        /// </summary>
        /// <param name="item">Guid of the item to delete</param>
        /// <returns>Result of the operation</returns>
        public static UpdateResult Delete(Guid item)
        {
            try
            {
                Log.Info(string.Format("Searching {0} for deletion.",item));
                var result_title = Delete(item, UpdateType.Title) == UpdateResult.Success;
                var result_artist = Delete(item, UpdateType.Artist) == UpdateResult.Success;
                var result_source = Delete(item, UpdateType.Source) == UpdateResult.Success;

                if (result_title || result_artist || result_source)
                {
                    Log.Info("Deletion completed.");
                    return UpdateResult.Success;
                }
                else
                {
                    Log.Info("No hit. No changes made.");
                    return UpdateResult.Failed;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed searching the database");
                Log.Debug(ex.Message);
                return UpdateResult.Failed;
            }
        }
    }
}