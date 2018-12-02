using System;
using System.Collections.Generic;
using LitJson;

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

            ID = RoomManager.Instance.GenRoomID();
        }

        public bool AddPlayer(Player player)
        {
            if (mPlayers.Count >= mMaxCount)
                return true;

            mPlayers.Add(player);
            mNotReady.Add(player);
            player.mRoom = this;
            
            NotifyRoomAddPlayer addPlayer = new NotifyRoomAddPlayer();
            addPlayer.NewPlayer = new RoomPlayerInfo();
            addPlayer.NewPlayer.UserID = player.mUserID;
            addPlayer.NewPlayer.Name = player.mPlayerInfo.NickName;
            addPlayer.NewPlayer.Portrait = null;

            NotifyRoomInfo roomInfo = new NotifyRoomInfo();
            roomInfo.Players = new List<RoomPlayerInfo>();
            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player p = mPlayers[i];
                RoomPlayerInfo playerInfo = new RoomPlayerInfo();
                playerInfo.UserID = p.mUserID;
                playerInfo.Name = p.mPlayerInfo.NickName;
                playerInfo.Portrait = null;
                roomInfo.Players.Add(playerInfo);
            }
            float life = Time.ElapsedSeconds - mStartTime;
            roomInfo.RestTime = mTime - life;

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

            ReqMatch msg = new ReqMatch();
            msg.UserID = player.mUserID;

            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player p = mPlayers[i];
                NetWork.NotifyMessage<ReqMatch>(p.mUserID, STC.STC_MatchReady, msg);
            }
        }

        public void Tick()
        {
            if (IsVanish)
                return;
            if (mTime > 0.0f)
            {
                float life = Time.ElapsedSeconds - mStartTime;
                if (life >= mTime)
                {
                    OnTimeOver();
                }
            }
        }

        private void OnTimeOver()
        {
            NotifyMatch result = new NotifyMatch();
            if (mNotReady.Count > 0 || mPlayers.Count < mMaxCount)
            {
                result.Success = 0;
                IsVanish = true;
            }
            else
            {
                result.Success = 1;
                mTime = -1;

                JsonData json = new JsonData();
                json["maxCount"] = mMaxCount;
                NetWork.SendToServer("RoomServer", STS.STS_CreateScn, json, OnCreateScnSuccess);
            }
            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player player = mPlayers[i];
                player.mRoom = null;
                NetWork.NotifyMessage<NotifyMatch>(player.mUserID, STC.STC_MatchFailed, result);
            }
        }

        void OnCreateScnSuccess(JsonData json)
        {
            int roomID = json["scnID"].AsInt;
            NotifySceneReady roomReady = new NotifySceneReady();
            roomReady.SceneID = roomID;
            for (int i = 0; i < mPlayers.Count; ++i)
            {
                Player player = mPlayers[i];
                NetWork.NotifyMessage<NotifySceneReady>(player.mUserID, STC.STC_SceneReady, roomReady);
            }
        }

        public bool IsVanish
        {
            private set;
            get;
        }

        public int ID
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
