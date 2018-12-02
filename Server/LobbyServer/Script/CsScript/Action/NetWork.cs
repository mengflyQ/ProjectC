using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Common.Serialization;
using GameServer.CommonLib;
using ProtoBuf;
using LitJson;
using ZyGames.Framework.RPC.IO;
using ZyGames.Framework.RPC.Sockets;
using ZyGames.Framework.Game.Service;

namespace GameServer.LobbyServer
{
    public class ServerService
    {
        public string IP
        {
            set;
            get;
        }

        public int Port
        {
            set;
            get;
        }

        public RemoteService Service
        {
            set;
            get;
        }

        public int HeartBeatInterval
        {
            set;
            get;
        }
    }

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

        public static void InitialAllServers()
        {
            InitialServerService("LoginServer", "127.0.0.1", 9002, 10 * 1000);
            //InitialServerService("LobbyServer", "127.0.0.1", 9003, 10 * 1000);
            InitialServerService("RoomServer", "127.0.0.1", 9004, 10 * 1000);
        }

        public static void InitialServerService(string serverProxy, string ip, int port, int heartBeatInterval)
        {
            if (mServerServices.ContainsKey(serverProxy))
            {
                return;
            }
            ServerService service = new ServerService();
            service.IP = ip;
            service.Port = port;
            service.HeartBeatInterval = heartBeatInterval;
            service.Service = RemoteService.CreateTcpProxy(serverProxy, ip, port, heartBeatInterval);
            mServerServices.Add(serverProxy, service);
        }

        public static void SendToServer(string serverProxy, STS sts, JsonData json, Action<JsonData> callback)
        {
            ServerService service = null;
            if (!mServerServices.TryGetValue(serverProxy, out service))
            {
                return;
            }

            RequestParam param = new RequestParam();
            param["sts"] = (int)sts;
            param["data"] = json.ToJson();
            param["response"] = (byte)((callback != null) ? 1 : 0);
            service.Service.Call("RemoteHandle", param, (result) =>
            {
                var reader = new MessageStructure(result.Message as byte[]);
                string jsonStr = reader.ReadString();
                JsonData jsonData = JsonMapper.ToObject(jsonStr);
                if (callback != null)
                {
                    callback(jsonData);
                }
            });
        }

        public static void RegisterRemote(STS sts, Action<JsonData, RemoteHandle> callBack)
        {
            Action<JsonData, RemoteHandle> action;
            if (mRegsterJsonSTS.TryGetValue(sts, out action))
            {
                action += callBack;
                return;
            }
            mRegsterJsonSTS.Add(sts, callBack);
        }

        public static void UnregisterRemote(STS sts)
        {
            mRegsterJsonSTS.Remove(sts);
        }

        public static Dictionary<CTS, Action<byte[], Action5001>> mRegsterBytesCTS = new Dictionary<CTS, Action<byte[], Action5001>>();
        public static Dictionary<CTS, Action<JsonData, Action5002>> mRegsterJsonCTS = new Dictionary<CTS, Action<JsonData, Action5002>>();
        public static Dictionary<STS, Action<JsonData, RemoteHandle>> mRegsterJsonSTS = new Dictionary<STS, Action<JsonData, RemoteHandle>>();
        private static Dictionary<string, ServerService> mServerServices = new Dictionary<string, ServerService>();

        internal static void NotifyMessage<T1>(int p, STC sTC)
        {
            throw new NotImplementedException();
        }
    }
}
