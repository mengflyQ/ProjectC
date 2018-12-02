using System;
using ProtoBuf;
using System.Collections.Generic;

[Serializable, ProtoContract]
public class ReqEnterScene
{
    [ProtoMember(1)]
    public int UserID;
    [ProtoMember(2)]
    public string NickName;
}

[Serializable, ProtoContract]
public class ReqLoadedScn
{
    [ProtoMember(1)]
    public int ScnUID;
    [ProtoMember(2)]
    public int ScnID;
}

[Serializable, ProtoContract]
public class ScnPlayerInfo
{
    [ProtoMember(1)]
    public string Name;

    [ProtoMember(2)]
    public int UserID;
}

[Serializable, ProtoContract]
public class NotifyStartGame
{
    [ProtoMember(1)]
    public List<ScnPlayerInfo> Players;
}