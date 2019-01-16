using System;
using System.Collections.Generic;

public partial class Character
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

    AtbTree mAtb = null;

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