using System.Threading.Tasks;

namespace MIBParser
{
    public interface ISNMPServer
    {
        Task RunRecieverLoop();
    }
}