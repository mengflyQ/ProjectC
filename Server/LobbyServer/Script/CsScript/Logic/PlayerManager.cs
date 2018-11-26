using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Contract;
using GameServer.Model;

namespace GameServer.LobbyServer
{
    public class PlayerManager
    {
        public void AddPlayer(int uid, GameSession session, UserInfo userInfo)
        {
            SessionUser user = new SessionUser();
            user.UserId = uid;

            Player oldPlayer = null;
            if (mPlayersMap.TryGetValue(uid, out oldPlayer))
            {
                oldPlayer.OnReplace();
                mPlayers.Remove(oldPlayer);
                mPlayersMap.Remove(uid);
            }

            session.Bind(user);

            Player player = new Player();
            player.mUserID = uid;
            player.mSession = session;
            player.mUserInfo = userInfo;
            player.mStatus = PlayerStatus.Online;
        }

        static PlayerManager mInstance = null;
        public static PlayerManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new PlayerManager();
                return mInstance;
            }
        }

        List<Player> mPlayers = new List<Player>();
        Dictionary<int, Player> mPlayersMap = new Dictionary<int, Player>();
    }
}
