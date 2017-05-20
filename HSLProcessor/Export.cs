using System;
using System.IO;
using System.Xml.Linq;
using log4net;

namespace HSLProcessor
{
    static class Exporter
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Exporter));
        public static void ExportXml(FileInfo file)
        {
            try
            {
                HSLContext context = new HSLContext();

                XElement xl = new XElement("hsl");
                foreach (var item in context.Songs)
                {
                    XElement xl_item = new XElement("entry");
                    xl_item.SetAttributeValue("id", item.Id);
                    xl_item.SetElementValue("title", item.Title);
                    xl_item.SetElementValue("artist", item.Artist);
                    xl_item.SetElementValue("source", item.Reference);
                    xl.Add(xl_item);
                }

                xl.Save(new StreamWriter(new FileStream(file.FullName, FileMode.CreateNew)));
            }
            catch (Exception ex)
            {
                Log.Error("Failed exporting to XML");
                Log.Debug(ex.Message);
            }
        }
    }
}