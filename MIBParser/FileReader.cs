using System.Collections.Generic;
using System.IO;

namespace MIBParser
{
    public class FileReader : IFileReader
    {
        public string GetFileEntireText(string filePath)
        {
            return File.ReadAllText(filePath).Trim('\n');
        }

        public IEnumerable<string> GetLine(string filePath)
        {
            using (var sr = File.OpenText(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                    yield return line;
            }
        }
    }
}