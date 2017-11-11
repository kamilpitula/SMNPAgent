using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            using (StreamReader sr = File.OpenText(filePath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
    }
}
