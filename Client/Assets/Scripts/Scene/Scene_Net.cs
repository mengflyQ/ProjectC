using UnityEngine;
using System;
using System.Collections.Generic;
using ProtoBuf;
using ZyGames.Framework.Common.Serialization;

public partial class Scene
{
    void OnInitPlayers(byte[] data)
    {
        NotifyStartGame startGame = ProtoBufUtils.Deserialize<NotifyStartGame>(data);
        excel_scn_list scnList = SceneSystem.Instance.mCurrentScene.mScnLists;

        GameController.mServerStartTime = startGame.ServerStartTime;
        GameController.mClientStartTime = Time.realtimeSinceStartup;

        for (int i = 0; i < startGame.Players.Count; ++i)
        {
            ScnPlayerInfo playerInfo = startGame.Players[i];

            excel_cha_class chaClass = excel_cha_class.Find(mScnLists.temp);
            if (chaClass == null)
                continue;

            excel_cha_list chaList = excel_cha_list.Find(chaClass.chaListID);

            GameObject o = ResourceSystem.Load<GameObject>(chaList.path);
            if (o != null)
            {
                GameObject mainPlayer = GameObject.Instantiate(o);
                Player player = mainPlayer.GetComponent<Player>();
                player.gid = playerInfo.GID;
                player.UserID = playerInfo.UserID;
                player.mChaList = chaList;

                mainPlayer.transform.position = new Vector3(82.51f, 7.25f, 34.82f);
                mainPlayer.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);

                mPlayersList.Add(player);
                mCharacterList.Add(player);
                mPlayers.Add(player.gid, player);
                mCharacters.Add(player.gid, player);

                if (GameController.mUserInfo.uid == playerInfo.UserID)
                {
                    player.mEvent += TargetChgEvent;

                    MessageSystem.Instance.MsgDispatch(MessageType.OnSetChaClass, chaClass);

                    GameController.OnPlayerInit(player);
                }
            }

            //ResourceSystem.LoadAsync<GameObject>(chaList.path, (obj) =>
            //{
            //    GameObject o = obj as GameObject;
            //    if (o != null)
            //    {
            //        GameObject mainPlayer = GameObject.Instantiate(o);
            //        Player player = mainPlayer.GetComponent<Player>();
            //        player.gid = playerInfo.GID;
            //        player.UserID = playerInfo.UserID;
            //        player.mChaList = chaList;

            //        mainPlayer.transform.position = new Vector3(82.51f, 7.25f, 34.82f);
            //        mainPlayer.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);

            //        mPlayersList.Add(player);
            //        mCharacterList.Add(player);
            //        mPlayers.Add(player.gid, player);
            //        mCharacters.Add(player.gid, player);

            //        if (GameController.mUserInfo.uid == playerInfo.UserID)
            //        {
            //            player.mEvent += TargetChgEvent;

            //            MessageSystem.Instance.MsgDispatch(MessageType.OnSetChaClass, chaClass);

            //            GameController.OnPlayerInit(player);
            //        }
            //    }
            //});
        }
    }

    void TargetChgEvent(CharacterEventType evtType, Character self, params object[] datas)
    {
        if (evtType != CharacterEventType.OnTargetChg)
            return;
        TargetCircle.Instance.SetTarget(self.GetTarget());
    }

    void OnPlayerMove(byte[] data)
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;

        NotifyPlayerMove moveInfo = ProtoBufUtils.Deserialize<NotifyPlayerMove>(data);
        Player player = scn.GetPlayer(moveInfo.uid);
        if (player == null)
            return;
        player.SyncMove.AddMoveData(moveInfo.moveData);
    }

    void OnTargetChg(byte[] data)
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;
        ReqTargetChg targetChg = ProtoBufUtils.Deserialize<ReqTargetChg>(data);
        Character cha = scn.GetCharacter(targetChg.uid);
        if (cha == null)
            return;
        Character target = scn.GetCharacter(targetChg.targetID);

        cha.SetTarget(target, false);
    }

    void SkillNotify(byte[] data)
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;
        ReqSkill reqSkill = ProtoBufUtils.Deserialize<ReqSkill>(data);
        Character caster = scn.GetCharacter(reqSkill.casterID);
        if (caster == null)
            return;

        caster.Position = reqSkill.position.ToVector3();
        caster.Direction = reqSkill.direction.ToVector3();

        SkillHandle handle = new SkillHandle();
        handle.skillID = reqSkill.skillID;
        handle.caster = caster;
        handle.autoTargetPos = reqSkill.autoTargetPos;
        handle.targetPos = reqSkill.targetPos.ToVector3();
        handle.skillTargetID = reqSkill.targetID;
        SkillHandle.UseSkill(handle);
    }

    void SkillBeginFunc(byte[] data)
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;
        SkillBegin msg = ProtoBufUtils.Deserialize<SkillBegin>(data);
        Character cha = scn.GetCharacter(msg.uid);
        if (cha == null)
            return;
        Skill skill = cha.GetSkill();
        if (skill == null)
            return;
        if (skill.SkillID != msg.skillID || skill.mSkillState != SkillState.TrackEnemy)
        {
            return;
        }
        cha.Position = msg.position.ToVector3();
        cha.Direction = msg.direction.ToVector3();
        cha.StopMove();
        skill.BeginSkill();
    }

    void AtbNotify(byte[] data)
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;
        NotifyAtb msg = ProtoBufUtils.Deserialize<NotifyAtb>(data);
        Character cha = scn.GetCharacter(msg.uid);
        if (cha == null)
        {
            Character.AtbInitData[msg.uid] = msg;
            return;
        }
        cha.InitAtbFromMsg(msg);
    }

    void OnChgHp(byte[] data)
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;
        NotifyHPChg msg = ProtoBufUtils.Deserialize<NotifyHPChg>(data);
        Character cha = scn.GetCharacter(msg.uid);
        if (cha == null)
            return;
        HPChgType hurtType = (HPChgType)msg.chgType;
        cha.headBar.CreateHeadText(hurtType, msg.hp);
    }

    void OnSetPos(byte[] data)
    {
        NotifySetPos msg = ProtoBufUtils.Deserialize<NotifySetPos>(data);
        Character cha = GetCharacter(msg.uid);
        if (cha == null)
            return;
        cha.Position = msg.position.ToVector3();
        cha.Direction = msg.direction.ToVector3();
    }

    void NPCBorn(byte[] data)
    {
        ScnNPCInfo npcInfo = ProtoBufUtils.Deserialize<ScnNPCInfo>(data);

        excel_cha_list chaList = excel_cha_list.Find(npcInfo.chaListID);
        if (chaList == null)
            return;

        GameObject o = ResourceSystem.Load<GameObject>(chaList.path);
        if (o != null)
        {
            GameObject mainPlayer = GameObject.Instantiate(o);
            NPC npc = mainPlayer.GetComponent<NPC>();

            npc.gid = npcInfo.gid;
            npc.Position = npcInfo.position.ToVector3();
            npc.Direction = npcInfo.direction.ToVector3();
            npc.mChaList = chaList;

            npc.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);

            mNPCList.Add(npc);
            mCharacterList.Add(npc);
            mNPCs.Add(npc.gid, npc);
            mCharacters.Add(npc.gid, npc);
        }
    }

    void SearchMove(byte[] data)
    {
        SearchMoveMsg msg = ProtoBufUtils.Deserialize<SearchMoveMsg>(data);
        Character cha = GetCharacter(msg.gid);
        if (cha == null)
            return;
        cha.Position = msg.position.ToVector3();
        if (msg.moveType == (byte)SearchMoveMsg.MoveType.SearchMove)
        {
            cha.SearchMove(msg.targetPos.ToVector3(), msg.radius, false);
        }
        else if (msg.moveType == (byte)SearchMoveMsg.MoveType.LineMove)
        {
            cha.LineMove(msg.targetPos.ToVector3(), msg.radius, false);
        }
        else if (msg.moveType == (byte)SearchMoveMsg.MoveType.StopMove)
        {
            cha.StopMove();
        }
        else if (msg.moveType == (byte)SearchMoveMsg.MoveType.StopSearch)
        {
            cha.StopSearchMove();
        }
    }

    void OnDead(byte[] data)
    {
        DeadMsg msg = ProtoBufUtils.Deserialize<DeadMsg>(data);
        Character cha = GetCharacter(msg.gid);
        if (cha == null)
            return;
        cha.Position = msg.position.ToVector3();
        DeadType deadType = (DeadType)msg.deadType;

        cha.SetDead(deadType);
    }

    void InitializeNet()
    {
        NetWork.RegisterNotify(STC.STC_StartClienGame, OnInitPlayers);
        NetWork.RegisterNotify(STC.STC_PlayerMove, OnPlayerMove);
        NetWork.RegisterNotify(STC.STC_TargetChg, OnTargetChg);
        NetWork.RegisterNotify(STC.STC_SkillNotify, SkillNotify);
        NetWork.RegisterNotify(STC.STC_SkillBegin, SkillBeginFunc);
        NetWork.RegisterNotify(STC.STC_AtbNotify, AtbNotify);
        NetWork.RegisterNotify(STC.STC_HPChg, OnChgHp);
        NetWork.RegisterNotify(STC.STC_SetPos, OnSetPos);
        NetWork.RegisterNotify(STC.STC_RefreshNPC, NPCBorn);
        NetWork.RegisterNotify(STC.STC_SearchMove, SearchMove);
        NetWork.RegisterNotify(STC.STC_Dead, OnDead);
    }

    void UninitializeNet()
    {
        NetWork.UnregisterNotify(STC.STC_StartClienGame, OnInitPlayers);
        NetWork.UnregisterNotify(STC.STC_PlayerMove, OnPlayerMove);
        NetWork.UnregisterNotify(STC.STC_TargetChg, OnTargetChg);
        NetWork.UnregisterNotify(STC.STC_SkillNotify, SkillNotify);
        NetWork.UnregisterNotify(STC.STC_SkillBegin, SkillBeginFunc);
        NetWork.UnregisterNotify(STC.STC_AtbNotify, AtbNotify);
        NetWork.UnregisterNotify(STC.STC_HPChg, OnChgHp);
        NetWork.UnregisterNotify(STC.STC_SetPos, OnSetPos);
        NetWork.UnregisterNotify(STC.STC_RefreshNPC, NPCBorn);
        NetWork.UnregisterNotify(STC.STC_SearchMove, SearchMove);
        NetWork.UnregisterNotify(STC.STC_Dead, OnDead);
    }
}