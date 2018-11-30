using System;
using System.Collections.Generic;

namespace GameServer.LobbyServer
{
    public class Room
    {
        public Room(float time, int maxPlayerCount)
        {
            mPlayers = new List<Player>();
            mNotReady = new List<Player>();
            mMaxCount = maxPlayerCount;

            mStartTime = Time.ElapsedSeconds;
            mTime = time;

            IsVanish = false;
        }

        public bool AddPlayer(Player player)
        {
            if (mPlayers.Count >= mMaxCount)
                return true;

            mPlayers.Add(player);
            mNotReady.Add(player);
            
            NotifyRoomAddPlayer addPlayer = new NotifyRoomAddPlayer();
            addPlayer.NewPlayer = new RoomPlayerInfo();
            addPlayer.NewPlayer.Name = player.mPlayerInfo.NickName;
            addPlayer.NewPlayer.Portrait = null;

            NotifyRoomInfo roomInfo = new NotifyRoomInfo();
            roomInfo.Players = new List<RoomPlayerInfo>();
            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player p = mPlayers[i];
                RoomPlayerInfo playerInfo = new RoomPlayerInfo();
                playerInfo.Name = p.mPlayerInfo.NickName;
                playerInfo.Portrait = null;
                roomInfo.Players.Add(playerInfo);
            }

            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player p = mPlayers[i];
                if (p == player)
                {
                    NetWork.NotifyMessage<NotifyRoomInfo>(p.mUserID, STC.STC_RoomInfo, roomInfo);
                }
                else
                {
                    NetWork.NotifyMessage<NotifyRoomAddPlayer>(p.mUserID, STC.STC_RoomAddPlayer, addPlayer);
                }
            }

            return mPlayers.Count >= mMaxCount;
        }

        public void ReadyPlayer(Player player)
        {
            int index = mNotReady.IndexOf(player);
            if (index < 0)
                return;
            mNotReady.RemoveAt(index);
        }

        public void Tick()
        {
            if (IsVanish)
                return;
            float life = Time.ElapsedSeconds - mStartTime;
            if (life >= mTime)
            {
                IsVanish = true;
                OnTimeOver();
            }
        }

        private void OnTimeOver()
        {
            NotifyMatch result = new NotifyMatch();
            if (mNotReady.Count > 0)
            {
                result.Success = 0;
            }
            else
            {
                result.Success = 1;
            }
            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player player = mPlayers[i];
                NetWork.NotifyMessage<NotifyMatch>(player.mUserID, STC.STC_MatchFailed, result);
            }
        }

        public bool IsVanish
        {
            private set;
            get;
        }

        int mMaxCount = 0;
        List<Player> mPlayers = null;
        List<Player> mNotReady = null;

        float mStartTime;
        float mTime;
    }
}
