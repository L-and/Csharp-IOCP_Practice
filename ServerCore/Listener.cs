using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            _listenSocket.Bind(endPoint);

            // 소켓을 수신상태로 설정 및 backlog(대기큐의 길이)를 10으로 설정
            _listenSocket.Listen(10);

            for(int i=0; i<10; i++)
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();

                // 비동기작업이 완료될 시 호출될 콜백함수(OnAcceptCompleted)를 등록
                args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

                RegisterAccpet(args); 
            }
        }

        void RegisterAccpet(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null; // args는 재사용되므로 연결소켓의 정보를 초기화 해줌

            // 클라이언트의 연결 요청을 대기
            // 요청이 바로들어온다면 args에 요청에 대한 정보를 저장하고 false를 리턴 

            // 클라이언트의 연결요청을 비동기적으로 대기 시작
            bool pending = _listenSocket.AcceptAsync(args);

            // System.Console.WriteLine($"[{args.GetHashCode()}]Waitng for client state : {pending}");
            
            // _listenSocket.AcceptAsync가 바로 처리된 경우 args.Completed가 실행되지 않으므로 직접 호출
            if (pending == false)
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
                Console.WriteLine(args.SocketError.ToString());

            RegisterAccpet(args);
        }
    }
}
