using System;
using ProtoBuf;
using System.Collections.Generic;

[Serializable, ProtoContract]
public class LoginData
{
    [ProtoMember(1)]
    public int ScreenWidth;
    [ProtoMember(2)]
    public int ScreenHeight;
    [ProtoMember(3)]
    public string UserName;
    [ProtoMember(4)]
    public string Platform;
    [ProtoMember(5)]
    public string DeviceUniqueIdentifier;
    [ProtoMember(6)]
    public string DeviceModel;
    [ProtoMember(7)]
    public string DeviceTypeStr;
}

[Serializable, ProtoContract]
public class LoginResponse
{
    [ProtoMember(1)]
    public int UserID;
    [ProtoMember(2)]
    public string NickName;
}

[Serializable, ProtoContract]
public class RegistData
{
    [ProtoMember(1)]
    public string NickName;
}

[Serializable, ProtoContract]
public class EnterLobbyResponse
{
    [ProtoMember(1)]
    public int UserID;
    [ProtoMember(2)]
    public string NickName;
    [ProtoMember(3)]
    public int Level;
    [ProtoMember(4)]
    public int Exp;
    [ProtoMember(5)]
    public int Money;
    [ProtoMember(6)]
    public int VIPLevel;
}

[Serializable, ProtoContract]
public class ReqMatch
{
    [ProtoMember(1)]
    public int UserID;
}

[Serializable, ProtoContract]
public class RoomPlayerInfo
{
    [ProtoMember(1)]
    public string Name;

    [ProtoMember(2)]
    public byte[] Portrait;
}

[Serializable, ProtoContract]
public class NotifyRoomInfo
{
    [ProtoMember(1)]
    public List<RoomPlayerInfo> Players;
}

[Serializable, ProtoContract]
public class NotifyRoomAddPlayer
{
    [ProtoMember(1)]
    public RoomPlayerInfo NewPlayer;
}

[Serializable, ProtoContract]
public class NotifyMatch
{
    [ProtoMember(1)]
    public int Success;
}