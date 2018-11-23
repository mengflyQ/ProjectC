using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Cache.Generic;

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

            return true;
        }

        public override bool TakeAction()
        {
            ShareCacheStruct<UserInfo> cache = new ShareCacheStruct<UserInfo>();
            UserInfo userInfo = cache.FindKey(mUserName);
            if (userInfo == null)
            {
                userInfo = new UserInfo();
                userInfo.Account = mUserName;
                userInfo.Password = "";
                userInfo.RegisterTime = DateTime.Now;
                userInfo.LastLoginTime = DateTime.Now;
                if (!cache.Add(userInfo))
                {
                    Console.WriteLine("Regist UserInfo Failed!");
                }
            }

            return true;
        }

        public override void BuildPacket()
        {
            PushIntoStack(mUserName);
            PushIntoStack(mDeviceID);
            PushIntoStack(this.UserId);
        }

        int mScreenX;
        int mScreenY;
        string mUserName;
        string mDeviceID;
    }
}
