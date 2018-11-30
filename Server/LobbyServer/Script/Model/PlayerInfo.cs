using System;
using ProtoBuf;
using ZyGames.Framework.Model;
using System.Collections.Generic;

namespace GameServer.Model
{
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Dictionary, "ZGame", "PlayerInfo")]
    public class PlayerInfo : BaseEntity
    {
        public PlayerInfo()
            : base(false)
        {
            Level = 1;
            Exp = 0;
            Money = 0;
            VIPLevel = 0;
        }

        [ProtoMember(1)]
        [EntityField(true)]
        public int UserId { get; set; }

        [ProtoMember(2)]
        [EntityField(IsUnique = true)]
        public string NickName { get; set; }

        [ProtoMember(3)]
        [EntityField]
        public int Level { get; set; }

        [ProtoMember(4)]
        [EntityField]
        public int Exp { get; set; }

        [ProtoMember(5)]
        [EntityField]
        public int Money { get; set; }

        [ProtoMember(6)]
        [EntityField]
        public int VIPLevel { get; set; }

        protected override int GetIdentityId()
        {
            return UserId;
        }
    }
}