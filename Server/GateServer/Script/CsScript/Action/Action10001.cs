using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Service;
using ZyGames.Framework.Game.Contract;
using ZyGames.Framework.RPC.IO;
using ProtoBuf;
using ZyGames.Framework.Common.Serialization;

[Serializable, ProtoContract]
public class LoginData
{
    [ProtoMember(1)]
    public int ScreenWidth;
    [ProtoMember(2)]
    public int ScreenHeight;
    [ProtoMember(3)]
    public string UserName;
    [ProtoMember(4)]
    public string Platform;
    [ProtoMember(5)]
    public string DeviceUniqueIdentifier;
    [ProtoMember(6)]
    public string DeviceModel;
    [ProtoMember(7)]
    public string DeviceTypeStr;
}

namespace GameServer.GateServer
{
    public class Action10001 : GateAction
    {
        public Action10001(ActionGetter actionGetter)
            : base((int)ActionType.LoginAction, actionGetter)
        {
        }

        public override bool GetUrlElement()
        {
            mData = (byte[])actionGetter.GetMessage();

            SetTcpProxy("login_poxy", "127.0.0.1", 9002, 10 * 1000);

            return true;
        }

        public override string GetRoutPath()
        {
            return "GateHandler";
        }

        public override bool SetRequestParam(RequestParam param)
        {
            param.Add("Data", mData);
            return true;
        }

        public override bool TakeAction()
        {
            return true;
        }        

        public override void BuildPacket()
        {
            PushIntoStack(ServerResultBytes);
        }

        byte[] mData;
    }
}
