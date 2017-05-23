using System;
using System.IO;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class Exporter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Exporter));
        public enum ExportResult { Success, Failed }

        /// <summary>
        /// Export to XML
        /// </summary>
        /// <param name="file">XML file to export to</param>
        /// <returns>Result of the export</returns>
        public static ExportResult ExportXml(FileInfo file)
        {
            try
            {
                HSLContext context = new HSLContext();

                // Create a root element
                XElement xl = new XElement("hsl");
                foreach (var item in context.Songs)
                {
                    XElement xl_item = new XElement("entry");
                    xl_item.SetAttributeValue("id", item.Id);
                    xl_item.SetElementValue("title", item.Title);
                    xl_item.SetElementValue("artist", item.Artist);
                    xl_item.SetElementValue("source", item.Source);
                    xl.Add(xl_item);
                }

                // Save to the file
                xl.Save(new StreamWriter(new FileStream(file.FullName, FileMode.CreateNew)));
                return ExportResult.Success;
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML.");
                Log.Debug(ex.Message);
                return ExportResult.Failed;
            }
        }
    }
}