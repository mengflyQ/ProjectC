using System;
using MathLib;
using System.Collections.Generic;
using GameServer.RoomServer;

public class DeadAction : IAction
{
    public DeadAction()
        : base(ChaActionType.Dead)
    {

    }

    public void Init(Character self, DeadType deadType)
    {
        mSelf = self;
        mDeadType = deadType;
    }

    public override void Enter()
    {
        mStartTime = Time.ElapsedSeconds;

        mSelf.SetTarget(null);

        HateSystem.Instance.Clear(mSelf);
    }

    public override bool LogicTick()
    {
        switch (mDeadType)
        {
            case DeadType.Vanish:
                return false;
            case DeadType.Fadeout:
            case DeadType.Dissolve:
                {
                    float time = mStartTime - Time.ElapsedSeconds;
                    float duration = 3.0f;
                    if (time > duration)
                    {
                        return false;
                    }
                }
                break;
            case DeadType.Kill:
                {
                    float time = mStartTime - Time.ElapsedSeconds;
                    float duration = mSelf.mChaList.deadAnimTime + 3.0f;
                    if (time > duration)
                    {
                        return false;
                    }
                }
                break;
        }
        return true;
    }

    public override void Exit()
    {
        
    }

    Character mSelf;
    DeadType mDeadType;
    float mStartTime;
}