namespace MIBParser
{
    public interface IBerCoder
    {
        byte[] Encode(SNMPMessage inputMessage);
    }
}