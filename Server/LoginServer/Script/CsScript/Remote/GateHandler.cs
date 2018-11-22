using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.RPC.IO;

namespace GameServer.CsScript.LoginServer
{
    class GateHandler : RemoteStruct
    {
        string mUserName = string.Empty;
        bool mSuccess = false;

        public GateHandler(ActionGetter paramGtter, MessageStructure response)
            : base(paramGtter, response)
        {

        }

        protected override bool Check()
        {
            if (paramGetter.GetString("UserName", ref mUserName))
            {
                return true;
            }
            return false;
        }

        protected override void TakeRemote()
        {
            var seesion = paramGetter.GetSession();
            if (seesion == null)
            {
                mSuccess = false;
                return;
            }
            mSuccess = true;
            return;
        }

        protected override void BuildPacket()
        {
            response.PushIntoStack(mSuccess);
        }
    }
}
