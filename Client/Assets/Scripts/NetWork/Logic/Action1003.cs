using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;

public class Action1003 : GameAction
{
    public Action1003()
        : base((int)ActionType.Regist)
    {

    }

    public override ActionResult GetResponseData()
    {
        if (actionResult == null)
            throw new Exception("Error: actionResult is null!");
        return actionResult;
    }

    protected override void DecodePackage(NetReader reader)
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
        writer.writeString("NickName", actionParam.Get<string>("NickName"));
    }

    private ActionResult actionResult;
}