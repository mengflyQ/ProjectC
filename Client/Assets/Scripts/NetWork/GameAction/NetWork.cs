using System;
using LitJson;
using UnityEngine;
using System.Collections.Generic;

public static class NetWork
{
    public static void SendPacket<T>(CTS cts, T obj, Action<byte[]> callback)
    {
        ActionParam param = new ActionParam();
        param["obj"] = obj;
        param["cts"] = (int)cts;
        param["response"] = (callback != null);
        Net.Instance.Send((int)ActionType.BytesPackage, (result) => {
            if (result == null)
                return;
            byte[] data = result.Get<byte[]>("data");
            if (callback != null)
            {
                callback(data);
            }
        }, param);
    }

    public static void SendJsonPacket(CTS cts, JsonData json, Action<JsonData> callback)
    {
        ActionParam param = new ActionParam();
        param["json"] = json;
        param["cts"] = (int)cts;
        param["response"] = (callback != null);
        Net.Instance.Send((int)ActionType.BytesPackage, (result) =>
        {
            if (result == null)
                return;
            string data = result.Get<string>("json");
            if (callback != null)
            {
                callback(JsonMapper.ToObject(data));
            }
        }, param);
    }

    public static void RegisterNotify(STC stc, Action<byte[]> callback)
    {
        Action<byte[]> action;
        if (mRegsterBytesSTC.TryGetValue(stc, out action))
        {
            action += callback;
            return;
        }
        mRegsterBytesSTC.Add(stc, callback);
    }

    public static void SetUrl(string url)
    {
        Net.Instance.CloseSocket();
        NetWriter.SetUrl(url);
    }

    public static Dictionary<STC, Action<byte[]>> mRegsterBytesSTC = new Dictionary<STC, Action<byte[]>>();
}