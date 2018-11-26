using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.RPC.IO;

namespace GameServer.GateServer
{
    public abstract class GateAction : BaseStruct
    {
        public GateAction(int actionID, ActionGetter httpGet)
            : base(actionID, httpGet)
        {

        }

        public void SetTcpProxy(string poxyId, string ip, int port, int heartInterval)
        {
            if (ServerSvice == null)
            {
                ServerSvice = RemoteService.CreateTcpProxy("proxy_gate_login", "127.0.0.1", 9002, 10 * 1000);
            }
        }

        public abstract string GetRoutPath();

        public abstract bool SetRequestParam(RequestParam param);

        void OnRemotePackage(RemotePackage result)
        {
            ServerResultBytes = result.Message as byte[];
            ServerResult = new MessageStructure(ServerResultBytes);

            if (IsWebSocket)
            {
                JsonWriteResponse(Response);
            }
            else
            {
                BinaryWriteResponse(Response);
            }
        }

        public override bool CheckAction()
        {
            var param = new RequestParam();
            if (!SetRequestParam(param))
            {
                return false;
            }
            string routPath = GetRoutPath();
            ServerSvice.Call(routPath, param, OnRemotePackage);
            return true;
        }

        public override void WriteResponse(BaseGameResponse response)
        {
            Response = response;
        }

        public RemoteService ServerSvice
        {
            private set;
            get;
        }

        public BaseGameResponse Response
        {
            private set;
            get;
        }

        protected MessageStructure ServerResult
        {
            private set;
            get;
        }

        protected byte[] ServerResultBytes
        {
            private set;
            get;
        }
    }
}
