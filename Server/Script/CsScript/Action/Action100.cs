using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZyGames.Framework.Game.Service;

namespace GameServer.CsScript.Action
{
    public class Action100 : BaseStruct
    {
        public Action100(ActionGetter httpGet)
            : base(100, httpGet)
        {

        }

        public override bool GetUrlElement()
        {
            return base.GetUrlElement();
        }

        public override bool TakeAction()
        {
            throw new NotImplementedException();
        }

        public override void BuildPacket()
        {
            base.BuildPacket();
        }
    }
}
