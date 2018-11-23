using System;
using ZyGames.Framework.Game.Service;

namespace GameServer.CsScript.LoginServer
{
    public class Action1002 : BaseStruct
    {
        public Action1002(ActionGetter httpGet)
            : base((int)ActionType.LoginOver, httpGet)
        {
        }

        public override bool GetUrlElement()
        {
            httpGet.Session.Close();

            return true;
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
