using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;

namespace GameServer.LobbyServer
{
    public enum PlayerStatus
    {
        Online,
        Offline
    }

    public class Player
    {
        public void OnReplace()
        {
            if (mSession != null)
            {
                mSession.Close();
            }
        }

        public int mUserID;
        public GameSession mSession;
        public PlayerInfo mPlayerInfo;
        public Room mRoom = null;

        public PlayerStatus mStatus;
    }
}
