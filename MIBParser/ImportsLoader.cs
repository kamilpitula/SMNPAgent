using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MIBParser
{
    public class ImportsLoader : IImportsLoader
    {
        private const string ImportPattern = @"IMPORTS\s*(?<insideImports>[\s\S]* FROM \S*)";
        private const string ValuesPattern = @"((?<what>\S*)\s*)";
        private readonly IFileReader fileReader;

        public ImportsLoader(IFileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        public void GetAllFiles(string path)
        {
            var text = fileReader.GetFileEntireText(path);
            var imports = ParseImports(text);
            foreach (var import in imports)
            {
                Console.WriteLine("From file: " + import.Name + " load:");
                foreach (var importType in import.Types)
                    Console.WriteLine("  * " + importType);
            }
        }

        public IEnumerable<Import> ParseImports(string source)
        {
            var imports = new List<Import>();
            var regex = new Regex(ImportPattern, RegexOptions.Multiline);
            var valuesRegex = new Regex(ValuesPattern, RegexOptions.Multiline);

            var matches = regex.Match(source).Groups["insideImports"].Value;
            var values = valuesRegex.Matches(matches);
            var currentImportsList = new List<string>();

            for (var i = 0; i < values.Count; i++)
            {
                var valueString = values[i].Groups["what"].Value.Trim(' ', '\r', '\n', ',');
                if (valueString != "FROM")
                {
                    currentImportsList.Add(valueString);
                }
                else
                {
                    imports.Add(new Import(values[i + 1].Groups["what"].Value.Trim(' ', '\r', '\n', ','),
                        currentImportsList));
                    currentImportsList = new List<string>();
                    i++;
                }
            }

            return imports;
        }
    }
}