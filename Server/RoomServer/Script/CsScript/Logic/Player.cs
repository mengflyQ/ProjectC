using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;

namespace GameServer.RoomServer
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
        public string mNickName;
        public GameSession mSession;
        public Scene mScene = null;

        public PlayerStatus mStatus;
    }
}
