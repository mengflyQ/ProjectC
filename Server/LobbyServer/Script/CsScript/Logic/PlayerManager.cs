using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Contract;
using GameServer.Model;

namespace GameServer.LobbyServer
{
    public class PlayerManager
    {
        public void AddPlayer(int uid, GameSession session, PlayerInfo userInfo)
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
            player.mPlayerInfo = userInfo;
            player.mStatus = PlayerStatus.Online;

            mPlayers.Add(player);
            mPlayersMap[uid] = player;
        }

        public Player FindPlayer(int uid)
        {
            Player player;
            if (!mPlayersMap.TryGetValue(uid, out player))
            {
                return null;
            }
            return player;
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
