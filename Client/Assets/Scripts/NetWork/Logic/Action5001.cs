using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;
using ProtoBuf;

public class Action5001 : BaseAction
{
    public Action5001()
        : base((int)ActionType.BytesPackage)
    {

    }

    public override ActionResult GetResponseData()
    {
        return actionResult;
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
        object obj = actionParam["obj"];
        int cts = actionParam.Get<int>("cts");
        byte[] response = new byte[1];
        response[0] = actionParam.Get<bool>("response") ? (byte)1 : (byte)0;
        byte[] ctsBytes = BitConverter.GetBytes(cts);
        byte[] data = ProtoBufUtils.Serialize(obj);
        byte[] bd = new byte[ctsBytes.Length + 1 + data.Length];
        ctsBytes.CopyTo(bd, 0);
        response.CopyTo(bd, ctsBytes.Length);
        data.CopyTo(bd, ctsBytes.Length + 1);
        
        writer.SetBodyData(bd);
    }

    protected override void DecodePackage(NetReader reader)
    {
        if (reader != null && reader.StatusCode == 0)
        {
            actionResult = new ActionResult();
            byte[] data = reader.readBytes();
            actionResult["data"] = data;
        }
    }

    private ActionResult actionResult;
}