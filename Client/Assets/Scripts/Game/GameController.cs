using UnityEngine;

public class GameController
{
	public static void OnLoadScene()
	{
		excel_cha_list chaList = excel_cha_list.Find(1);

		GameObject o = Resources.Load<GameObject>(chaList.path);
		if (o != null)
		{
            Debug.LogError("------------");
			GameObject mainPlayer = GameObject.Instantiate(o);
			mMainPlayer = mainPlayer.GetComponent<Character>();
            mMainPlayer.mChaList = chaList;

			mainPlayer.transform.position = new Vector3(81.51f, 7.25f, 34.82f);
			mainPlayer.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);

			if (MobaMainCamera.MainCameraCtrl != null)
			{
				MobaMainCamera.MainCameraCtrl.target = mainPlayer.transform;
			}
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
	}

	public static Character mMainPlayer = null;

    public static UserInfo mUserInfo = new UserInfo();
}