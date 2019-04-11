using System;
using System.Collections.Generic;
using MathLib;

public class BTWeightRandomSelector : BTRandomSelector
{
    public BTWeightRandomSelector(Character self)
        : base(self)
    {

    }

    protected override void Enter()
    {
        curChildIndex = 0;

        Random r = null;
        if (mUseSeed)
        {
            r = new Random(mSeed);
        }

        if (mWeights.Count != children.Count)
        {
            return;
        }

        int totalWeight = 0;
        for (int i = 0; i < mWeights.Count; ++i)
        {
            totalWeight += mWeights[i];
        }

        int count = Mathf.Min(children.Count, mExeCount);
        for (int i = 0; i < count; ++i)
        {
            int j = -1;
            int m = 0;
            if (mUseSeed)
            {
                m = r.Next(0, totalWeight);
            }
            else
            {
                m = Mathf.RandRange(0, totalWeight);
            }
            int curTotalWeight = 0;
            for (j = i; j < children.Count; ++j)
            {
                curTotalWeight += mWeights[j];
                if (curTotalWeight >= m)
                {
                    break;
                }
            }
            if (j >= 0)
            {
                BTBehavior temp = children[i];
                children[i] = children[j];
                children[j] = temp;

                int tempWeight = mWeights[i];
                mWeights[i] = mWeights[j];
                mWeights[j] = tempWeight;

                totalWeight -= mWeights[i];
            }
        }
    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        mWeights.Clear();
        LitJson.JsonData weights = json["Weights"];
        for (int i = 0; i < weights.Count; ++i)
        {
            mWeights.Add(weights[i].AsInt);
        }
    }

    protected List<int> mWeights = new List<int>();
}