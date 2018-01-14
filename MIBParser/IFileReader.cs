using System.Collections.Generic;

namespace MIBParser
{
    public interface IFileReader
    {
        string GetFileEntireText(string filePath);
        IEnumerable<string> GetLine(string filePath);
    }
}