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

        excel_cha_list chaList = excel_cha_list.Find(scn.mScnLists.temp);

		GameObject o = Resources.Load<GameObject>(chaList.path);
		if (o != null)
		{
			GameObject mainPlayer = GameObject.Instantiate(o);
			mMainPlayer = mainPlayer.GetComponent<Player>();
            mMainPlayer.ID = mUserInfo.uid;
            mMainPlayer.mChaList = chaList;

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
	}

    public static void LogicTick()
    {
        if (mPlayerSync != null)
        {
            mPlayerSync.LogicTick();
        }
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


    public static Player mMainPlayer = null;

    public static MainPlayerRecord mPlayerSync = null;

    public static UserInfo mUserInfo = new UserInfo();

    public static int mScnUID = -1;
}