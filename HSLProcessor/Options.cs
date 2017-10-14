using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace HSLProcessor
{
    public class Options
    {
        [VerbOption("import", HelpText = "Import from a file.")]
        public ImportSubOptions ImportVerb { get; set; }

        [VerbOption("export", HelpText = "Export to a file.")]
        public ExportSubOptions ExportVerb { get; set; }

        [VerbOption("list", HelpText = "List the database.")]
        public ListSubOptions ListVerb { get; set; }

        [VerbOption("generate", HelpText = "Generate the content.")]
        public GenerateSubOptions GenerateVerb { get; set; }

        [VerbOption("search", HelpText = "Search the database.")]
        public SearchSubOptions SearchVerb { get; set; }

        [VerbOption("delete", HelpText = "Delete from the database.")]
        public DeleteSubOptions DeleteVerb { get; set; }


        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this,
                (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        public class ExportSubOptions
        {
            [Option('t', "type", Required = true,
                HelpText = "Type of file.")]
            public string Type { get; set; }

            [Option('f', "file", Required = true,
                HelpText = "File to process.")]
            public string File { get; set; }
        }

        public class ImportSubOptions
        {
            [Option('t', "type", Required = true,
                HelpText = "Type of file.")]
            public string Type { get; set; }

            [Option('f', "file", Required = true,
                HelpText = "File to process.")]
            public string File { get; set; }
        }

        public class ListSubOptions
        {
        }

        public class DeleteSubOptions
        {
            [Option('i', "item", Required = true,
                HelpText = "Item to delete.")]
            public string Id { get; set; }
        }

        public class SearchSubOptions
        {
            [Option('q', "query", Required = true,
                HelpText = "Search query.")]
            public string Query { get; set; }
        }

        public class GenerateSubOptions
        {
            [Option('t', "type", Required = true,
                HelpText = "Type of generation to perform..")]
            public string Type { get; set; }

            [Option('b', "base",
                HelpText = "Base of the structure.")]
            public string Base { get; set; }

            [Option('f', "file", Required = true,
                HelpText = "File to process.")]
            public string File { get; set; }
        }
    }
}