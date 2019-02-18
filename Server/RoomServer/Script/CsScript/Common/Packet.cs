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
    [ProtoMember(3)]
    public int ClassID;
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

    [ProtoMember(3)]
    public int GID;
}

[Serializable, ProtoContract]
public class NotifyStartGame
{
    [ProtoMember(1)]
    public List<ScnPlayerInfo> Players;
    [ProtoMember(2)]
    public float ServerStartTime;
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
    public int gid;
    [ProtoMember(2)]
    public float speed;
    [ProtoMember(3)]
    public Vector3Packat direction;
    [ProtoMember(4)]
    public Vector3Packat position;
    [ProtoMember(5)]
    public bool control;
    [ProtoMember(6)]
    public float timestamp;
    [ProtoMember(7)]
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

[Serializable, ProtoContract]
public class ReqTargetChg
{
    [ProtoMember(1)]
    public int uid;
    [ProtoMember(2)]
    public int targetID;
}

[Serializable, ProtoContract]
public class ReqSkill
{
    [ProtoMember(1)]
    public int skillID;
    [ProtoMember(2)]
    public int casterID;
    [ProtoMember(3)]
    public Vector3Packat direction;
    [ProtoMember(4)]
    public Vector3Packat position;
    [ProtoMember(5)]
    public bool autoTargetPos;
    [ProtoMember(6)]
    public Vector3Packat targetPos;
    [ProtoMember(7)]
    public int targetID;
}

[Serializable, ProtoContract]
public class SkillBegin
{
    [ProtoMember(1)]
    public int uid;
    [ProtoMember(2)]
    public int skillID;
    [ProtoMember(3)]
    public Vector3Packat direction;
    [ProtoMember(4)]
    public Vector3Packat position;
}

[Serializable, ProtoContract]
public class NotifyAtb
{
    [ProtoMember(1)]
    public int uid;
    [ProtoMember(2)]
    public List<int> atbTypes = new List<int>();
    [ProtoMember(3)]
    public List<int> atbValues = new List<int>();
}

[Serializable, ProtoContract]
public class NotifyHPChg
{
    [ProtoMember(1)]
    public int uid;
    [ProtoMember(2)]
    public int hp;
    [ProtoMember(3)]
    public HPChgType chgType;
}

[Serializable, ProtoContract]
public class NotifySetPos
{
    [ProtoMember(1)]
    public int uid;
    [ProtoMember(2)]
    public Vector3Packat position;
    [ProtoMember(3)]
    public Vector3Packat direction;
}

[Serializable, ProtoContract]
public class ScnNPCInfo
{
    [ProtoMember(1)]
    public string name;
    [ProtoMember(2)]
    public int chaListID;
    [ProtoMember(3)]
    public int gid;
    [ProtoMember(4)]
    public Vector3Packat position;
    [ProtoMember(5)]
    public Vector3Packat direction;
}