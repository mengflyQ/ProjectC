using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;

public partial class Player : Character
{
    void InitPlayerAtb()
    {
        if (mChaClass == null)
            return;
        excel_cha_atb chaAtbExcel = excel_cha_atb.Find(mChaClass.chaAtbID);
        if (chaAtbExcel == null)
            return;
        SetAtb(AtbType.Base_MaxHP,          chaAtbExcel.maxHP);
        SetAtb(AtbType.Base_MaxMP,          chaAtbExcel.maxMP);
        SetAtb(AtbType.Base_PhyAtk,         chaAtbExcel.phyAtk);
        SetAtb(AtbType.Base_MagAtk,         chaAtbExcel.magAtk);
        SetAtb(AtbType.Base_PhyDef,         chaAtbExcel.phyDef);
        SetAtb(AtbType.Base_MagDef,         chaAtbExcel.magDef);
        SetAtb(AtbType.Base_PhyPen,         chaAtbExcel.phyPen);
        SetAtb(AtbType.Base_MagPen,         chaAtbExcel.magPen);
        SetAtb(AtbType.Base_PhyPenPct,      chaAtbExcel.phyPenPct);
        SetAtb(AtbType.Base_MagPenPct,      chaAtbExcel.magPenPct);
        SetAtb(AtbType.Base_RegenHP,        chaAtbExcel.regenHP);
        SetAtb(AtbType.Base_RegenMP,        chaAtbExcel.regenMP);
        SetAtb(AtbType.Base_PhyVampPct,     chaAtbExcel.phyVampPct);
        SetAtb(AtbType.Base_MagVampPct,     chaAtbExcel.magVampPct);
        SetAtb(AtbType.Base_CritPct,        chaAtbExcel.critPct);
        SetAtb(AtbType.Base_CritDecPct,     chaAtbExcel.critDecPct);
        SetAtb(AtbType.Base_CritEffect,     chaAtbExcel.critEffect);
        SetAtb(AtbType.Base_CritDecEffect,  chaAtbExcel.critDecEffect);
        SetAtb(AtbType.Base_MoveSpeed,      chaAtbExcel.moveSpeed);
        SetAtb(AtbType.Base_CDReducePct,    chaAtbExcel.cdReducePct);
        SetAtb(AtbType.Base_AtkSpeedPct,    chaAtbExcel.atkSpeedPct);
        SetAtb(AtbType.Base_HRTimeDefPct,   chaAtbExcel.hrTimeDefPct);

        HP = GetAtb(AtbType.MaxHP);
        MP = GetAtb(AtbType.MaxMP);

        Scene scn = mScene;
        if (scn != null)
        {
            for (int i = 0; i < scn.GetPlayerCount(); ++i)
            {
                Player p = scn.GetPlayerByIndex(i);
                FightUtility.SendHpChg(p.uid, uid, HP, HPChgType.Init);
            }
        }
    }
}