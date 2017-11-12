using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MIBParser
{
    public class ImportsLoader : IImportsLoader
    {
        const string ImportPattern = @"IMPORTS\s*(?<insideImports>[\s\S]* FROM \S*)";
        private const string valuesPattern = @"((?<what>\S*)\s*)";
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
                {
                    Console.WriteLine("  * " + importType);
                }
            }
        }
        public IEnumerable<Import> ParseImports(string source)
        {
            var imports = new List<Import>();
            var regex = new Regex(ImportPattern, RegexOptions.Multiline);
            var valuesRegex = new Regex(valuesPattern, RegexOptions.Multiline);

            var matches = regex.Match(source).Groups["insideImports"].Value;
            var values = valuesRegex.Matches(matches);
            var currentImportsList = new List<string>();

            for (int i = 0; i < values.Count; i++)
            {

                var valueString = values[i].Groups["what"].Value.Trim(new char[] { ' ', '\r', '\n', ',' });
                if (valueString != "FROM")
                {
                    currentImportsList.Add(valueString);
                }
                else
                {
                    imports.Add(new Import(values[i + 1].Groups["what"].Value.Trim(new char[] { ' ', '\r', '\n', ',' }), currentImportsList));
                    currentImportsList = new List<string>();
                    i++;
                }
            }

            return imports;
        }


    }
}
