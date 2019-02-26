using System;
using UnityEngine;
using System.Collections.Generic;

public class DeadAction : IAction
{
    public DeadAction() : base(ChaActionType.Dead)
    {

    }

    public void Init(Character self, DeadType deadType)
    {
        mSelf = self;
        mDeadType = deadType;
    }

    public override void Enter()
    {
        mStartTime = Time.realtimeSinceStartup;

        mSelf.SetTarget(null);

        switch (mDeadType)
        {
            case DeadType.Kill:
                {
                    mSelf.PlayPriorityAnimation(5, AnimPriority.Die);
                }
                break;
        }
    }

    public override bool LogicTick()
    {
        switch (mDeadType)
        {
            case DeadType.Kill:
                {
                    float time = mStartTime - Time.realtimeSinceStartup;
                    float animTime = mSelf.mChaList.deadAnimTime;
                    float tweenTime = 3.0f;
                    if (time <= animTime)
                    {
                    }
                    else if (time > animTime && time <= (animTime + tweenTime))
                    {
                        if (tweenTime <= 0.0f)
                        {
                            return false;
                        }
                        float t = time - animTime;
                        t = 1.0f - t / tweenTime;
                        mSelf.Alpha = t;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case DeadType.Fadeout:
                {
                    float time = mStartTime - Time.realtimeSinceStartup;
                    float duration = 3.0f;
                    if (duration <= 0.0f)
                    {
                        return false;
                    }
                    if (time <= duration)
                    {
                        float t = 1.0f - time / duration;
                        mSelf.Alpha = t;
                    }
                    else
                    {
                        return false;
                    }
                }
                break;
            case DeadType.Dissolve:
                {
                    float time = mStartTime - Time.realtimeSinceStartup;
                    float duration = 3.0f;
                    if (time > duration)
                    {
                        return false;
                    }
                }
                break;
            case DeadType.Vanish:
                {
                    return false;
                }
        }
        return true;
    }

    public override void Exit()
    {
        switch (mDeadType)
        {
            case DeadType.Kill:
            case DeadType.Fadeout:
                {
                    mSelf.Alpha = 1.0f;
                }
                break;
        }
    }

    Character mSelf;
    DeadType mDeadType;
    float mStartTime;
}
