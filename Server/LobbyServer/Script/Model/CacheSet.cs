﻿using ZyGames.Framework.Cache.Generic;
using GameServer.Model;

namespace GameServer.Model
{
    public class CacheSet
    {
        public static PersonalCacheStruct<PlayerInfo> PlayerInfoCache = new PersonalCacheStruct<PlayerInfo>();
    }
}
