using System;
using MathLib;

public class BTRandomSequence : BTComposite
{
    public BTRandomSequence(Character self)
        : base(self)
    {

    }

    protected override void Enter()
    {
        curChildIndex = 0;
        if (mUseSeed)
        {
            Mathf.ShuffleWithSeed(ref children, mSeed);
        }
        else
        {
            Mathf.Shuffle(ref children);
        }
    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        mUseSeed = json["UseSeed"].AsBool;
        mSeed = json["Seed"].AsInt;
        mExeCount = json["ExeCount"].AsInt;
    }

    protected override BTStatus Update()
    {
        while (curChildIndex < children.Count)
        {
            BTBehavior curChild = children[curChildIndex];
            if (curChild == null)
            {
                break;
            }
            BTStatus s = curChild.Tick();
            // 如果执行成功了就继续执行，否则返回;
            if (s != BTStatus.Success)
                return s;
            if (++curChildIndex >= children.Count)
            {
                return BTStatus.Success;
            }
            if (mExeCount >= 0 && curChildIndex >= mExeCount)
            {
                return BTStatus.Success;
            }
        }
        return BTStatus.Invalid; // 循环意外终止;
    }

    protected int curChildIndex = 0;

    protected bool mUseSeed = false;
    protected int mSeed;
    protected int mExeCount = -1;
}