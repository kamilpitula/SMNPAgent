using System.Collections.Generic;

namespace MIBParser
{
    public interface IImportsLoader
    {
        void GetAllFiles(string path);
        IEnumerable<Import> ParseImports(string source);
    }
}