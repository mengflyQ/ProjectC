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
            return true;
        }

        public override void BuildPacket()
        {
        }

        int mUserID = -1;
    }
}
