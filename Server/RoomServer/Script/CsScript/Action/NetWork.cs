using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Common.Serialization;
using GameServer.CommonLib;
using ProtoBuf;
using LitJson;
using ZyGames.Framework.RPC.Sockets;

namespace GameServer.RoomServer
{
    public static class NetWork
    {
        public static void RegisterMessage(CTS cts, Action<byte[], Action5001> callBack)
        {
            Action<byte[], Action5001> action;
            if (mRegsterBytesCTS.TryGetValue(cts, out action))
            {
                action += callBack;
                return;
            }
            mRegsterBytesCTS.Add(cts, callBack);
        }

        public static void RegisterMessage(CTS cts, Action<JsonData, Action5002> callBack)
        {
            Action<JsonData, Action5002> action;
            if (mRegsterJsonCTS.TryGetValue(cts, out action))
            {
                action += callBack;
                return;
            }
            mRegsterJsonCTS.Add(cts, callBack);
        }

        public static void UnregisterMessage(CTS cts)
        {
            mRegsterBytesCTS.Remove(cts);
            mRegsterJsonCTS.Remove(cts);
        }

        public static void NotifyMessage<T>(int uid, STC stc, T obj)
        {
            byte[] head = new byte[5 * sizeof(int)];
            byte[] actionId = BitConverter.GetBytes((int)ActionType.OnNotify);
            Buffer.BlockCopy(actionId, 0, head, 12, sizeof(int));

            byte[] stcBytes = BitConverter.GetBytes((int)stc);
            byte[] body = ProtoBufUtils.Serialize(obj);

            byte[] buffer = new byte[sizeof(int) + head.Length + stcBytes.Length + body.Length];

            byte[] streamLenBytes = BitConverter.GetBytes(buffer.Length);

            int pos = 0;
            Buffer.BlockCopy(streamLenBytes, 0, buffer, pos, streamLenBytes.Length);
            pos += streamLenBytes.Length;
            Buffer.BlockCopy(head, 0, buffer, pos, head.Length);
            pos += head.Length;
            Buffer.BlockCopy(stcBytes, 0, buffer, pos, stcBytes.Length);
            pos += stcBytes.Length;
            Buffer.BlockCopy(body, 0, buffer, pos, body.Length);

            GameSession session = GameSession.Get(uid);

            session.SendAsync(OpCode.Binary, buffer, 0, buffer.Length, asyncResult =>
            {
                Console.WriteLine("Push Action -> {0} STC -> {2} result is -> {1}", (int)ActionType.OnNotify, asyncResult.Result == ResultCode.Success ? "ok" : "fail", stc.ToString());
            });
        }

        public static Dictionary<CTS, Action<byte[], Action5001>> mRegsterBytesCTS = new Dictionary<CTS, Action<byte[], Action5001>>();
        public static Dictionary<CTS, Action<JsonData, Action5002>> mRegsterJsonCTS = new Dictionary<CTS, Action<JsonData, Action5002>>();

        internal static void NotifyMessage<T1>(int p, STC sTC)
        {
            throw new NotImplementedException();
        }
    }
}
