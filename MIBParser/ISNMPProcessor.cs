namespace MIBParser
{
    public interface ISNMPProcessor
    {
        byte[] ProcessMessage(byte[] incoming_message);
    }
}