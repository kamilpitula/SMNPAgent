using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MIBParser
{
    public class SNMPServer : ISnmpServer
    {
        private const int Port = 161;
        //private BER_coding BER = new BER_coding();

        private readonly SNMPProcessor snmp;
        private string tempReturnAddres = "127.0.0.1";
        private int tempReturnPort;

        public SNMPServer(SNMPProcessor snmp)
        {
            this.snmp = snmp;
        }

        public Task RunRecieverLoop()
        {
            var recThread = new Task(RecieverLoop);
            return recThread;
        }

        public bool Send(string ip, byte[] dataToSend)
        {
            var result = false;

            var OutputSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var address = IPAddress.Parse(ip);
            var endpoint = new IPEndPoint(address, tempReturnPort);

            try
            {
                OutputSocket.SendTo(dataToSend, endpoint);
            }
            catch (Exception exception)
            {
                result = true;
                Console.WriteLine(" Błąd przy wysyłaniu pakietu: ", exception.Message);
            }

            OutputSocket.Close();

            return result;
        }

        //to pewnie będzie latać na swoim wątku
        public byte[] Recieve()
        {
            var listener = new UdpClient(Port);
            var groupEP = new IPEndPoint(IPAddress.Any, Port);
            byte[] receiveByteArray;

            receiveByteArray = listener.Receive(ref groupEP);
            tempReturnPort = groupEP.Port;
            tempReturnAddres = groupEP.Address.ToString();

            listener.Close();

            return receiveByteArray;
        }

        public void RecieverLoop()
        {
            while (true)
            {
                var temp = snmp.ProcessMessage(Recieve());
                if (temp != null)
                    Send(tempReturnAddres, temp);
            }
        }
    }
}