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

                context.LoadRelations();

                // Create a root element
                XElement xl = new XElement("hsl");
                foreach (var item in context.Songs)
                {
                    XElement xl_item = new XElement("entry");
                    xl_item.SetAttributeValue("id", item.TitleId);
                    xl_item.SetElementValue("title", item.Title);

                    XElement xl_artist = new XElement("artist");
                    xl_artist.SetAttributeValue("id",item.Artist.ArtistId);
                    xl_artist.Value = item.Artist.Name;
                    xl_item.Add(xl_artist);

                    XElement xl_source = new XElement("source");
                    xl_source.SetAttributeValue("id",item.Source.SourceId);
                    xl_source.Value = item.Source.Name;
                    xl_item.Add(xl_source);

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