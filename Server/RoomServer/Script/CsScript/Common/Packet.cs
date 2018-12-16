using System;
using MathLib;
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

[Serializable, ProtoContract]
public class Vector3Packat
{
    [ProtoMember(1)]
    public float x;
    [ProtoMember(2)]
    public float y;
    [ProtoMember(3)]
    public float z;

    public Vector3 ToVector3()
    {
        Vector3 v = new Vector3();
        v.x = x;
        v.y = y;
        v.z = z;
        return v;
    }

    public static Vector3Packat FromVector3(Vector3 v)
    {
        Vector3Packat packet = new Vector3Packat();
        packet.x = v.x;
        packet.y = v.y;
        packet.z = v.z;
        return packet;
    }
}

[Serializable, ProtoContract]
public class ReqPlayerMove
{
    [ProtoMember(1)]
    public float speed;
    [ProtoMember(2)]
    public Vector3Packat direction;
    [ProtoMember(3)]
    public Vector3Packat position;
    [ProtoMember(4)]
    public float timestamp;
    [ProtoMember(5)]
    public float timespan;
}

[Serializable, ProtoContract]
public class NotifyPlayerMove
{
    [ProtoMember(1)]
    public int uid;
    [ProtoMember(2)]
    public ReqPlayerMove moveData;
}