using UnityEngine;
using System.Collections;
using System;
using ZyGames.Framework.Common.Serialization;
using ProtoBuf;

public class Action6002 : BaseAction
{
    public Action6002()
        : base((int)ActionType.Heartbeat)
    {

    }

    public override ActionResult GetResponseData()
    {
        return null;
    }

    protected override void SendParameter(NetWriter writer, ActionParam actionParam)
    {
    }

    protected override void DecodePackage(NetReader reader)
    {
        if (reader != null && reader.StatusCode == 0)
        {
            byte[] data = reader.Buffer;

            int floatSize = sizeof(float);
            byte[] svrTimeDatas = new byte[floatSize];
            Buffer.BlockCopy(data, 0, svrTimeDatas, 0, floatSize);
            float svrTime = BitConverter.ToSingle(svrTimeDatas, 0);

            float delay = Time.realtimeSinceStartup - GameController.mHeartbeatStartTime;

            float svrTimeElapsed = svrTime - GameController.mServerStartTime;
            GameController.mClientStartTime = Time.realtimeSinceStartup - svrTimeElapsed;
            GameController.mNetDelay = Mathf.FloorToInt(delay / 0.03333f * 1000.0f);
        }
    }
}