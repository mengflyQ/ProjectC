using System;
using LitJson;
using GameServer.Model;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;
using ZyGames.Framework.Cache.Generic;

namespace GameServer.LobbyServer
{
    public class NetWorkRegister
    {
        static void OnEnterLobby(byte[] data, Action5001 action)
        {
            LoginResponse userInfoData = null;
            userInfoData = ProtoBufUtils.Deserialize<LoginResponse>(data);

            GameSession session = action.GetActionGetter().GetSession();

            string key = string.Format("{0}", userInfoData.UserID);
            PlayerInfo playerInfo = CacheSet.PlayerInfoCache.FindKey(key);

            if (session == null)
            {
                return;
            }
            if (playerInfo == null)
            {
                playerInfo = new PlayerInfo();
                playerInfo.UserId = userInfoData.UserID;
                playerInfo.NickName = userInfoData.NickName;
                CacheSet.PlayerInfoCache.Add(playerInfo);
            }
            PlayerManager.Instance.AddPlayer(userInfoData.UserID, session, playerInfo);

            EnterLobbyResponse enterLobby = new EnterLobbyResponse();
            enterLobby.UserID = playerInfo.UserId;
            enterLobby.NickName = playerInfo.NickName;
            enterLobby.Level = playerInfo.Level;
            enterLobby.Exp = playerInfo.Exp;
            enterLobby.Money = playerInfo.Money;
            byte[] responseData = ProtoBufUtils.Serialize(enterLobby);
            action.SetResponseData(responseData);
        }

        static void OnMatch(byte[] data, Action5001 action)
        {
            ReqMatch reqMatch = ProtoBufUtils.Deserialize<ReqMatch>(data);
            Player player = PlayerManager.Instance.FindPlayer(reqMatch.UserID);
            if (player == null)
                return;

            RoomManager.Instance.Match(player);
        }

        static void OnMatchReady(byte[] data, Action5001 action)
        {
            int uid = action.UserId;

            Player player = PlayerManager.Instance.FindPlayer(uid);
            if (player == null)
                return;

            if (player.mRoom == null)
                return;

            player.mRoom.ReadyPlayer(player);
        }

        public static void Initialize()
        {
            NetWork.RegisterMessage(CTS.CTS_EnterLobby, OnEnterLobby);
            NetWork.RegisterMessage(CTS.CTS_Match, OnMatch);
            NetWork.RegisterMessage(CTS.CTS_MatchReady, OnMatchReady);
        }

        public static void Uninitialize()
        {
            NetWork.UnregisterMessage(CTS.CTS_Login);
            NetWork.UnregisterMessage(CTS.CTS_Match);
        }
    }
}