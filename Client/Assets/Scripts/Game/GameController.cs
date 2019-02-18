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