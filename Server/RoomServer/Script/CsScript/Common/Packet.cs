using System;
using ProtoBuf;
using System.Collections.Generic;

[Serializable, ProtoContract]
public class ReqEnterRoom
{
    [ProtoMember(1)]
    public int UserID;
    [ProtoMember(2)]
    public string NickName;
}