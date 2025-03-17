using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> _sessions = new List<ClientSession>();
        JobQueue _jobQueue = new JobQueue();
        List<ArraySegment<byte>> _pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            _jobQueue.Push(job);
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = $"{chat} I am {packet.playerId}";
            ArraySegment<byte> segment = packet.Write();

            // 보낼 패킷들을 나중에 한번에 보내기위해 리스트에 넣어둠
            _pendingList.Add(segment);

            //foreach (ClientSession s in _sessions)
            //    session.Send(segment);
        }

        // _pendingList의 값들을 한번에 처리
        public void Flush()
        {
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Enter(ClientSession session)
        {
            _sessions.Add(session);
            session.Room = this;
        }
        
        public void Leave(ClientSession session)
        {
            _sessions.Remove(session);
            
        }
    }
}
