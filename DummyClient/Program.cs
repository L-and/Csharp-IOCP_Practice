using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient
{


    class Program
    {
        static void Main(string[] args)
        {
            // IP Settings
            int port = 7777;
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port);

            Connector connector = new Connector();

            connector.Connect(endPoint, () => { return new ServerSession(); });

            while(true)
            {
                try
                {

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(1000);
            }
        }
    }
}