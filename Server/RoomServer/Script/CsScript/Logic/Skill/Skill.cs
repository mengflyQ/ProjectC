using MathLib;
using System;
using System.Collections.Generic;

public class Skill
{
    public Skill(int id, SkillContext context)
    {
        mSkillInfo = excel_skill_list.Find(id);
        mSkillContext = context;

        mSkilState = SkillState.Failed;
    }

    public void SetStage(int stageID)
    {
        SkillStage stage = null;
        if (stageID != 0)
        {
            stage = new SkillStage(this, stageID, mSkillContext);
        }

        SkillStage lastStage = mCurStage;
        mCurStage = stage;
        if (lastStage != null)
        {
            lastStage.Exit();
        }

        if (mCurStage != null)
        {
            mCurStage.Enter();
        }
    }

    public void Enter()
    {
        if (mSkillInfo.targetType != (int)SkillTargetType.All)
        {

        }
    }

    public bool LogicTick()
    {
        return true;
    }

    public void Exit()
    {

    }

    bool IsInRange()
    {
        Character skillTarget = mSkillContext.SkillTarget;
        if (skillTarget == null)
        {
            return true;
        }
        float maxDist = (float)mSkillData.MaxDistance * 0.001f;
        float distance = SkillUtility.GetDistance(Owner, mSkillContext.mSkillTarget, DistanceCalcType.OuterB);
        if (distance > maxDist)
        {
            return false;
        }
        return true;
    }

    Character Owner
    {
        get
        {
            return mSkillContext.mOwner;
        }
    }

    // 技能表格数据;
    excel_skill_list mSkillInfo = null;
    // 正在执行的技能段;
    public SkillStage mCurStage;
    // 当前技能的上下文;
    SkillContext mSkillContext = null;
    // 技能状态;
    public SkillState mSkilState;
}