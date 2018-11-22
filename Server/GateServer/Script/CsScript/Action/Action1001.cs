using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
// using ZyGames.Framework.Game.Sns;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.RPC.IO;

namespace GameServer.CsScript.Action
{
    public class Action1001 : BaseStruct
    {
        public Action1001(ActionGetter httpGet)
            : base((int)ActionType.Login, httpGet)
        {
        }

        public override bool GetUrlElement()
        {
            mScreenX = httpGet.GetInt("ScreenX");
            mScreenY = httpGet.GetInt("ScreenY");
            mUserName = httpGet.GetString("UserName");
            mDeviceID = httpGet.GetString("DeviceID");

            // string[] userList = SnsManager.GetRegPassport(mDeviceID);
            mPassport = "1";    // userList[0];
            mPassword = "123";  // userList[1];

            return true;
        }

        public override bool TakeAction()
        {
            return true;
        }

        public override void BuildPacket()
        {
            if (mLoginRemote == null)
            {
                mLoginRemote = RemoteService.CreateTcpProxy("proxy_gate_login", "127.0.0.1", 9002, 10 * 1000);
            }

            var param = new RequestParam();
            param["UserName"] = mUserName;
            mLoginRemote.Call("GateHandler", param, OnLogin);
        }

        void OnLogin(RemotePackage result)
        {
            var reader = new MessageStructure(result.Message as byte[]);
            bool success = reader.ReadBool();
            if (success)
            {
                PushIntoStack(mPassport);
                PushIntoStack(mPassword);
            }
        }

        int mScreenX;
        int mScreenY;
        string mUserName;
        string mPassport;
        string mPassword;
        string mDeviceID;

        private RemoteService mLoginRemote = null;
    }
}
