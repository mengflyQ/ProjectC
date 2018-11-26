using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;

public class Action1001 : GameAction
{
    public Action1001() : base((int)ActionType.Login)
	{
		
	}

	public override ActionResult GetResponseData()
	{
        if (actionResult == null)
            throw new Exception("Error: actionResult is null!");
        return actionResult;
	}

	protected override void DecodePackage (NetReader reader)
	{
		if (reader != null && reader.StatusCode == 0)
		{
            actionResult = new ActionResult();

            bool regist = reader.getInt() > 0;
            actionResult["Regist"] = regist;
            actionResult["UserID"] = reader.getInt();
            if (!regist)
            {
                actionResult["NickName"] = reader.readString();
                actionResult["Level"] = reader.getInt();
                actionResult["Exp"] = reader.getInt();
                actionResult["Money"] = reader.getInt();
                actionResult["VIPLevel"] = reader.getInt();
            }
		}
	}

	protected override void SendParameter(NetWriter writer, ActionParam actionParam)
	{
        writer.writeInt32("ScreenX", GameApp.Instance.ScreenWidth);
        writer.writeInt32("ScreenY", GameApp.Instance.ScreenHeight);
        writer.writeString("UserName", GameApp.Instance.UserName);
        writer.writeString("Platform", GameApp.Instance.Platform);
        writer.writeString("DeviceID", GameApp.Instance.DeviceUniqueIdentifier);
        writer.writeString("DeviceModel", GameApp.Instance.DeviceModel);
        writer.writeString("DeviceType", GameApp.Instance.DeviceTypeStr);
	}

    private ActionResult actionResult;
}