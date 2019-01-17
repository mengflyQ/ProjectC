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

    public void PacketAtb()
    {
        mAtb.PacketToMsg();
        mAtb.mAtbMsgAround.atbTypes.Add(50001);
        mAtb.mAtbMsgAround.atbValues.Add(HP);
        mAtb.mAtbMsgAround.atbTypes.Add(50002);
        mAtb.mAtbMsgAround.atbValues.Add(MP);

        mAtb.mAtbMsgSelf.atbTypes.Add(50001);
        mAtb.mAtbMsgSelf.atbValues.Add(HP);
        mAtb.mAtbMsgSelf.atbTypes.Add(50002);
        mAtb.mAtbMsgSelf.atbValues.Add(MP);
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