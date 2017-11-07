using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MIBParser
{
    public class ImportsLoader
    {
        const string ImportPattern = @"\s{4}(?<name>.+)\s+FROM (?<from>.+)";
        private readonly IFileReader fileReader;

        public ImportsLoader(IFileReader fileReader)
        {
            this.fileReader = fileReader;
        }

        public void GetAllFiles(string path)
        {
            var text = fileReader.GetEntireFileText(path);
            var imports = ParseImports(text);
            foreach (var import in imports)
            {
                Console.WriteLine("From file: "+import.Name+" load:");
                foreach (var importType in import.Types)
                {
                    Console.WriteLine("  * "+importType);
                }
            }
        }
        private IEnumerable<Import> ParseImports(string source)
        {
            var imports = new List<Import>();
            var regex = new Regex(ImportPattern, RegexOptions.IgnoreCase);

            var matches = regex.Matches(source);

            foreach (Match match in matches)
            {
                var groups = match.Groups;
                imports.Add(new Import(groups["from"].ToString(),groups["name"].ToString()));
            }

            return imports;
        }

        
    }
}
