using UnityEngine;
using System.Collections.Generic;

public enum AtbType
{
    None,
    MaxHP = 1,
    MaxMP,
    PhyAtk,
    MagAtk,
    PhyDef,
    MagDef,
    PhyPen,
    MagPen,
    PhyPenPct,
    MagPenPct,
    RegenHP,
    RegenMP,
    PhyVampPct,
    MagVampPct,
    CritPct,
    CritDecPct,
    CritEffect,
    CritDecEffect,
    MoveSpeed,
    CDReducePct,
    AtkSpeedPct,
    HRTimeDefPct,

    Grow_MaxHP = 1001,
    Grow_MaxMP,
    Grow_PhyAtk,
    Grow_MagAtk,
    Grow_PhyDef,
    Grow_MagDef,

    Base_MaxHP = 2001,
    Base_MaxMP,
    Base_PhyAtk,
    Base_MagAtk,
    Base_PhyDef,
    Base_MagDef,
    Base_PhyPen,
    Base_MagPen,
    Base_PhyPenPct,
    Base_MagPenPct,
    Base_RegenHP,
    Base_RegenMP,
    Base_PhyVampPct,
    Base_MagVampPct,
    Base_CritPct,
    Base_CritDecPct,
    Base_CritEffect,
    Base_CritDecEffect,
    Base_MoveSpeed,
    Base_CDReducePct,
    Base_AtkSpeedPct,
    Base_HRTimeDefPct,

    State_MaxHP = 3001,
    State_MaxMP,
    State_PhyAtk,
    State_MagAtk,
    State_PhyDef,
    State_MagDef,
    State_PhyPen,
    State_MagPen,
    State_PhyPenPct,
    State_MagPenPct,
    State_RegenHP,
    State_RegenMP,
    State_PhyVampPct,
    State_MagVampPct,
    State_CritPct,
    State_CritDecPct,
    State_CritEffect,
    State_CritDecEffect,
    State_MoveSpeed,
    State_CDReducePct,
    State_AtkSpeedPct,
    State_HRTimeDefPct,
}

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

public partial class Character : MonoBehaviour
{
    public void SetAtb(AtbType atb, int value)
    {
        int oldValue = 0;
        if (!mAtbs.TryGetValue(atb, out oldValue))
        {
            mAtbs.Add(atb, value);
        }
        else
        {
            mAtbs[atb] = value;
        }
    }

    public int GetAtb(AtbType atb)
    {
        int v = 0;
        if (!mAtbs.TryGetValue(atb, out v))
        {
            return 0;
        }
        return v;
    }

    public float GetAtbPct(AtbType atb)
    {
        int v = 0;
        if (!mAtbs.TryGetValue(atb, out v))
        {
            return 0.0f;
        }
        float pct = (float)v * 0.0001f;
        return pct;
    }

    public Dictionary<AtbType, int> mAtbs = new Dictionary<AtbType, int>();

    public int HP
    {
        set;
        get;
    }

    public int MP
    {
        set;
        get;
    }
}