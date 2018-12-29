﻿using System;
using ProtoBuf;
using System.Collections.Generic;
using UnityEngine;

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
public class EmptyMsg
{

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
    public int UserID;

    [ProtoMember(3)]
    public byte[] Portrait;
}

[Serializable, ProtoContract]
public class NotifyRoomInfo
{
    [ProtoMember(1)]
    public List<RoomPlayerInfo> Players;

    [ProtoMember(2)]
    public float RestTime;
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

[Serializable, ProtoContract]
public class NotifySceneReady
{
    [ProtoMember(1)]
    public int SceneID;
}

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
    public bool control;
    [ProtoMember(5)]
    public float timestamp;
    [ProtoMember(6)]
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