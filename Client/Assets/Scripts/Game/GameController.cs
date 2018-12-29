using UnityEngine;
using ProtoBuf;
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

        excel_cha_class chaClass = excel_cha_class.Find(scn.mScnLists.temp);
        excel_cha_list chaList = excel_cha_list.Find(chaClass.chaListID);

		GameObject o = Resources.Load<GameObject>(chaList.path);
		if (o != null)
		{
			GameObject mainPlayer = GameObject.Instantiate(o);
			mMainPlayer = mainPlayer.GetComponent<Player>();
            mMainPlayer.UserID = mUserInfo.uid;
            mMainPlayer.mChaList = chaList;
            mMainPlayer.mChaClass = chaClass;
            mMainPlayer.mEvent += TargetChgEvent;

            MessageSystem.Instance.MsgDispatch(MessageType.OnSetChaClass, chaClass);

            mPlayerSync = new MainPlayerRecord(mMainPlayer);

            mainPlayer.transform.position = new Vector3(81.51f, 7.25f, 34.82f);
			mainPlayer.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);

			if (MobaMainCamera.MainCameraCtrl != null)
			{
				MobaMainCamera.MainCameraCtrl.target = mainPlayer.transform;
			}

            scn.mPlayersList.Add(mMainPlayer);
            scn.mCharacterList.Add(mMainPlayer);
            scn.mPlayers.Add(mUserInfo.uid, mMainPlayer);
            scn.mCharacters.Add(mUserInfo.uid, mMainPlayer);
        }

		RectTransform canvas = GameObject.Find("Canvas").GetComponent<RectTransform>();

		GameObject joystick = Resources.Load<GameObject>("GUI/UI_Joystick");
		if (joystick != null)
		{
			joystick = GameObject.Instantiate(joystick);
			RectTransform jt = joystick.GetComponent<RectTransform>();
			jt.parent = canvas;
		}

        NavigationSystem.OnEnterScene();

        NetWork.RegisterNotify(STC.STC_PlayerMove, OnPlayerMove);
        NetWork.RegisterNotify(STC.STC_TargetChg, OnTargetChg);
        NetWork.RegisterNotify(STC.STC_SkillNotify, SkillNotify);
        NetWork.RegisterNotify(STC.STC_SkillBegin, SkillBeginFunc);
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
    }

    static void TargetChgEvent(CharacterEventType evtType, Character self)
    {
        if (evtType != CharacterEventType.OnTargetChg)
            return;
        TargetCircle.Instance.SetTarget(self.GetTarget());
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
        skill.BeginSkill();
    }

    public static Player mMainPlayer = null;

    public static MainPlayerRecord mPlayerSync = null;

    public static UserInfo mUserInfo = new UserInfo();

    public static int mScnUID = -1;
}