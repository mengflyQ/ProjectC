using System;
using ZyGames.Framework.Game.Contract.Action;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Cache.Generic;
using GameServer.Model;

namespace GameServer.LoginServer
{
    public class Action1003 : AuthorizeAction
    {
        public Action1003(ActionGetter httpGet)
            : base((int)ActionType.Regist, httpGet)
        {
        }

        public override bool GetUrlElement()
        {
            string nickName = string.Empty;
            if (!httpGet.GetString("NickName", ref nickName))
            {
                return false;
            }
            mUserID = actionGetter.GetSession().UserId;
            ShareCacheStruct<UserInfo> cache = new ShareCacheStruct<UserInfo>();
            mUserInfo = cache.FindKey(mUserID);
            if (mUserInfo == null)
                return false;
            mUserInfo.ModifyLocked(() =>
            {
                mUserInfo.NickName = nickName;
            });

            return true;
        }

        public override bool TakeAction()
        {
            return true;
        }

        public override void BuildPacket()
        {
            PushIntoStack(0);
            PushIntoStack(UserId);

            PushIntoStack(mUserInfo.NickName);
            PushIntoStack(mUserInfo.Level);
            PushIntoStack(mUserInfo.Exp);
            PushIntoStack(mUserInfo.Money);
            PushIntoStack(mUserInfo.VIPLevel);
        }

        private int mUserID;
        private UserInfo mUserInfo;
    }
}
