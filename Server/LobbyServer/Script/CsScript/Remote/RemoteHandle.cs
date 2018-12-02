using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.IO;
using LitJson;

namespace GameServer.LobbyServer
{
    public class RemoteHandle : RemoteStruct
    {
        public RemoteHandle(ActionGetter paramGtter, MessageStructure response)
            : base(paramGtter, response)
        {

        }

        protected override bool Check()
        {
            mResponseData = null;

            int sts = 0;
            string data = string.Empty;
            byte response = 0;

            if (!int.TryParse(paramGetter.RequestPackage.Params["sts"], out sts))
            {
                return false;
            }
            data = paramGetter.RequestPackage.Params["data"];
            if (!byte.TryParse(paramGetter.RequestPackage.Params["response"], out response))
            {
                return false;
            }

            STS eSTS = (STS)sts;
            Action<JsonData, RemoteHandle> action;
            if (!NetWork.mRegsterJsonSTS.TryGetValue(eSTS, out action))
            {
                return true;
            }

            if (action != null)
            {
                JsonData json = JsonMapper.ToObject(data);
                action(json, this);
            }
            if (response == 0)
            {
                mResponseData = null;
            }

            return true;
        }

        public void SetResponseData(JsonData data)
        {
            mResponseData = data;
        }

        protected override void TakeRemote()
        {
            return;
        }

        protected override void WriteResponse()
        {
            if (mResponseData == null)
                return;
            base.WriteResponse();
        }

        protected override void BuildPacket()
        {
            response.PushIntoStack(mResponseData.ToJson());
        }

        JsonData mResponseData = null;
    }
}
