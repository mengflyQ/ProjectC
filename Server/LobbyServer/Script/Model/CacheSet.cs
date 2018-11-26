using ZyGames.Framework.Cache.Generic;
using GameServer.Model;

namespace GameServer.LobbyServer
{
    public class CacheSet
    {
        public static ShareCacheStruct<UserInfo> UserInfoCach = new ShareCacheStruct<UserInfo>();
    }
}
