using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;

public class Action10001 : BaseAction
{
    public Action10001()
        : base((int)ActionType.LoginAction)
    {

    }

    public override ActionResult GetResponseData()
    {
        if (actionResult == null)
            throw new Exception("Error: actionResult is null!");
        return actionResult;
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
    }

    protected override void DecodePackage(NetReader reader)
    {
        if (reader != null && reader.StatusCode == 0)
        {
            //byte[] data = reader.Buffer;
            //LoginData loginData = ProtoBufUtils.Deserialize<LoginData>(data);
            //actionResult = new ActionResult(loginData);
        }
    }

    private ActionResult actionResult;
}
