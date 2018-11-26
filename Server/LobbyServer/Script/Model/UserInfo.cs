using System;
using ProtoBuf;
using ZyGames.Framework.Model;
using System.Collections.Generic;

namespace GameServer.Model
{
    [Serializable, ProtoContract]
    [EntityTable(CacheType.Entity, "ZGame", "UserInfo")]
    public class UserInfo : ShareEntity
    {
        public UserInfo()
            : base(false)
        {
            LastLoginTime = DateTime.Now;
            RegisterTime = DateTime.Now;

            NickName = string.Empty;
            Level = 1;
            Exp = 0;
            Money = 0;
            VIPLevel = 0;
        }

        [ProtoMember(1)]
        [EntityField(true)]
        public uint UserId { get; set; }

        [ProtoMember(2)]
        [EntityField(IsUnique = true)]
        public string Account { get; set; }

        [ProtoMember(3)]
        [EntityField(ColumnLength = 24)]
        public string Platform { get; set; }

        [ProtoMember(4)]
        [EntityField(IsUnique = true)]
        public string NickName { get; set; }

        [ProtoMember(5)]
        [EntityField]
        public int Token { get; set; }

        [ProtoMember(6)]
        [EntityField(ColumnLength = 128)]
        public string DeviceID { get; set; }

        [ProtoMember(7)]
        [EntityField(ColumnLength = 128)]
        public string DeviceModel { get; set; }

        [ProtoMember(8)]
        [EntityField(ColumnLength = 24)]
        public string DeviceType { get; set; }

        [ProtoMember(9)]
        [EntityField]
        public int Level { get; set; }

        [ProtoMember(10)]
        [EntityField]
        public int Exp { get; set; }

        [ProtoMember(11)]
        [EntityField]
        public int Money { get; set; }

        [ProtoMember(12)]
        [EntityField]
        public int VIPLevel { get; set; }

        [ProtoMember(13)]
        [EntityField]
        public DateTime RegisterTime { get; set; }

        [ProtoMember(14)]
        [EntityField]
        public DateTime LastLoginTime { get; set; }
    }
}