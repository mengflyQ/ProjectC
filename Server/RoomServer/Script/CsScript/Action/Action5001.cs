using System;
using ZyGames.Framework.Game.Contract.Action;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;

namespace GameServer.RoomServer
{
    public class Action5001 : AuthorizeAction
    {
        public Action5001(ActionGetter httpGet)
            : base((int)ActionType.BytesPackage, httpGet)
        {
        }

        public override bool GetUrlElement()
        {
            mResponseData = null;

            byte[] data = (byte[])actionGetter.GetMessage();
            byte[] ctsBytes = new byte[4];
            int len = data.Length - ctsBytes.Length - 1;
            byte[] d = new byte[len];
            byte[] response = new byte[1];

            int pos = 0;
            Buffer.BlockCopy(data, pos, ctsBytes, 0, ctsBytes.Length);
            pos += ctsBytes.Length;
            Buffer.BlockCopy(data, pos, response, 0, 1);
            pos += 1;
            Buffer.BlockCopy(data, pos, d, 0, d.Length);

            CTS cts = (CTS)BitConverter.ToInt32(ctsBytes, 0);
            bool rst = response[0] > 0 ? true : false;

            Action<byte[], Action5001> callback;
            if (!NetWork.mRegsterBytesCTS.TryGetValue(cts, out callback))
            {
                return true;
            }
            callback(d, this);

            if (mResponseData == null)
                return true;
            if (!rst)
                mResponseData = null;

            return rst;
        }

        public void SetResponseData(byte[] data)
        {
            mResponseData = data;
        }

        public ActionGetter GetActionGetter()
        {
            return actionGetter;
        }

        public override bool TakeAction()
        {
            return true;
        }

        public override void WriteResponse(BaseGameResponse response)
        {
            if (mResponseData == null)
                return;
            base.WriteResponse(response);
        }

        public override void BuildPacket()
        {
            if (mResponseData == null)
                return;
            PushIntoStack(mResponseData);
            mResponseData = null;
        }

        byte[] mResponseData = null;
    }
}