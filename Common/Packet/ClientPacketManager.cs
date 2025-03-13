using System;
using System.Collections.Generic;
using ServerCore;

class PacketManager
{
    #region Singleton
    static PacketManager _instance;
    public static PacketManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PacketManager();
            return _instance;
        }
    }
    #endregion

    // PacketId의 패킷은 어떤 Action을 할지 저장하는 딕셔너리
    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>>>();
    Dictionary<ushort, Action<PacketSession, IPacket>> _handler = new Dictionary<ushort, Action<PacketSession, IPacket>>();


    // _onRecv에 packetId마다 action을 등록
    public void Register()
    {
      _onRecv.Add((ushort)PacketID.S_Test, MakePacket<S_Test>);
        _handler.Add((ushort)PacketID.S_Test, PacketHandler.S_TestHandler);

    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort pktId = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>> action = null;
        if (_onRecv.TryGetValue(pktId, out action))
            action.Invoke(session, buffer);
    }

    // 제네릭 T의 패킷을 조립하는 함수 (T는 IPakcet을 구현하는 놈 이고, new()로 인스턴스 생성이 가능해야 함)
    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer) where T : IPacket, new()
    {
        T pkt = new T();
        pkt.Read(buffer);

        Action<PacketSession, IPacket> action = null;
        if (_handler.TryGetValue(pkt.Protocol, out action))
            action.Invoke(session, pkt);
    }
}