using System;
using System.Collections.Generic;
using LitJson;

namespace GameServer.LoginServer
{
    public static class NetWork
    {
        public static void RegisterMessage(CTS cts, Action<byte[], Action5001> callBack)
        {
            if (mRegsterBytesCTS.ContainsKey(cts))
            {
                return;
            }
            mRegsterBytesCTS.Add(cts, callBack);
        }

        public static void RegisterMessage(CTS cts, Action<JsonData, Action5002> callBack)
        {
            if (mRegsterJsonCTS.ContainsKey(cts))
            {
                return;
            }
            mRegsterJsonCTS.Add(cts, callBack);
        }

        public static void UnregisterMessage(CTS cts)
        {
            mRegsterBytesCTS.Remove(cts);
            mRegsterJsonCTS.Remove(cts);
        }

        public static Dictionary<CTS, Action<byte[], Action5001>> mRegsterBytesCTS = new Dictionary<CTS, Action<byte[], Action5001>>();
        public static Dictionary<CTS, Action<JsonData, Action5002>> mRegsterJsonCTS = new Dictionary<CTS, Action<JsonData, Action5002>>();
    }
}
