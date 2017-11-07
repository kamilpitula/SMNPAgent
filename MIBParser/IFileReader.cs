using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIBParser
{
    public interface IFileReader
    {
        string GetEntireFileText(string filePath);
        IEnumerable<string> GetLine(string filePath);
    }
}
