using System;
using ZyGames.Framework.Game.Service;

namespace GameServer.CsScript.Action
{
    public class Action2001 : BaseStruct
    {
        public Action2001(ActionGetter httpGet)
            : base((int)ActionType.EnterLobby, httpGet)
        {
        }

        public override bool TakeAction()
        {
            throw new NotImplementedException();
        }
    }
}
