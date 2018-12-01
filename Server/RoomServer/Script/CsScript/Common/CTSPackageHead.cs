﻿using System;
using ProtoBuf;

namespace GameServer.CommonLib
{
    [Serializable, ProtoContract]
    public class CTSPackageHead
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
        public string SessionId
        {
            get;
            set;
        }

        [ProtoMember(4)]
        public int UserId
        {
            get;
            set;
        }
    }
}
