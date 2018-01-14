using System.Linq;

namespace MIBParser
{
    public class SNMPProcessor : ISnmpProcessor
    {
        private readonly IBerCoder coder;
        private readonly string community = "community";
        private readonly MibNode master;
        private readonly IBerDecoder decoder;

        public SNMPProcessor(IBerDecoder decoder, IBerCoder coder, MibNode master)
        {
            this.decoder = decoder;
            this.coder = coder;
            this.master = master;
        }

        public byte[] ProcessMessage(byte[] incomingMessage)
        {
            var incoming = decoder.Decode(incomingMessage);
            var outgoing = new SNMPMessage();
            outgoing.SNMPMessageType = SNMPMessageTypes.GetResponse;
            outgoing.CommunityString = incoming.CommunityString;
            outgoing.RawObjectId = incoming.RawObjectId;
            outgoing.ReqId = incoming.ReqId;

            if (incoming.CommunityString != community)
                return null;

            switch (incoming.SNMPMessageType)
            {
                case SNMPMessageTypes.GetRequest:
                    if (incoming.ObjectId[incoming.ObjectId.Length - 1] == '0')
                    {
                        var object_id = incoming.ObjectId.Substring(2);
                        var myNode = (ObjectType) master.GetMibNodeStack()
                            .FirstOrDefault(node => node.GetOID() == object_id.Substring(0, object_id.Length - 2));


                        if (myNode != null)
                            if (myNode.IsReadable())
                                switch (myNode.NodeType)
                                {
                                    case NodeTypes.TypeInteger:
                                        outgoing.IntValue = myNode.IntValue;
                                        break;

                                    case NodeTypes.TypeCounter:
                                        outgoing.IntValue = myNode.IntValue;
                                        outgoing.AplicationSpecId = 0x41;
                                        break;

                                    case NodeTypes.TypeGauge:
                                        outgoing.IntValue = myNode.IntValue;
                                        outgoing.AplicationSpecId = 0x42;
                                        break;

                                    case NodeTypes.TypeTimeTicks:
                                        outgoing.IntValue = myNode.IntValue;
                                        outgoing.AplicationSpecId = 0x43;
                                        break;

                                    case NodeTypes.TypeDisplayString:
                                    case NodeTypes.TypePhysAddress:
                                        outgoing.OctetStringValue = myNode.OctetStringValue;
                                        break;

                                    default:
                                        break;
                                }
                            else
                                outgoing.Error = 0x05;
                        else
                            outgoing.Error = 0x02;
                    }
                    break;

                case SNMPMessageTypes.SetRequest:
                    if (incoming.ObjectId[incoming.ObjectId.Length - 1] == '0')
                    {
                        var object_id = incoming.ObjectId.Substring(2);
                        var myNode = (ObjectType) master.GetMibNodeStack()
                            .FirstOrDefault(node => node.GetOID() == object_id.Substring(0, object_id.Length - 2));
                        if (myNode != null)
                            if (myNode.IsWritable())
                                if (incoming.OctetStringValue != null)
                                    switch (myNode.NodeType)
                                    {
                                        case NodeTypes.TypeDisplayString:
                                        case NodeTypes.TypePhysAddress:
                                            outgoing.OctetStringValue = incoming.OctetStringValue;
                                            if (myNode.SetValue(incoming.OctetStringValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;

                                        default:
                                            outgoing.Error = 0x03;
                                            break;
                                    }
                                else
                                    switch (myNode.NodeType)
                                    {
                                        case NodeTypes.TypeInteger:
                                            outgoing.IntValue = incoming.IntValue;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;


                                        case NodeTypes.TypeCounter:
                                            outgoing.IntValue = incoming.IntValue;
                                            outgoing.AplicationSpecId = 0x41;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;

                                        case NodeTypes.TypeGauge:
                                            outgoing.IntValue = incoming.IntValue;
                                            outgoing.AplicationSpecId = 0x42;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;

                                        case NodeTypes.TypeTimeTicks:
                                            outgoing.IntValue = incoming.IntValue;
                                            outgoing.AplicationSpecId = 0x43;
                                            if (myNode.SetValue((int) incoming.IntValue)) ;
                                            else outgoing.Error = 0x05;
                                            break;

                                        default:
                                            outgoing.Error = 0x03;
                                            break;
                                    }
                            else
                                outgoing.Error = 0x04;
                        else
                            outgoing.Error = 0x02;
                    }
                    break;

                default:
                    break;
            }

            var return_message = coder.Encode(outgoing);
            return return_message;
        }
    }
}