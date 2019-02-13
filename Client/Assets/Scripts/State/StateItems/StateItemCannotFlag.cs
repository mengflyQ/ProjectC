using System;

public class StateItemCannotFlag : BaseStateItem
{
    public override void Enter()
    {
        if (excel.dataIntArray.Length != 1)
            return;
        int canNotFlagID = excel.dataIntArray[0];

        SetEffectFlag(canNotFlagID, true);
    }

    public override void Exit()
    {
        if (excel.dataIntArray.Length != 1)
            return;
        int canNotFlagID = excel.dataIntArray[0];

        SetEffectFlag(canNotFlagID, false);
    }

    void SetEffectFlag(int id, bool v)
    {
        excel_can_not_flag excel = excel_can_not_flag.Find(id);
        if (excel == null)
            return;
        StateMgr stateMgr = stateGroup.mSelf.mStateMgr;
        if (stateMgr == null)
            return;

        if (excel.canNotMove != 0)
        {
            SetFlag(stateMgr, CannotFlag.CannotMove, v);
        }
        if (excel.canNotControl != 0)
        {
            SetFlag(stateMgr, CannotFlag.CannotControl, v);
        }
        if (excel.canNotSkill != 0)
        {
            SetFlag(stateMgr, CannotFlag.CannotSkill, v);
        }
        if (excel.canNotSelected != 0)
        {
            SetFlag(stateMgr, CannotFlag.CannotSelected, v);
        }
    }

    void SetFlag(StateMgr stateMgr, CannotFlag flag, bool v)
    {
        if (v)
        {
            int count = stateMgr.mCannotFlagCount[(int)flag] + 1;
            stateMgr.mCannotFlagCount[(int)flag] = count;
            if (count == 1)
            {
                stateMgr.mOwner.SetCannotFlag(flag, OptType.State, true);
            }
        }
        else
        {
            int count = stateMgr.mCannotFlagCount[(int)flag] - 1;
            if (count <= 0)
            {
                count = 0;
                stateMgr.mOwner.SetCannotFlag(flag, OptType.State, false);
            }
            stateMgr.mCannotFlagCount[(int)flag] = count;
        }
    }
}