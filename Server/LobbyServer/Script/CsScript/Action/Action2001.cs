using System;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.Game.Lang;
using GameServer.Model;

namespace GameServer.LobbyServer
{
    public class Action2001 : BaseStruct
    {
        public Action2001(ActionGetter httpGet)
            : base((int)ActionType.EnterLobby, httpGet)
        {
        }

        public override bool GetUrlElement()
        {
            if (!httpGet.GetInt("UserID", ref mUserID))
            {
                return false;
            }
            return true;
        }

        public override bool TakeAction()
        {
            GameSession session = httpGet.GetSession();
            mUserInfo = CacheSet.UserInfoCach.FindKey(mUserID);
            if (session != null || mUserInfo != null)
            {
                ErrorCode = (int)CodeType.EnterLobbyError;
                ErrorInfo = "EnterLobby Error!";
                return false;
            }
            PlayerManager.Instance.AddPlayer(mUserID, session, mUserInfo);

            return true;
        }

        public override void BuildPacket()
        {
            if (mUserInfo == null)
                return;
            PushIntoStack(mUserInfo.NickName);
            PushIntoStack(mUserInfo.Level);
            PushIntoStack(mUserInfo.Exp);
            PushIntoStack(mUserInfo.Money);
            PushIntoStack(mUserInfo.VIPLevel);
        }

        UserInfo mUserInfo = null;
        int mUserID = -1;
    }
}
