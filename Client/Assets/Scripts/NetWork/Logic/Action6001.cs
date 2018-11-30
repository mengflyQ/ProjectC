using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;
using ProtoBuf;

public class Action6001 : BaseAction
{
    public Action6001()
        : base((int)ActionType.OnNotify)
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
            actionResult = new ActionResult();
            byte[] data = reader.Buffer;
            actionResult["data"] = data;

            if (data.Length > 4)
            {
                int bodyLen = data.Length - 4;
                byte[] stc = new byte[4];
                byte[] body = new byte[bodyLen];
                Buffer.BlockCopy(data, 0, stc, 0, 4);
                Buffer.BlockCopy(data, 4, body, 0, bodyLen);
                STC eStc = (STC)BitConverter.ToInt32(stc, 0);

                Action<byte[]> callback;
                if (NetWork.mRegsterBytesSTC.TryGetValue(eStc, out callback))
                {
                    callback(body);
                }
            }
        }
    }

    private ActionResult actionResult;
}