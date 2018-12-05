using System;
using LitJson;
using GameServer.Model;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Cache.Generic;
using ZyGames.Framework.Common.Serialization;

namespace GameServer.RoomServer
{
    public class NetWorkRegister
    {
        // 客户端角色开始加载场景;
        static void OnEnterScene(byte[] data, Action5001 action)
        {
            ReqEnterScene enterScn = ProtoBufUtils.Deserialize<ReqEnterScene>(data);
            GameSession session = action.GetActionGetter().GetSession();
            if (session == null)
            {
                return;
            }

            Player player = PlayerLoadingManager.Instance.AddPlayer(enterScn.UserID, session);
            if (player != null)
            {
                player.mNickName = enterScn.NickName;
            }
        }

        // 客户端角色加载场景完成;
        static void OnPlayerLoadedScn(byte[] data, Action5001 action)
        {
            ReqLoadedScn loadedScn = ProtoBufUtils.Deserialize<ReqLoadedScn>(data);
            Scene scn = SceneManager.Instance.FindScene(loadedScn.ScnUID);
            if (scn == null)
                return;
            Player player = PlayerLoadingManager.Instance.FindPlayer(action.UserId);
            if (player == null)
                return;
            int maxPlayerCount = scn.GetPlayerCount();
            if (maxPlayerCount >= scn.PlayerMaxCount)
            {
                return;
            }
            scn.AddPlayer(player);
            PlayerLoadingManager.Instance.RemovePlayer(player);

            maxPlayerCount = scn.GetPlayerCount();
            if (maxPlayerCount >= scn.PlayerMaxCount)
            {
                scn.StartClientGame();
            }
        }

        // Lobby服务器通知创建场景;
        static void CreateScene(JsonData json, RemoteHandle handle)
        {
            int maxCount = json["maxCount"].AsInt;
            int scnID = json["scnID"].AsInt;
            Scene scene = new Scene(scnID, maxCount);
            SceneManager.Instance.mScenes.Add(scene);

            JsonData responseData = new JsonData();
            responseData["scnID"] = scene.ScnUID;
            handle.SetResponseData(responseData);
        }

        public static void Initialize()
        {
            NetWork.RegisterMessage(CTS.CTS_EnterScn, OnEnterScene);
            NetWork.RegisterMessage(CTS.CTS_LoadedScn, OnPlayerLoadedScn);

            NetWork.RegisterRemote(STS.STS_CreateScn, CreateScene);
        }

        public static void Uninitialize()
        {
            NetWork.UnregisterMessage(CTS.CTS_EnterScn);
        }
    }
}