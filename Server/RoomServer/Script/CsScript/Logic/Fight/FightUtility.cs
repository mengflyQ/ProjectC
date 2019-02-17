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
    public static void SendHpChg(Character sender, int chgHPGID, int hp, HPChgType chgType)
    {
        if (sender.Type != CharacterType.Player)
            return;
        Player player = sender as Player;
        NotifyHPChg msg = new NotifyHPChg();
        msg.hp = hp;
        msg.chgType = chgType;
        msg.uid = chgHPGID;
        NetWork.NotifyMessage<NotifyHPChg>(player.UserID, STC.STC_HPChg, msg);
    }
}