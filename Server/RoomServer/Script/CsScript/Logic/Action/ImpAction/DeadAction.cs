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

    public void Init(Character self, DeadType deadType, int[] datas)
    {
        mSelf = self;
        mDeadType = deadType;
        mDatas = datas;
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
                    float duration = (float)mDatas[0] * 0.001f;
                    if (time > duration)
                    {
                        return false;
                    }
                }
                break;
            case DeadType.Kill:
                {
                    float time = mStartTime - Time.ElapsedSeconds;
                    float duration = (float)(mDatas[0] + mDatas[1]) * 0.001f;
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

    int[] mDatas;
}