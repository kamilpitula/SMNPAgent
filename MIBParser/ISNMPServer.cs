using System.Threading.Tasks;

namespace MIBParser
{
    public interface ISnmpServer
    {
        Task RunRecieverLoop();
    }
}