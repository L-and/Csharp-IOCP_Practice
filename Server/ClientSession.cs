using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    class ClientSession : PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected : {endPoint}");


            //Packet packet = new Packet() { size = 4, packetId = 10 };

            //ArraySegment<byte> openSegment = SendBufferHelper.Open(4096); // 0~4096바이트까지 snedBuffer 쓸듯?
            //                                                              // openSegment에 바이트 복사
            //byte[] buffer = BitConverter.GetBytes(packet.size);
            //byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            //Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            //Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);

            //ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length); // 실제로는 buffer + buffer2 바이트 만큼 썻음

            //Send(sendBuff); // 그래서 open한거말고 close하고 리턴된걸 사용하는거
            Thread.Sleep(1000);

            Disconnect();
        }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId: {id}, Size: {size}");
        }

        public override void OnDisConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected : {endPoint}");
        }

        public override void OnSend(int numOfByte)
        {
            Console.WriteLine($"Transferred bytes : {numOfByte}");
        }
    }
}
