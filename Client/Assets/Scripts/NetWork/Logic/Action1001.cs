using UnityEngine;
using System.Collections;
using System;
using GameRanking.Pack;
using ZyGames.Framework.Common.Serialization;

public class Action1001 : GameAction
{
    public Action1001() : base((int)ActionType.Login)
	{
		
	}

	public override ActionResult GetResponseData()
	{
        return actionResult;
	}

	// 服务器返回的内容，按照服务器Push的顺序返回相应值;
	protected override void DecodePackage (NetReader reader)
	{
		if (reader != null && reader.StatusCode == 0)
		{
            actionResult = new ActionResult();
			Debug.Log("Login Success " + reader.readString());
		}
	}

	protected override void SendParameter(NetWriter writer, ActionParam actionParam)
	{
        writer.writeInt32("ScreenX", GameApp.Instance.ScreenWidth);
        writer.writeInt32("ScreenY", GameApp.Instance.ScreenHeight);
        writer.writeString("UserName", GameApp.Instance.UserName);
        writer.writeString("Password", GameApp.Instance.Password);
	}

    private ActionResult actionResult;
}