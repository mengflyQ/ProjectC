using System;
using System.Collections.Generic;
using ZyGames.Framework.Common.Serialization;

namespace GameServer.RoomServer
{
    public class Scene
    {
        public Scene(int scnID, int maxPlayerCount)
        {
            PlayerMaxCount = maxPlayerCount;

            mStartTime = Time.ElapsedSeconds;

            IsVanish = false;

            ScnID = scnID;
            ScnUID = SceneManager.Instance.GenSceneID();
        }

        public void AddPlayer(Player player)
        {
            player.mScene = this;

            mPlayersList.Add(player);
            mCharactersList.Add(player);

            mPlayers.Add(player.UID, player);
            mCharacters.Add(player.UID, player);
        }

        public Player FindPlayer(int uid)
        {
            Player player = null;
            if (mPlayers.TryGetValue(uid, out player))
            {
                return player;
            }
            return null;
        }

        public int GetPlayerCount()
        {
            return mPlayersList.Count;
        }

        // 当所有角色加载场景完毕，通知客户端开始游戏;
        public void StartClientGame()
        {
            NotifyStartGame startGame = new NotifyStartGame();
            startGame.Players = new List<ScnPlayerInfo>();

            for (int i = 0; i < mPlayersList.Count; ++i)
            {
                Player player = mPlayersList[i];

                ScnPlayerInfo playerInfo = new ScnPlayerInfo();
                playerInfo.Name = player.mNickName;
                playerInfo.UserID = player.UID;
                startGame.Players.Add(playerInfo);
            }

            for (int i = 0; i < mPlayersList.Count; ++i)
            {
                Player player = mPlayersList[i];

                NetWork.NotifyMessage<NotifyStartGame>(player.UID, STC.STC_StartClienGame, startGame);
            }
        }

        public void Tick()
        {
            if (IsVanish)
                return;
            for (int i = 0; i < mCharactersList.Count; ++i)
            {
                Character cha = mCharactersList[i];
                if (cha == null)
                {
                    Console.WriteLine("Error: Character List Contains Empty Character!");
                    continue;
                }
                cha.Update();
            }
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

        Dictionary<int, Character> mCharacters = new Dictionary<int, Character>();
        Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();

        List<Character> mCharactersList = new List<Character>();
        List<Player> mPlayersList = new List<Player>();

        float mStartTime;
    }
}
