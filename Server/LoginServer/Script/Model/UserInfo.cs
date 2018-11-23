using System;
using System.Collections.Generic;
using ProtoBuf;
using ZyGames.Framework.Model;

namespace GameServer.CsScript.LoginServer
{
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Entity, "UserInfo")]
    public class UserInfo : ShareEntity
    {
        [ProtoMember(1)]
        [EntityField(true)]
        public string Account { get; set; }

        [ProtoMember(2)]
        [EntityField]
        public string Password { get; set; }

        [ProtoMember(3)]
        [EntityField]
        public uint UserId { get; set; }

        [ProtoMember(4)]
        [EntityField]
        public string Token { get; set; }

        [ProtoMember(5)]
        [EntityField]
        public DateTime RegisterTime { get; set; }

        [ProtoMember(6)]
        [EntityField]
        public DateTime LastLoginTime { get; set; }
    }
}