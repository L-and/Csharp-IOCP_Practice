using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class ServerSession : Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");

            for (int i = 0; i < 5; i++)
            {
                // 데이터 송신
                Packet packet = new Packet() { size = 4, packetId = 7 };

                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096); // 0~4096바이트까지 snedBuffer 쓸듯?
                                                                              // openSegment에 바이트 복사
                byte[] buffer = BitConverter.GetBytes(packet.size);
                byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
                Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
                Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

                ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); // 실제로는 buffer + buffer2 바이트 만큼 썻음

                Send(sendBuff); // 그래서 open한거말고 close하고 리턴된걸 사용하는거
            }
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");

            return buffer.Count;
        }

        public override void OnSend(int numOfByte)
        {
            Console.WriteLine($"Transferred bytes : {numOfByte}");
        }
    }
}
