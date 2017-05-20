﻿using System;
using System.IO;
using System.Reflection;
using System.Xml;
using log4net;
using log4net.Config;
using log4net.Repository;

namespace HSLProcessor
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        static int Main(string[] args)
        {
            try
            {
                // Configuration for logging
                XmlDocument log4netConfig = new XmlDocument();

                using (StreamReader reader = new StreamReader(new FileStream("log4net.config", FileMode.Open, FileAccess.Read)))
                {
                    log4netConfig.Load(reader);
                }

                ILoggerRepository rep = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                XmlConfigurator.Configure(rep, log4netConfig["log4net"]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Loggng error");
                Console.WriteLine(ex.Message);
                return -1;
            }

            Log.Debug("Starting the process.");

            // No argument
            if (args.Length == 0)
            {
                Log.Error("Missing arguments!");
                Console.WriteLine("Options are: importcsv, importxml, importcsv, and list");
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

                        var result = Importer.ImportCsv(args[1]);

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

                // Other (invalid) options
                default:
                    {
                        Log.Error("Unknown option.");
                        return -1;
                    }
            }

            return 0;
        }
    }
}
