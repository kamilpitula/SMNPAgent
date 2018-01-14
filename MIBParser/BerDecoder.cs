using System;
using System.Linq;
using System.Text;

namespace MIBParser
{
    public class BerDecoder:IBerDecoder
    {
        public SNMPMessage Decode(byte[] input) //tu się pewnie typ zmieni
        {
            Console.WriteLine(BitConverter.ToString(input));
            var snmpMessage = new SNMPMessage();

            var rawObjId = new byte[1];

            switch (input[0])
            {
                case 0x30: //snmp
                    Console.WriteLine("Typ wiadomości SNMP o długości: " + getLength(ref input));

                    if (input != null)
                    {
                        GetInt(ref input);
                        var communityString = GetOctetString(ref input);

                        snmpMessage.CommunityString = communityString;

                        int requestId;
                        string objectId;
                        switch (input[0])
                        {
                            case 0xA0: //SNMP get
                                getLength(ref input);

                                snmpMessage.SNMPMessageType = SNMPMessageTypes.GetRequest;

                                requestId = GetInt(ref input);
                                snmpMessage.ReqId = requestId;

                                StripSequence(ref input);
                                StripSequence(ref input);

                                objectId = GetObjectId(ref input, ref rawObjId);
                                snmpMessage.ObjectId = objectId;
                                snmpMessage.RawObjectId = rawObjId;

                                Console.WriteLine("Wiadmość typu SNMP GetRequest dla object_id: " + objectId);

                                break;

                            case 0xA3: //SNMP set
                                getLength(ref input);

                                snmpMessage.SNMPMessageType = SNMPMessageTypes.SetRequest;

                                requestId = GetInt(ref input);
                                snmpMessage.ReqId = requestId;

                                StripSequence(ref input);
                                StripSequence(ref input);

                                objectId = GetObjectId(ref input, ref rawObjId);
                                snmpMessage.ObjectId = objectId;
                                snmpMessage.RawObjectId = rawObjId;
                                input = DecodeValue(input, snmpMessage, objectId);

                                break;

                            default:
                                Console.WriteLine("Typ zapytania SNMP nie jest obsługiwany");
                                break;
                        }
                    }

                    //Console.WriteLine(BitConverter.ToString(input));
                    break;

                default:
                    Console.WriteLine("Typ wiadomości nie jest obsługiwany");
                    break;
            }
            return snmpMessage;
        }

        public byte[] DecodeValue(byte[] input, SNMPMessage snmpMessage, string objectId)
        {
            switch (input[0])
            {
                case 0x02:
                    var valueInt = GetInt(ref input);
                    snmpMessage.IntValue = valueInt;
                    Console.WriteLine("Wiadmość typu SNMP SetRequest dla object_id: " + objectId +
                                      " i wartości: " + valueInt);
                    break;
                case 0x04:
                    var valueString = GetOctetString(ref input);
                    snmpMessage.OctetStringValue = valueString;
                    Console.WriteLine("Wiadmość typu SNMP SetRequest dla object_id: " + objectId +
                                      " i wartości: " + valueString);
                    break;
                case 0x10:
                    getLength(ref input);
                    SNMPMessage sequence = GetSequence(ref input);
                    snmpMessage = sequence;
                    break;
                case 0x05:
                    snmpMessage.IsNull = true;
                    Console.WriteLine("Wiadomosc typu SNMP SetRequest dla object_i: " + objectId + " o wartosci null");
                    break;
                default:
                    Console.WriteLine("Typ wartości nie jest obsługiwany");
                    break;
            }

            return input;
        }

        public SNMPMessage GetSequence(ref byte[] input)
        {
            var result = new SNMPMessage();
            if (input[0] != 0x10)
            {
                Console.WriteLine("Zły typ, spodziewano się sequence");
            }
            var temp = new byte[input.Length];
            Buffer.BlockCopy(input, 0, temp, 0, input.Length);
            //getLength(ref input);
            var size = (int)input[1];
            var second = input;
            if (size<temp.Length-2)
            {

                
                var first = input.Take(size + 2).ToArray();
                second = input.Skip(size + 2).ToArray();
                getLength(ref first);
                result.Sequence = GetSequence(ref first);
                
                
            }
            DecodeValue(second, result, "internal");
            return result;
        }

        private void StripSequence(ref byte[] input)
        {
            getLength(ref input);
        }

        public string GetObjectId(ref byte[] input, ref byte[] rawObjId)
        {
            var objId = "";

            if (input[0] != 0x06)
            {
                Console.WriteLine("Zły typ, spodziwano się object_id");
                return "error";
            }

            var temp = new byte[input.Length];
            Buffer.BlockCopy(input, 0, temp, 0, input.Length);

            var size = getLength(ref input);
            rawObjId = new byte[size + (temp.Length - input.Length)];
            Buffer.BlockCopy(temp, 0, rawObjId, 0, size + (temp.Length - input.Length));

            for (var i = 0; i < size; i++)
                if (input[i] == 0x2B) objId += "1.3";
                else objId += "." + input[i];

            Console.WriteLine("Object id: " + objId + " o długości:" + size);

            input = input.Skip(size).ToArray();

            return objId;
        }

        private int getLength(ref byte[] input)
        {
            var size = 0;
            if (input[1] >= 128)
            {
                for (var i = 0; i < Convert.ToInt32(input[1] & 0x7F); i++)
                    if (i == 0) size = Convert.ToInt32(input[2]);
                    else size = size * 256 + Convert.ToInt32(input[2 + i]);
                input = input.Skip(2 + Convert.ToInt32(input[1] & 0x7F)).ToArray();
            }
            else
            {
                size = Convert.ToInt32(input[1]);
                input = input.Skip(2).ToArray();
            }


            return size;
        }

        public int GetInt(ref byte[] input)
        {
            if (input[0] != 0x02)
            {
                Console.WriteLine("Zły typ, spodziwano się inta");
                return -1;
            }
            var value = 0;
            var length = getLength(ref input);
            for (var i = 0; i < length; i++)
                if (i == 0) value = Convert.ToInt32(input[i] & 0x7F);
                else value = value * 256 + Convert.ToInt32(input[i]);
            value = value - Convert.ToInt32((input[0] & 0x80) << (8 * (length - 1)));
            Console.WriteLine("Int o długości: " + length + " i wartości: " + value);
            input = input.Skip(length).ToArray();
            return value;
        }

        public string GetOctetString(ref byte[] input)
        {
            if (input[0] != 0x04)
            {
                Console.WriteLine("Zły typ, spodziwano się stringa");
                return "error";
            }
            var length = getLength(ref input);
            var myString = new byte[length];
            Array.Copy(input, 0, myString, 0, length);
            Console.WriteLine("Octet string: " + Encoding.ASCII.GetString(myString) + " o długości:" + length);
            var value = Encoding.ASCII.GetString(myString);
            input = input.Skip(length).ToArray();
            return value;
        }
    }
}