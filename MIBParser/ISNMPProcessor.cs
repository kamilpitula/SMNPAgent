namespace MIBParser
{
    public interface ISnmpProcessor
    {
        byte[] ProcessMessage(byte[] incomingMessage);
    }
}