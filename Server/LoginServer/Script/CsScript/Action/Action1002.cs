using System;
using ZyGames.Framework.Game.Contract.Action;
using ZyGames.Framework.Game.Service;

namespace GameServer.LoginServer
{
    public class Action1002 : AuthorizeAction
    {
        public Action1002(ActionGetter httpGet)
            : base((int)ActionType.LoginOver, httpGet)
        {
        }

        public override bool GetUrlElement()
        {
            httpGet.Session.Close();

            return false;
        }

        public override bool TakeAction()
        {
            return false;
        }

        public override void BuildPacket()
        {
            
        }
    }
}
