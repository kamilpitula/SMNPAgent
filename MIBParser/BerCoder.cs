using System;
using System.Text;

namespace MIBParser
{
    public class BerCoder : IBerCoder
    {
        public byte[] Encode(SNMPMessage inputMessage)
        {
            byte[] result = new byte[2];

            int version = 0;
            int error = 0;

            switch (inputMessage.SNMPMessageType)
            {
                case MessageType.GetResponse:
                {
                    byte[] value;


                    if (inputMessage.StringValue != null)
                    {
                        value = CodeOctetString(inputMessage.StringValue);
                    }
                    else
                    {
                        if (inputMessage.AplicationSpecId != 0)
                            value = CodeInt(inputMessage.IntValue, inputMessage.AplicationSpecId);
                        else value = CodeInt(inputMessage.IntValue);
                    }

                    result = CombineArrays(inputMessage.RawObjectId, value);

                    result = AddSequence(result);
                    result = AddSequence(result);

                    result = CombineArrays(CodeInt(error), result);
                    result = CombineArrays(CodeInt(inputMessage.Error), result);
                    result = CombineArrays(CodeInt(inputMessage.ReqId), result);

                    result = AddSnmpResponsePdu(result);

                    result = CombineArrays(CodeOctetString(inputMessage.CommunityString), result);
                    result = CombineArrays(CodeInt(version), result);

                    result = AddSequence(result);

                    Console.WriteLine(BitConverter.ToString(result));

                    break;
                }


                default:
                    Console.WriteLine("Not supported response");
                    break;
            }

            return result;
        }

        public byte[] CodeOctetString(string input)
        {

            byte[] length = CodeLength(input.Length);
            byte[] array = new byte[1 + length.Length + input.Length];
            array[0] = 0x04;
            length.CopyTo(array, 1);

            byte[] temp = new byte[input.Length];
            temp = Encoding.ASCII.GetBytes(input);
            temp.CopyTo(array, 1 + length.Length);

            Console.WriteLine("String: " + input + " to: " + BitConverter.ToString(array));
            return array;
        }

        public byte[] CodeInt(int input, byte custom_type = 0x02)
        {
            byte[] array;
            if (input <= 127 && input > -128)
            {
                array = new byte[3];
                array[0] = custom_type;
                array[1] = 0x01;
                if (input >= 0) array[2] = Convert.ToByte(input);
                else
                {
                    array[2] = Convert.ToByte(input + 128);
                    array[2] |= 0x80;
                }
            }
            else if (input <= 32767 && input > -32768)
            {
                array = new byte[4];
                array[0] = custom_type;
                array[1] = 0x02;
                if (input >= 0)
                {
                    array[2] = Convert.ToByte(input / 256);
                    array[3] = Convert.ToByte(input % 256);
                }
                else
                {
                    int temp = input + 32768;
                    array[2] = Convert.ToByte(temp / 256);
                    array[3] = Convert.ToByte(temp % 256);
                    array[2] |= 0x80;
                }
            }
            else if (input <= 8388607 && input > -8388608)
            {
                array = new byte[5];
                array[0] = custom_type;
                array[1] = 0x03;
                if (input >= 0)
                {
                    array[2] = Convert.ToByte(input / 65536);
                    array[3] = Convert.ToByte((input % 65536) / 256);
                    array[4] = Convert.ToByte((input % 65536) % 256);
                }
                else
                {
                    int temp = input + 8388608;
                    array[2] = Convert.ToByte(temp / 65536);
                    array[3] = Convert.ToByte((temp % 65536) / 256);
                    array[4] = Convert.ToByte((temp % 65536) % 256);
                    array[2] |= 0x80;
                }
            }
            else
            {
                array = new byte[6];
                array[0] = custom_type;
                array[1] = 0x04;
                if (input >= 0)
                {
                    array[2] = Convert.ToByte(input / 16777216);
                    array[3] = Convert.ToByte((input % 16777216) / 65536);
                    array[4] = Convert.ToByte(((input % 16777216) % 65536) / 256);
                    array[5] = Convert.ToByte(((input % 16777216) % 65536) % 256);
                }
                else
                {
                    int temp = input + 2147483647;
                    array[2] = Convert.ToByte(temp / 16777216);
                    array[3] = Convert.ToByte((temp % 16777216) / 65536);
                    array[4] = Convert.ToByte(((temp % 16777216) % 65536) / 256);
                    array[5] = Convert.ToByte(((temp % 16777216) % 65536) % 256);
                }
            }


            Console.WriteLine("Integer: " + BitConverter.ToString(array));

            return array;
        }


        private byte[] CodeLength(int size)
        {
            byte[] array;
            if (size < 128)
            {
                array = new byte[1];
                array[0] = Convert.ToByte(size);
            }
            else
            {
                int count = 0;
                int temp = size;
                while (temp >= 1)
                {
                    count++;
                    temp = temp / 256;
                }
                array = new byte[count + 1];
                array[0] = Convert.ToByte(count + 128);

                for (int i = count; i >= 1; i--)
                {
                    array[i] = Convert.ToByte(size % 256);
                    size = size / 256;
                }
            }
            return array;

        }
        public byte[] CombineArrays(byte[] x, byte[] y)
        {
            byte[] z = new byte[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            return z;
        }

        public byte[] AddSequence(byte[] x)
        {
            byte[] length = CodeLength(x.Length);
            byte[] output = new byte[1 + length.Length + x.Length];
            output[0] = 0x30;
            length.CopyTo(output, 1);
            x.CopyTo(output, 1 + length.Length);
            return output;
        }

        public byte[] AddSnmpResponsePdu(byte[] x)
        {
            byte[] length = CodeLength(x.Length);
            byte[] output = new byte[1 + length.Length + x.Length];
            output[0] = 0xA2;
            length.CopyTo(output, 1);
            x.CopyTo(output, 1 + length.Length);
            return output;
        }

    }

}