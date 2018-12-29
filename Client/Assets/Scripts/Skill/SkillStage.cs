using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillStage
{
    public SkillStage(Skill skill, int stageID, SkillContext context)
    {
        mSkill = skill;
        mStageInfo = excel_skill_stage.Find(stageID);
        mSkillContext = context;
        mBreak = 0;
    }

    public void Enter()
    {
        mVanish = false;

        mJumpType = SkillJumpType.None;
        mJumpID = 0;
        mJumpTick = 0;
        mBreak = 0;

        mTick = 0;
    }

    public bool LogicTick()
    {
        if (IsVanish())
        {
            return false;
        }

        DoEventFrame();
        UpdateState();

        return !IsVanish();
    }

    public void Exit()
    {
        if (!IsBreak())
        {
            DoEvent(this, SkillEventTriggerType.NormalEnd);
        }
        DoEvent(this, SkillEventTriggerType.FinalEnd);
    }

    public void SetVanish()
    {
        mVanish = true;
    }

    public bool IsVanish()
    {
        return mVanish;
    }

    void DoEventFrame()
    {
        DoEvent(this, SkillEventTriggerType.Frame, mTick);
        DoLoopEvent(this, mTick);

        ++mTick;
    }

    void UpdateState()
    {
        if (mStageInfo == null)
        {
            SetVanish();
            return;
        }
        bool bOver = false;
        int nextStageID = mStageInfo.nextStageID;
        if (IsJumpStream())
        {
            bOver = true;
            if (mJumpID > 0)
            {
                nextStageID = mJumpID;
            }
            else
            {
                nextStageID = 0;
            }
            SetBreak(SkillBreakType.Jump, false);
        }
        else
        {
            if (IsTimeOver())
            {
                bOver = true;
            }
        }
        if (IsJumpSkill())
        {
            nextStageID = 0;
            bOver = true;
            SetBreak(SkillBreakType.Jump, false);
        }

        if (bOver)
        {
            if (IsBreak())
            {
                DoEvent(this, SkillEventTriggerType.ExeptEnd);
                SetVanish();
                nextStageID = 0;
            }
            if (nextStageID > 0)
            {
                mSkill.SetStage(nextStageID);
            }
            else
            {
                SetVanish();
            }
        }

        if (IsJumpSkill())
        {
            SkillHandle handle = new SkillHandle();
            handle.skillID = mJumpID;
            handle.caster = Owner;
            handle.autoTargetPos = false;
            handle.targetPos = mSkillContext.TargetPos;
            handle.skillTargetID = mSkillContext.SkillTargetID;
            SkillHandle.UseSkill(handle);
        }
    }

    public bool IsJumpStream()
    {
        if (mJumpType == SkillJumpType.Stage)
        {
            if (mJumpTick <= mTick)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsJumpSkill()
    {
        if (mJumpType == SkillJumpType.Skill)
        {
            if (mJumpTick <= mTick)
            {
                return true;
            }
        }
        return false;
    }

    public void SetBreak(SkillBreakType type, bool force = true)
    {
        mBreak |= (1 << (int)type);
        if (force)
        {
            DoEvent(this, SkillEventTriggerType.ExeptEnd);
            SetVanish();
        }
    }

    public bool IsBreak()
    {
        return mBreak != 0;
    }

    public bool IsTimeOver()
    {
        int totalTick = mStageInfo.time;

        if (mTick >= totalTick)
            return true;
        return false;
    }

    public static bool IsStageTrait(SkillStageTrait trait, excel_skill_stage stageInfo)
    {
        if (stageInfo == null)
            return false;
        int flag = 1 << (int)trait;
        if ((stageInfo.trait & flag) != 0)
        {
            return true;
        }
        return false;
    }

    public static bool IsEventTrait(SkillEventTrait trait, excel_skill_event eventInfo)
    {
        if (eventInfo == null)
            return false;
        int flag = 1 << (int)trait;
        if ((eventInfo.trait & flag) != 0)
        {
            return true;
        }
        return false;
    }

    public static void DoEvent(SkillStage stage, SkillEventTriggerType triggerType, int param1 = -1, int param2 = -1)
    {
        excel_skill_stage stageInfo = stage.mStageInfo;
        for (int i = 0; i < stageInfo.events.Length; ++i)
        {
            int eventID = stageInfo.events[i];
            excel_skill_event eventInfo = excel_skill_event.Find(eventID);
            if ((int)triggerType != eventInfo.triggerType)
                continue;
            if (!IsEventTrait(SkillEventTrait.Client, eventInfo))
                continue;
            if (param1 != -1 && param1 != eventInfo.triggerParam1)
                continue;
            if (param2 != -1 && param2 != eventInfo.triggerParam2)
                continue;

            SkillEventRegister.SkillEventMethod e = null;
            SkillEventType type = (SkillEventType)eventInfo.eventType;
            if (!SkillEventRegister.events.TryGetValue(type, out e))
            {
                continue;
            }
            e(stage.Owner, stage.mSkillContext, eventInfo);
        }
    }

    public static void DoLoopEvent(SkillStage stage, int curTick)
    {
        excel_skill_stage stageInfo = stage.mStageInfo;
        for (int i = 0; i < stageInfo.events.Length; ++i)
        {
            int eventID = stageInfo.events[i];
            excel_skill_event eventInfo = excel_skill_event.Find(eventID);
            if ((int)SkillEventTriggerType.Loop != eventInfo.triggerType)
                continue;
            if (!IsEventTrait(SkillEventTrait.Client, eventInfo))
                continue;
            int tick = curTick - eventInfo.triggerParam1;
            if (tick >= 0 && eventInfo.triggerParam2 > 0 && tick % eventInfo.triggerParam2 == 0)
            {
                SkillEventRegister.SkillEventMethod e = null;
                SkillEventType type = (SkillEventType)eventInfo.eventType;
                if (!SkillEventRegister.events.TryGetValue(type, out e))
                    continue;
                e(stage.mSkillContext.mOwner, stage.mSkillContext, eventInfo);
            }
        }
    }

    public Character Owner
    {
        get
        {
            return mSkillContext.mOwner;
        }
    }

    public excel_skill_stage mStageInfo = null;
    public SkillContext mSkillContext = null;

    private Skill mSkill;

    SkillJumpType mJumpType;
    int mJumpTick = 0;
    int mJumpID = 0;

    bool mVanish = false;
    int mBreak = 0;
    int mTick = 0;
}