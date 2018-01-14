using System.Linq;

namespace MIBParser
{
    public class SNMPProcessor : ISNMPProcessor
    {
        private IBerDecoder decoder;
        private readonly IBerCoder coder;
        private readonly MIBNode master;
        private readonly string community="community";

        public SNMPProcessor(IBerDecoder decoder, IBerCoder coder, MIBNode master)
        {
            this.decoder = decoder;
            this.coder = coder;
            this.master = master;
        }

        public byte[] ProcessMessage(byte[] incomingMessage)
        {

            SNMPMessage incoming = decoder.Decode(incomingMessage);
            SNMPMessage outgoing = new SNMPMessage();
            outgoing.SNMPMessageType = SNMPMessageTypes.GetResponse;
            outgoing.CommunityString = incoming.CommunityString;
            outgoing.RawObjectId = incoming.RawObjectId;
            outgoing.ReqId = incoming.ReqId;

            if (incoming.CommunityString != community)
            {
                return null;
            }

            switch (incoming.SNMPMessageType)
            {
                case SNMPMessageTypes.GetRequest:
                    if (incoming.ObjectId[incoming.ObjectId.Length - 1] == '0')
                    {
                        string object_id = incoming.ObjectId.Substring(2);
                        ObjectType myNode = (ObjectType) master.GetMibNodeStack()
                            .FirstOrDefault(node => node.GetOID() == object_id.Substring(0, object_id.Length - 2));
                            


                        if (myNode != null)
                        {
                            if (myNode.IsReadable())
                            {
                                switch (myNode.nodeType)
                                {
                                    case NodeTypes.Type_INTEGER:
                                        outgoing.IntValue = myNode.IntValue;
                                        break;

                                    case NodeTypes.Type_Counter:
                                        outgoing.IntValue = myNode.IntValue;
                                        outgoing.AplicationSpecId = 0x41;
                                        break;

                                    case NodeTypes.Type_Gauge:
                                        outgoing.IntValue = myNode.IntValue;
                                        outgoing.AplicationSpecId = 0x42;
                                        break;

                                    case NodeTypes.Type_TimeTicks:
                                        outgoing.IntValue = myNode.IntValue;
                                        outgoing.AplicationSpecId = 0x43;
                                        break;

                                    case NodeTypes.Type_DisplayString:
                                    case NodeTypes.Type_PhysAddress:
                                        outgoing.OctetStringValue = myNode.OctetStringValue;
                                        break;

                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                outgoing.Error = 0x05;
                            }
                        }
                        else
                        {
                            outgoing.Error = 0x02;
                        }
                    }
                    break;

                case SNMPMessageTypes.SetRequest:
                    if (incoming.ObjectId[incoming.ObjectId.Length - 1] == '0')
                    {
                        string object_id = incoming.ObjectId.Substring(2);
                        ObjectType myNode =(ObjectType) master.GetMibNodeStack()
                            .FirstOrDefault(node => node.GetOID() == object_id.Substring(0, object_id.Length - 2));
                        if (myNode != null)
                        {
                            if (myNode.IsWritable())
                            {
                                if (incoming.OctetStringValue != null)
                                {
                                    switch (myNode.nodeType)
                                    {
                                        case NodeTypes.Type_DisplayString:
                                        case NodeTypes.Type_PhysAddress:
                                            outgoing.OctetStringValue = incoming.OctetStringValue;
                                            if (myNode.SetValue(incoming.OctetStringValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;

                                        default:
                                            outgoing.Error = 0x03;
                                            break;
                                    }
                                }
                                else
                                {
                                    switch (myNode.nodeType)
                                    {
                                        case NodeTypes.Type_INTEGER:
                                            outgoing.IntValue = incoming.IntValue;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;


                                        case NodeTypes.Type_Counter:
                                            outgoing.IntValue = incoming.IntValue;
                                            outgoing.AplicationSpecId = 0x41;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error= 0x05;
                                            break;

                                        case NodeTypes.Type_Gauge:
                                            outgoing.IntValue = incoming.IntValue;
                                            outgoing.AplicationSpecId = 0x42;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;

                                        case NodeTypes.Type_TimeTicks:
                                            outgoing.IntValue = incoming.IntValue;
                                            outgoing.AplicationSpecId = 0x43;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;

                                        default:
                                            outgoing.Error = 0x03;
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                outgoing.Error = 0x04;
                            }
                        }
                        else
                        {
                            outgoing.Error = 0x02;
                        }
                    }
                    break;

                default:
                    break;
            }

            byte[] return_message = coder.Encode(outgoing);
            return return_message;

        }

    }
}