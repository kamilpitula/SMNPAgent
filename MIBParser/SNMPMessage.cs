namespace MIBParser
{
    public class SNMPMessage
    {

        public MessageType SNMPMessageType { get; set; }
        public int ReqId { get; set; }
        public string ObjectId { get; set; }
        public byte[] RawObjectId { get; set; }
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public string CommunityString { get; set; }
        public int Error { get; set; }
        public byte AplicationSpecId { get; set; }

    }
}