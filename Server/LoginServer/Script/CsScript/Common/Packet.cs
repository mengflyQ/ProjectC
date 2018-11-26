using System;
using ProtoBuf;

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
    public string NickName;
    [ProtoMember(2)]
    public int Level;
    [ProtoMember(3)]
    public int Exp;
    [ProtoMember(4)]
    public int Money;
    [ProtoMember(5)]
    public int VIPLevel;
    [ProtoMember(6)]
    public int UserID;
}

[Serializable, ProtoContract]
public class RegistData
{
    [ProtoMember(1)]
    public string NickName;
}