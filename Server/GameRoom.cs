﻿using ServerCore;
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

        // _pendingList의 값들을 한번에 처리
        public void Flush()
        {
            foreach (ClientSession s in _sessions)
                s.Send(_pendingList);

            //Console.WriteLine($"Flushed {_pendingList.Count} items");
            _pendingList.Clear();
        }

        public void Broadcast(ArraySegment<byte> segment)
        {
            _pendingList.Add(segment);
        }

        public void Enter(ClientSession session)
        {
            // 플레이어를 세션리스트에 추가한다
            _sessions.Add(session);
            session.Room = this;

            // 새 플레이어에게 모든 플레이어 목록 전송
            S_PlayerList players = new S_PlayerList();
            foreach (ClientSession s in _sessions)
            {
                players.players.Add(new S_PlayerList.Player()
                {
                    isSelf = (s == session),
                    playerId = s.SessionId,
                    posX = s.PosX,
                    posY = s.PosY,
                    posZ = s.PosZ,
                });
            }
            session.Send(players.Write());

            // 새 플레이어가 입장했음을 모두에게 알림
            S_BroadcastEnterGame enter = new S_BroadcastEnterGame();
            enter.playerId = session.SessionId;
            // 접속 시 기본 위치
            enter.posX = 0;
            enter.posY = 0;
            enter.posZ = 0;
            Broadcast(enter.Write());
        }
        
        public void Leave(ClientSession session)
        {
            // 플레이어를 세션리스트에서 제거
            _sessions.Remove(session);

            // 모두에게 알림
            S_BroadcastLeaveGame leave = new S_BroadcastLeaveGame();
            leave.playerId = session.SessionId;
            Broadcast(leave.Write());
        }

        public void Move(ClientSession session, C_Move packet)
        {
            // 좌표 바꿔주고
            session.PosX = packet.posX;
            session.PosY = packet.posY;
            session.PosZ = packet.posZ;

            // 모두에게 알림
            S_BroadcastMove move = new S_BroadcastMove();
            move.playerId = session.SessionId;
            move.posX = session.PosX;
            move.posY = session.PosY;
            move.posZ = session.PosZ;
            Broadcast(move.Write());
        }
    }
}
