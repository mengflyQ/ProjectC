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

        mSelf.StopMove(false);

        mSelf.SetCannotFlag(CannotFlag.CannotControl, OptType.Dead, true);
        mSelf.SetCannotFlag(CannotFlag.CannotMove, OptType.Dead, true);
        mSelf.SetCannotFlag(CannotFlag.CannotSkill, OptType.Dead, true);
        mSelf.SetCannotFlag(CannotFlag.CannotSelected, OptType.Dead, true);
        mSelf.SetCannotFlag(CannotFlag.CannotBeHit, OptType.Dead, true);

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
                    float time = Time.ElapsedSeconds - mStartTime;
                    float duration = 3.0f;
                    if (time > duration)
                    {
                        return false;
                    }
                }
                break;
            case DeadType.Kill:
                {
                    float time = Time.ElapsedSeconds - mStartTime;
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
        mSelf.SetCannotFlag(CannotFlag.CannotControl, OptType.Dead, false);
        mSelf.SetCannotFlag(CannotFlag.CannotMove, OptType.Dead, false);
        mSelf.SetCannotFlag(CannotFlag.CannotSkill, OptType.Dead, false);
        mSelf.SetCannotFlag(CannotFlag.CannotSelected, OptType.Dead, false);
        mSelf.SetCannotFlag(CannotFlag.CannotBeHit, OptType.Dead, false);

        mSelf.Destroy();
    }

    Character mSelf;
    DeadType mDeadType;
    float mStartTime;
}