using System;
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

                var invokedVerb = "";
                object invokedVerbInstance = null;

                var options = new Options();

                if (args.Length == 0)
                {
                    options.GetUsage();
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
                
                if (!CommandLine.Parser.Default.ParseArguments(args, options,
                    (verb, subOptions) =>
                    {
                        // if parsing succeeds the verb name and correct instance
                        // will be passed to onVerbCommand delegate (string,object)
                        invokedVerb = verb;
                        invokedVerbInstance = subOptions;
                    }))
                {
                    options.GetUsage();
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }

                switch (invokedVerb)
                {
                    case "import":
                    {
                        Console.WriteLine("Import mode");
                        var importSubOptions = (Options.ImportSubOptions) invokedVerbInstance;
                        if (importSubOptions.File != null)
                        {
                            switch (importSubOptions.Type)
                            {
                                case "csv":
                                {
                                    Console.WriteLine("CSV");
                                    var result = Importer.ImportCsv(new FileInfo(importSubOptions.File));

                                    if (result == Importer.ImportResult.Failed)
                                    {
                                        Log.Error("Import failed");

                                        return -1;
                                    }

                                    break;
                                }

                                case "protobuf":
                                {
                                    Console.WriteLine("Protocol Buffer");
                                    Importer.ImportProtocolBuffer(new FileInfo(importSubOptions.File));
                                    break;
                                }
                                    
                                case "xml":
                                {
                                    Console.WriteLine("XML");
                                    Importer.ImportXml(new FileInfo(importSubOptions.File));
                                    break;
                                }

                                case "seriescsv":
                                {
                                    Console.WriteLine("Series CSV");
                                    Importer.ImportSourceSeriesCsv(new FileInfo(importSubOptions.File));
                                    break;
                                }
                            }
                        }
                        break;
                    }

                    case "list":
                    {
                        Lister.List();
                        break;
                    }

                    case "export":
                    {
                        Console.WriteLine("Export mode");
                        var exportSubOptions = (Options.ExportSubOptions) invokedVerbInstance;
                        if (exportSubOptions.File != null)
                        {
                            switch (exportSubOptions.Type)
                            {
                                case "xml":
                                {
                                    Console.WriteLine("XML");
                                    Exporter.ExportXml(new FileInfo(exportSubOptions.File));
                                    break;
                                }

                                case "json":
                                {
                                    Console.WriteLine("JSON");
                                    Exporter.ExportJson(new FileInfo(exportSubOptions.File));
                                    break;
                                }

                                case "protobuf":
                                {
                                    Console.WriteLine("Protocol Buffer");
                                    Exporter.ExportProtoBuffer(new FileInfo(exportSubOptions.File));
                                    break;
                                }
                            }
                        }
                        break;
                    }

                    case "generate":
                    {
                        Console.WriteLine("Generation mode");
                        var generateSubOptions = (Options.GenerateSubOptions) invokedVerbInstance;
                        switch (generateSubOptions.Type)
                        {
                            case "sitemap":
                            {
                                Console.WriteLine("Sitemap");
                                if (generateSubOptions.File != null)
                                {
                                    PageGenerator.ExportSitemap(new FileInfo(generateSubOptions.File),
                                        generateSubOptions.Base);
                                }
                                break;
                            }

                            case "html":
                            {
                                Console.WriteLine("HTML tree");
                                if (generateSubOptions.File != null && generateSubOptions.Base != null)
                                {
                                    PageGenerator.Generate(new DirectoryInfo(generateSubOptions.File),
                                        new DirectoryInfo(generateSubOptions.Base));
                                }
                                break;
                            }
                        }
                        break;
                    }

                    case "search":
                    {
                        var searchSubOptions = (Options.SearchSubOptions) invokedVerbInstance;
                        // Get information for all
                        var (result1, hit1) = Searcher.Search(searchSubOptions.Query, Searcher.SearchType.Title);
                        var (result2, hit2) = Searcher.Search(searchSubOptions.Query, Searcher.SearchType.Artist);
                        var (result3, hit3) = Searcher.Search(searchSubOptions.Query, Searcher.SearchType.Source);

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

                    case "delete":
                    {
                        var deleteSubOptions = (Options.DeleteSubOptions) invokedVerbInstance;
                        Updater.Delete(new Guid(deleteSubOptions.Id));
                        break;
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