using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Sns;

namespace GameServer.CsScript.LoginServer
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
            PushIntoStack(mPassport);
            PushIntoStack(mPassword);
        }

        int mScreenX;
        int mScreenY;
        string mUserName;
        string mPassport;
        string mPassword;
        string mDeviceID;
    }
}
