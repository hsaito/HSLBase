﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository.Hierarchy;

namespace HSLProcessor
{
    internal class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        public static int Main(string[] args)
        {
            try
            {
                // Configuration for logging
                var log4NetConfig = new XmlDocument();

                using (var reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4NetConfig.Load(reader);
                }

                var rep = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(Hierarchy));
                XmlConfigurator.Configure(rep, log4NetConfig["log4net"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loggng error");
                Console.WriteLine(ex.Message);
                return -1;
            }

            try
            {
                Log.Debug("Starting the process.");

                // No argument
                if (args.Length == 0)
                {
                    Log.Error("Missing arguments!");
                    Console.WriteLine(
                        "Options are: importcsv, importxml, importseriescsv, exportxml, exportjson, generatehtml, generatesitemap, deleteitem, and list");
                    return -1;
                }

                switch (args[0])
                {
                    // Import
                    case "importcsv":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error("Missing file name.");
                            return -1;
                        }

                        var result = Importer.ImportCsv(new FileInfo(args[1]));

                        if (result == Importer.ImportResult.Failed)
                        {
                            Log.Error("Import failed");

                            return -1;
                        }

                        break;
                    }

                    // List
                    case "list":
                    {
                        Lister.List();
                        break;
                    }

                    // Export to XML
                    case "exportxml":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error("Missing file name.");
                            return -1;
                        }
                        Exporter.ExportXml(new FileInfo(args[1]));
                        break;
                    }

                    case "exportjson":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error("Missing file name.");
                            return -1;
                        }

                        Exporter.ExportJson(new FileInfo(args[1]));
                        break;
                    }

                    case "generatesitemap":
                    {
                        if (args.Length < 3)
                        {
                            Log.Error("Missing arguments.\nUsage: file urlbase");
                            return -1;
                        }

                        PageGenerator.ExportSitemap(new FileInfo(args[1]), args[2]);
                        break;
                    }

                    // Import XML
                    case "importxml":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error("Missing file name.");
                            return -1;
                        }
                        Importer.ImportXml(new FileInfo(args[1]));
                        break;
                    }

                    case "importseriescsv":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error("Missing file name.");
                            return -1;
                        }
                        Importer.ImportSourceSeriesCsv(new FileInfo(args[1]));
                        break;
                    }

                    // Search
                    case "search":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error("Missing file name.");
                            return -1;
                        }

                        // Get information for all
                        var (result1, hit1) = Searcher.Search(args[1], Searcher.SearchType.Title);
                        var (result2, hit2) = Searcher.Search(args[1], Searcher.SearchType.Artist);
                        var (result3, hit3) = Searcher.Search(args[1], Searcher.SearchType.Source);

                        // Merge the list
                        var hit = new List<Song>();
                        if (result1 == Searcher.SearchResult.Success &&
                            result1 == result2 &&
                            result2 == result3)
                        {
                            hit.AddRange(hit1);
                            hit.AddRange(hit2);
                            hit.AddRange(hit3);
                        }

                        // Uniquify + Sort
                        var uniqueList = hit.GroupBy(song => song.TitleId).Select(group => group.First()).ToList();
                        uniqueList = uniqueList.OrderBy(list => list.Title).ToList();
                        Lister.List(uniqueList.ToList());
                        break;
                    }

                    // Generate HTML static pages
                    case "generatehtml":
                    {
                        if (args.Length < 3)
                        {
                            Log.Error("Missing template and  file name.");
                            return -1;
                        }
                        PageGenerator.Generate(new DirectoryInfo(args[1]), new DirectoryInfo(args[2]));
                        break;
                    }

                    case "deleteitem":
                    {
                        if (args.Length < 2)
                        {
                            Log.Error("Missing file name.");
                            return -1;
                        }
                        Updater.Delete(new Guid(args[1]));
                        break;
                    }

                    // Other (invalid) options
                    default:
                    {
                        Log.Error("Unknown option.");
                        return -1;
                    }
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
                Log.Debug(ex.StackTrace);
                return -1;
            }
        }
    }
}