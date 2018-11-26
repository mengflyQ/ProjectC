﻿using System;
using ProtoBuf;

[Serializable, ProtoContract]
public class STCPackageHead
{
    [ProtoMember(1)]
    public int MsgId
    {
        get;
        set;
    }

    [ProtoMember(2)]
    public int ActionId
    {
        get;
        set;
    }

    [ProtoMember(3)]
    public int ErrorCode
    {
        get;
        set;
    }

    [ProtoMember(4)]
    public string ErrorInfo
    {
        get;
        set;
    }

    [ProtoMember(5)]
    public string St
    {
        get;
        set;
    }
}
