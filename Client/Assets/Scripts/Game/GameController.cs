using UnityEngine;
using ProtoBuf;
using System.Collections.Generic;
using ZyGames.Framework.Common.Serialization;

public class GameController
{
	public static void OnLoadScene()
	{
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;
        if (scn.mScnLists == null)
            return;
        if (scn.mScnLists.temp == 0)
            return;

        RectTransform canvas = UIRoot2D.Instance.GetComponent<RectTransform>();

		GameObject joystick = Resources.Load<GameObject>("GUI/UI_Joystick");
		if (joystick != null)
		{
			joystick = GameObject.Instantiate(joystick);
			RectTransform jt = joystick.GetComponent<RectTransform>();
			jt.parent = canvas;
            jt.localScale = new Vector3(2.2f, 2.2f, 2.2f);
        }

        NavigationSystem.OnEnterScene();

        NetWork.SetSendHeartbeatCallback(OnHeartbeatSend);

        NetWork.RegisterNotify(STC.STC_PlayerMove, OnPlayerMove);
        NetWork.RegisterNotify(STC.STC_TargetChg, OnTargetChg);
        NetWork.RegisterNotify(STC.STC_SkillNotify, SkillNotify);
        NetWork.RegisterNotify(STC.STC_SkillBegin, SkillBeginFunc);
        NetWork.RegisterNotify(STC.STC_AtbNotify, AtbNotify);
        NetWork.RegisterNotify(STC.STC_HPChg, OnChgHp);
        NetWork.RegisterNotify(STC.STC_SetPos, OnSetPos);
    }

    public static void OnPlayerInit(Player player)
    {
        mMainPlayer = player;

        mPlayerSync = new MainPlayerRecord(player);

        if (MobaMainCamera.MainCameraCtrl != null)
        {
            MobaMainCamera.MainCameraCtrl.target = player.transform;
        }
    }

    static void OnHeartbeatSend(object o)
    {
        mHeartbeatStartTime = mTime;
    }

    public static bool IsConntrller(Character cha)
    {
        return cha == mMainPlayer;
    }

    public static void LogicTick()
    {
        if (mPlayerSync != null)
        {
            mPlayerSync.LogicTick();
        }
        mTime = Time.realtimeSinceStartup;
    }

    static void OnPlayerMove(byte[] data)
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

    static void OnTargetChg(byte[] data)
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

    static void SkillNotify(byte[] data)
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

    static void SkillBeginFunc(byte[] data)
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

    static void AtbNotify(byte[] data)
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

    static void OnChgHp(byte[] data)
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

    static void OnSetPos(byte[] data)
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        if (scn == null)
            return;
        NotifySetPos msg = ProtoBufUtils.Deserialize<NotifySetPos>(data);
        Character cha = scn.GetCharacter(msg.uid);
        if (cha == null)
            return;
        cha.Position = msg.position.ToVector3();
        cha.Direction = msg.direction.ToVector3();
    }

    public static Player mMainPlayer = null;

    public static MainPlayerRecord mPlayerSync = null;

    public static UserInfo mUserInfo = new UserInfo();

    public static int mScnUID = -1;

    public static float mServerStartTime = 0.0f;
    public static float mClientStartTime = 0.0f;

    public static float mHeartbeatStartTime = 0.0f;
    public static float mTime = 0.0f;
    public static int mNetDelay = 0;
}