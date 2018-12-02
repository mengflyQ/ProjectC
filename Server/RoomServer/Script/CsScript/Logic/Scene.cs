using System;
using System.Collections.Generic;
using ZyGames.Framework.Common.Serialization;

namespace GameServer.RoomServer
{
    public class Scene
    {
        public Scene(int maxCount)
        {
            mPlayers = new List<Player>();
            PlayerMaxCount = maxCount;

            mStartTime = Time.ElapsedSeconds;

            IsVanish = false;

            ScnUID = SceneManager.Instance.GenSceneID();
        }

        public void AddPlayer(Player player)
        {
            player.mScene = this;
            mPlayers.Add(player);
        }

        public int GetPlayerCount()
        {
            return mPlayers.Count;
        }

        public void StartClientGame()
        {
            NotifyStartGame startGame = new NotifyStartGame();
            startGame.Players = new List<ScnPlayerInfo>();

            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player player = mPlayers[i];

                ScnPlayerInfo playerInfo = new ScnPlayerInfo();
                playerInfo.Name = player.mNickName;
                playerInfo.UserID = player.mUserID;
                startGame.Players.Add(playerInfo);
            }

            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player player = mPlayers[i];

                NetWork.NotifyMessage<NotifyStartGame>(player.mUserID, STC.STC_StartClienGame, startGame);
            }
        }

        public void Tick()
        {
            if (IsVanish)
                return;
        }

        public bool IsVanish
        {
            private set;
            get;
        }

        public int ScnUID
        {
            private set;
            get;
        }

        public int ScnID
        {
            set;
            get;
        }

        public int PlayerMaxCount
        {
            private set;
            get;
        }

        List<Player> mPlayers = null;

        float mStartTime;
    }
}
