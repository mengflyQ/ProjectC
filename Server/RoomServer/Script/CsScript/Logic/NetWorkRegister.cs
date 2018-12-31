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
                player.mChaClass = excel_cha_class.Find(enterScn.ClassID);
                if (player.mChaClass == null)
                {
                    Debug.LogError("未找到ID为{0}的职业表", enterScn.ClassID);
                    return;
                }
                player.mChaList = excel_cha_list.Find(player.mChaClass.chaListID);
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

        static void TargetChg(byte[] data, Action5001 action)
        {
            ReqTargetChg targetChg = ProtoBufUtils.Deserialize<ReqTargetChg>(data);
            Character cha = SceneManager.Instance.FindCharacter(targetChg.uid);
            if (cha == null)
                return;
            Character target = cha.mScene.FindCharacter(targetChg.targetID);
            if (target == null)
                return;
            cha.SetTarget(target, false);
        }

        static void RequestSkill(byte[] data, Action5001 action)
        {
            ReqSkill req = ProtoBufUtils.Deserialize<ReqSkill>(data);
            Character cha = SceneManager.Instance.FindCharacter(action.UserId);
            if (cha == null)
                return;
            SkillHandle handle = new SkillHandle();
            handle.skillID = req.skillID;
            handle.caster = cha;
            handle.skillTargetID = req.targetID;
            handle.autoTargetPos = req.autoTargetPos;
            handle.targetPos = req.targetPos.ToVector3();
            SkillHandle.UseSkill(handle);
        }

        static void SkillBeginFunc(byte[] data, Action5001 action)
        {
            Player cha = SceneManager.Instance.FindPlayer(action.UserId);
            if (cha == null)
                return;
            SkillBegin msg = ProtoBufUtils.Deserialize<SkillBegin>(data);
            Skill skill = cha.GetSkill();
            if (skill == null)
                return;
            if (skill.SkillID != msg.skillID || skill.mSkillState != SkillState.TrackEnemy)
            {
                cha.SetSkill(null);
                return;
            }
            cha.Position = msg.position.ToVector3();
            cha.Direction = msg.direction.ToVector3();
            skill.BeginSkill();
        }

        public static void Initialize()
        {
            NetWork.RegisterMessage(CTS.CTS_EnterScn, OnEnterScene);
            NetWork.RegisterMessage(CTS.CTS_LoadedScn, OnPlayerLoadedScn);
            NetWork.RegisterMessage(CTS.CTS_TargetChg, TargetChg);
            NetWork.RegisterMessage(CTS.CTS_SkillReq, RequestSkill);
            NetWork.RegisterMessage(CTS.CTS_SkillBegin, SkillBeginFunc);

            NetWork.RegisterRemote(STS.STS_CreateScn, CreateScene);
        }

        public static void Uninitialize()
        {
            NetWork.UnregisterMessage(CTS.CTS_EnterScn);
        }
    }
}