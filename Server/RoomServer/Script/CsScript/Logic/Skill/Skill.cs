using MathLib;
using System;
using System.Collections.Generic;

public class Skill
{
    public Skill(int id, SkillContext context)
    {
        mSkillInfo = excel_skill_list.Find(id);
        mSkillContext = context;

        mSkillState = SkillState.Failed;
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
        mSkillState = SkillState.TrackEnemy;

        //if (mSkillInfo.targetType != (int)SkillTargetType.All)
        //{
        //    Character skillTarget = mSkillContext.SkillTarget;
        //    if (skillTarget == null)
        //    {
        //        mSkillState = SkillState.Failed;
        //        return;
        //    }
        //    // float destRadius = MathLib.Math.Max(mSkillInfo.maxDistance - 0.5f, 0.0f);
        //    // Owner.SearchMove(skillTarget.Position, destRadius, false);
        //    mSkillState = SkillState.TrackEnemy;
        //}
        //else
        //{
        //    BeginSkill();
        //}
    }

    public bool LogicTick()
    {
        bool rst = false;
        switch (mSkillState)
        {
            case SkillState.TrackEnemy:
                {
                    //if (IsInRange())
                    //{
                    //    BeginSkill();
                    //}
                    rst = true;
                    break;
                }
            case SkillState.Using:
                {
                    if (mCurStage == null)
                    {
                        Debug.LogError("当前技能没有技能段");
                    }
                    else
                    {
                        rst = mCurStage.LogicTick();
                    }
                    break;
                }
        }
        return rst;
    }

    public void Exit()
    {
        SetStage(0);
    }

    void BeginSkill()
    {
        int firstStageID = -1;
        for (int i = 0; i < mSkillInfo.stages.Length; ++i)
        {
            int stageID = mSkillInfo.stages[i];
            excel_skill_stage stageInfo = excel_skill_stage.Find(stageID);
            if (stageInfo == null)
            {
                Debug.LogError("Error: 找不到技能段{0}，技能ID：{1}", stageID, mSkillInfo.id);
                continue;
            }
            if (SkillStage.IsTrait(SkillStageTrait.FirstStage, stageInfo))
            {
                firstStageID = stageInfo.id;
                break;
            }
        }
        if (firstStageID < 0)
        {
            Debug.LogError("Error: 找不到初始技能段，技能ID：{0}", mSkillInfo.id);
            return;
        }
        SetStage(firstStageID);
        if (mCurStage == null)
        {
            mSkillState = SkillState.Break;
            return;
        }

        Character skillTarget = mSkillContext.SkillTarget;
        if (mSkillInfo.targetType != (int)SkillTargetType.All && skillTarget != null)
        {
            Vector3 dir = skillTarget.Position - Owner.Position;
            Owner.Direction = dir;
        }

        if (!SkillStage.IsTrait(SkillStageTrait.AllowMove, mCurStage.mStageInfo)
            || SkillStage.IsTrait(SkillStageTrait.MoveBreak, mCurStage.mStageInfo))
        {
            Owner.StopMove(false);
        }

        mSkillState = SkillState.Using;
    }

    bool IsInRange()
    {
        Character skillTarget = mSkillContext.SkillTarget;
        if (skillTarget == null)
        {
            return true;
        }
        float maxDist = mSkillInfo.maxDistance;
        float distance = SkillUtility.GetDistance(Owner, mSkillContext.SkillTarget, DistanceCalcType.OuterB);
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
    public SkillState mSkillState;
}