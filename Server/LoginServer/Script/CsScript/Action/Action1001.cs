using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;

namespace GameServer.LoginServer
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
            mPlatform = httpGet.GetString("Platform");
            mDeviceID = httpGet.GetString("DeviceID");
            mDeviceModel = httpGet.GetString("DeviceModel");
            mDeviceType = httpGet.GetString("DeviceType");

            return true;
        }

        public override bool TakeAction()
        {
            ShareCacheStruct<UserInfo> cache = new ShareCacheStruct<UserInfo>();

            var listUser = cache.FindAll(t => t.Account == mUserName);
            UserInfo userInfo = null;
            if (listUser.Count >= 1)
                userInfo = listUser[0];

            if (userInfo == null)
            {
                userInfo = new UserInfo();
                userInfo.UserId = (uint)cache.GetNextNo();
                userInfo.Account = mUserName;
                userInfo.Platform = mPlatform;
                userInfo.DeviceID = mDeviceID;
                userInfo.DeviceModel = mDeviceModel;
                userInfo.DeviceType = mDeviceType;
                userInfo.RegisterTime = DateTime.Now;
                userInfo.LastLoginTime = DateTime.Now;
                userInfo.Token = 1;
                if (!cache.Add(userInfo))
                {
                    Console.WriteLine("Regist UserInfo Failed!");
                }
                mRegist = true;
            }
            else
            {
                mNickName = userInfo.NickName;
                mLevel = userInfo.Level;
                mExp = userInfo.Exp;
                mMoney = userInfo.Money;
                mVIPLevel = userInfo.VIPLevel;

                userInfo.ModifyLocked(() => {
                    userInfo.Account = mUserName;
                    userInfo.Platform = mPlatform;
                    userInfo.DeviceID = mDeviceID;
                    userInfo.DeviceModel = mDeviceModel;
                    userInfo.DeviceType = mDeviceType;
                    userInfo.LastLoginTime = DateTime.Now;
                    userInfo.Token = userInfo.Token + 1;
                });
                mRegist = false;
                if (string.IsNullOrEmpty(mNickName))
                {
                    mRegist = true;
                }
            }
            int uid = (int)userInfo.UserId;

            SessionUser user = new SessionUser();
            user.UserId = uid;
            GameSession session = httpGet.GetSession();

            var OldSession = GameSession.Get(uid);
            if (OldSession != null)
            {
                OldSession.Close();
            }

            session.Bind(user);

            return true;
        }

        public override void BuildPacket()
        {
            PushIntoStack(mRegist ? 1 : 0);
            PushIntoStack(UserId);
            if (!mRegist)
            {
                PushIntoStack(mNickName);
                PushIntoStack(mLevel);
                PushIntoStack(mExp);
                PushIntoStack(mMoney);
                PushIntoStack(mVIPLevel);
            }
        }

        int mScreenX;
        int mScreenY;
        string mUserName;
        string mPlatform;
        string mDeviceID;
        string mDeviceModel;
        string mDeviceType;
        bool mRegist;
        string mNickName;
        int mLevel;
        int mExp;
        int mMoney;
        int mVIPLevel;
    }
}
