namespace MIBParser
{
    public interface IBerDecoder
    {
        SNMPMessage Decode(byte[] input);
    }
}