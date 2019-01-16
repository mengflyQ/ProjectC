using System;
using GameServer.RoomServer;
using System.Collections.Generic;

public enum HPChgType
{
    Init,
    PhyDamage,
    MagDamage,
    RealDamage,
    CritDamage,
    Cure,
    DebuffDamage,
}

public static class FightUtility
{
    public static void SendHpChg(int sendToUID, int chgHPUID, int hp, HPChgType chgType)
    {
        NotifyHPChg msg = new NotifyHPChg();
        msg.hp = hp;
        msg.chgType = chgType;
        msg.uid = chgHPUID;
        NetWork.NotifyMessage<NotifyHPChg>(sendToUID, STC.STC_AtbNotify, msg);
    }
}