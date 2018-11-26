using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;
using ProtoBuf;
using LitJson;

public class Action5002 : BaseAction
{
    public Action5002()
        : base((int)ActionType.KeyValuePackage)
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
        JsonData jsonData = actionParam.Get<JsonData>("json");
        int cts = actionParam.Get<int>("cts");
        writer.writeInt32("cts", cts);
        writer.writeWord("response", actionParam.Get<bool>("response") ? (ushort)1 : (ushort)0);
        writer.writeString("json", jsonData.ToJson());
    }

    protected override void DecodePackage(NetReader reader)
    {
        if (reader != null && reader.StatusCode == 0)
        {
            actionResult = new ActionResult();
            string json = reader.readString();
            actionResult["json"] = json;
        }
    }

    private ActionResult actionResult;
}