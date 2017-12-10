using System;
using System.Linq;
using System.Text;

namespace MIBParser
{
    public class BerDecoder
    {
        public SNMPMessage Decode(byte[] input) //tu się pewnie typ zmieni
        {
            Console.WriteLine(BitConverter.ToString(input));
            SNMPMessage snmp_message = new SNMPMessage();
            int snmp_version = 0;
            string community_string = "";

            int request_id = 0;
            int error = 0;
            int error_index = 0;

            string object_id = "";
            byte[] raw_obj_id = new byte[1];

            int value_int = 0;
            string value_string = "";

            switch (input[0])
            {
                case 0x30:  //snmp
                    Console.WriteLine("Typ wiadomości SNMP o długości: " + get_length(ref input));

                    snmp_version = get_int(ref input);
                    community_string = get_octet_string(ref input);

                    snmp_message.CommunityString = community_string;

                    switch (input[0])
                    {
                        case 0xA0: //SNMP get
                            get_length(ref input);

                            snmp_message.SNMPMessageType = MessageType.GetRequest;

                            request_id = get_int(ref input);
                            snmp_message.ReqId = request_id;

                            error = get_int(ref input);
                            error_index = get_int(ref input);

                            strip_sequence(ref input);
                            strip_sequence(ref input);

                            object_id = get_object_id(ref input, ref raw_obj_id);
                            snmp_message.ObjectId = object_id;
                            snmp_message.RawObjectId = raw_obj_id;

                            Console.WriteLine("Wiadmość typu SNMP GetRequest dla object_id: " + object_id);

                            break;

                        case 0xA3: //SNMP set
                            get_length(ref input);

                            snmp_message.SNMPMessageType = MessageType.SetRequest;

                            request_id = get_int(ref input);
                            snmp_message.ReqId = request_id;

                            error = get_int(ref input);
                            error_index = get_int(ref input);

                            strip_sequence(ref input);
                            strip_sequence(ref input);

                            // TODO: może zrób wiele var bindsów

                            object_id = get_object_id(ref input, ref raw_obj_id);
                            snmp_message.ObjectId = object_id;
                            snmp_message.RawObjectId = raw_obj_id;

                            switch (input[0])
                            {
                                case 0x02:
                                    value_int = get_int(ref input);
                                    snmp_message.IntValue = value_int;
                                    Console.WriteLine("Wiadmość typu SNMP SetRequest dla object_id: " + object_id + " i wartości: " + value_int);
                                    break;
                                case 0x04:
                                    value_string = get_octet_string(ref input);
                                    snmp_message.StringValue = value_string;
                                    Console.WriteLine("Wiadmość typu SNMP SetRequest dla object_id: " + object_id + " i wartości: " + value_string);
                                    break;
                                default:
                                    Console.WriteLine("Typ wartości nie jest obsługiwany");
                                    break;
                            }

                            break;

                        default:
                            Console.WriteLine("Typ zapytania SNMP nie jest obsługiwany");
                            break;
                    }

                    //Console.WriteLine(BitConverter.ToString(input));
                    break;

                default:
                    Console.WriteLine("Typ wiadomości nie jest obsługiwany");
                    break;
            }
            return snmp_message;
        }

        private void strip_sequence(ref byte[] input)
        {
            get_length(ref input);
        }

        private string get_object_id(ref byte[] input, ref byte[] raw_obj_id)
        {
            string obj_id = "";

            if (input[0] != 0x06)
            {
                Console.WriteLine("Zły typ, spodziwano się object_id");
                return "error";
            }

            byte[] temp = new byte[input.Length];
            Buffer.BlockCopy(input, 0, temp, 0, input.Length);

            int size = get_length(ref input);
            raw_obj_id = new byte[size + (temp.Length - input.Length)];
            Buffer.BlockCopy(temp, 0, raw_obj_id, 0, size + (temp.Length - input.Length));

            for (int i = 0; i < size; i++)
            {
                if (input[i] == 0x2B) obj_id += "1.3";
                else obj_id += "." + input[i].ToString();
            }

            Console.WriteLine("Object id: " + obj_id + " o długości:" + size);

            input = input.Skip(size).ToArray();

            return obj_id;
        }
        private int get_length(ref byte[] input)
        {
            int size = 0;
            if (input[1] >= 128)
            {
                for (int i = 0; i < Convert.ToInt32(input[1] & 0x7F); i++)
                {
                    if (i == 0) size = Convert.ToInt32(input[2]);
                    else size = size * 256 + Convert.ToInt32(input[2 + i]);
                }
                input = input.Skip(2 + Convert.ToInt32(input[1] & 0x7F)).ToArray();
            }
            else
            {
                size = Convert.ToInt32(input[1]);
                input = input.Skip(2).ToArray();
            }



            return size;
        }
        private int get_int(ref byte[] input)
        {
            if (input[0] != 0x02)
            {
                Console.WriteLine("Zły typ, spodziwano się inta");
                return -1;
            }
            int value = 0;
            int length = get_length(ref input);
            for (int i = 0; i < length; i++)
            {
                if (i == 0) value = Convert.ToInt32(input[i] & 0x7F);
                else value = value * 256 + Convert.ToInt32(input[i]);
            }
            value = value - Convert.ToInt32((input[0] & 0x80) << 8 * (length - 1));
            Console.WriteLine("Int o długości: " + length + " i wartości: " + value);
            input = input.Skip(length).ToArray();
            return value;
        }

        private string get_octet_string(ref byte[] input)
        {
            if (input[0] != 0x04)
            {
                Console.WriteLine("Zły typ, spodziwano się stringa");
                return "error";
            }
            string value = "";
            int length = get_length(ref input);
            byte[] my_string = new byte[length];
            Array.Copy(input, 0, my_string, 0, length);
            Console.WriteLine("Octet string: " + Encoding.ASCII.GetString(my_string) + " o długości:" + length);
            value = Encoding.ASCII.GetString(my_string);
            input = input.Skip(length).ToArray();
            return value;
        }
    }
}