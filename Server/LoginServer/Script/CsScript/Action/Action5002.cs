using System;
using ZyGames.Framework.Game.Contract.Action;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;
using LitJson;

namespace GameServer.LoginServer
{
    public class Action5002 : AuthorizeAction
    {
        public Action5002(ActionGetter httpGet)
            : base((int)ActionType.KeyValuePackage, httpGet)
        {
        }

        public override bool GetUrlElement()
        {
            mResponseData = null;

            CTS cts = CTS.CTS_Default;
            int iCTS = 0;
            string jsonStr = string.Empty;
            ushort response = 0;
            if (!actionGetter.GetInt("cts", ref iCTS)
                || !actionGetter.GetString("json", ref jsonStr)
                || !actionGetter.GetWord("response", ref response))
                return false;
            cts = (CTS)iCTS;
            JsonData json = JsonMapper.ToObject(jsonStr);

            Action<JsonData, Action5002> callback;
            if (!NetWork.mRegsterJsonCTS.TryGetValue(cts, out callback))
            {
                return false;
            }
            callback(json, this);

            bool rst = response > 0;

            if (mResponseData == null)
                return false;

            return rst;
        }

        public void SetResponseData(JsonData data)
        {
            mResponseData = data;
        }

        public override bool TakeAction()
        {
            return true;
        }

        public override void BuildPacket()
        {
            PushIntoStack(mResponseData.ToJson());
        }

        JsonData mResponseData = null;
    }
}