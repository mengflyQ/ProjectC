using System;
using System.Collections.Generic;

public partial class Character : GameObject
{
    void InitAtb()
    {
        mAtb = new AtbTree(this);
    }

    public void SetAtb(AtbType atb, int value)
    {
        mAtb.SetAtb(atb, value);
    }

    public int GetAtb(AtbType atb)
    {
        return mAtb.GetAtb(atb);
    }

    public float GetAtbPct(AtbType atb)
    {
        return mAtb.GetAtbPct(atb);
    }

    public void PacketAtb()
    {
        mAtb.PacketToMsg();
    }

    AtbTree mAtb = null;

    public int HP
    {
        set
        {
            SetAtb(AtbType.HP, value);
        }
        get
        {
            return GetAtb(AtbType.HP);
        }
    }

    public int MP
    {
        set
        {
            SetAtb(AtbType.MP, value);
        }
        get
        {
            return GetAtb(AtbType.MP);
        }
    }
}