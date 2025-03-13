using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;

namespace Server
{
    

    class Program
    {
        static Listener _listener = new Listener();
        public static GameRoom Room = new GameRoom();

        static void Main(string[] args)
        {
            #region IP 설정

            int port = 7777; // 서버 포트설정
            string host = Dns.GetHostName(); // Local 의 호스트이름
            IPHostEntry ipHost = Dns.GetHostEntry(host); // local host의 ipHost
            IPAddress ipAddr = ipHost.AddressList[0]; // Ipv6 링크-로컬주소를 사용
            IPEndPoint endPoint = new IPEndPoint(ipAddr, port); // 주소와 포트로 endPoint 설정

            #endregion

            _listener.init(endPoint, () => { return SessionManager.Instance.Generate(); });
            Console.WriteLine("Listening...");
            while (true)
            {

            }


        }
    }
}