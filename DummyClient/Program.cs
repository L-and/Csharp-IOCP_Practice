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

            const int DUMMY_COUNT = 500;

            connector.Connect(endPoint, 
                () => { return SessionManager.Instance.Generate(); },
                DUMMY_COUNT);

            while(true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                Thread.Sleep(250);
            }
        }
    }
}