using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;

public class Action1002 : GameAction
{
    public Action1002() : base((int)ActionType.LoginOver)
    {
    }

    public override ActionResult GetResponseData()
    {
        return null;
    }

    protected override void DecodePackage(NetReader reader)
    {
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {

    }
}
