using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace MIBParser
{
    public class SNMPServer : ISNMPServer
    {
        private int port = 161;
        private int tempReturnPort = 0;
        private string tempReturnAddres = "127.0.0.1";
        //private BER_coding BER = new BER_coding();

        private SNMPProcessor snmp;

        public SNMPServer(SNMPProcessor snmp)
        {
            this.snmp = snmp;
        }

        public bool Send(string ip, byte[] dataToSend)
        {
            bool result = false;

            Socket OutputSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint endpoint = new IPEndPoint(address, tempReturnPort);

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
            UdpClient listener = new UdpClient(port);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, port);
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
                byte[] temp = snmp.ProcessMessage(Recieve());
                if (temp != null)
                {
                    Send(tempReturnAddres, temp);
                }
            }
        }

        public Task RunRecieverLoop()
        {
            Task recThread = new Task(RecieverLoop);
            return recThread;
        }

    }
}