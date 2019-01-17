﻿using MathLib;
using System;
using System.Collections.Generic;
using GameServer.RoomServer;

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

        if (mSkillInfo.targetType != (int)SkillTargetType.All
            && !IsInRange())
        {
            Character skillTarget = mSkillContext.SkillTarget;
            if (skillTarget == null)
            {
                mSkillState = SkillState.Failed;
                return;
            }
            float destRadius = MathLib.Math.Max(mSkillInfo.maxDistance - 0.5f, 0.0f);
            Owner.SearchMove(skillTarget.Position, destRadius, false);
            mSkillState = SkillState.TrackEnemy;
        }
        else
        {
            if (Owner.Type != CharacterType.Player)
            {
                BeginSkill();
            }
            else
            {
                mSkillState = SkillState.TrackEnemy;
            }
        }
    }

    public bool LogicTick()
    {
        bool rst = false;
        switch (mSkillState)
        {
            case SkillState.TrackEnemy:
                {
                    if (!TrackEnemy())
                    {
                        mSkillState = SkillState.Break;
                    }
                    if (IsInRange() && mSkillState != SkillState.Break)
                    {
                        BeginSkill();
                    }
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

        if (mSkillEnd != null)
        {
            mSkillEnd(SkillID);
        }
    }

    public void BeginSkill()
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
            if (SkillStage.IsStageTrait(SkillStageTrait.FirstStage, stageInfo))
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

        if (!SkillStage.IsStageTrait(SkillStageTrait.AllowMove, mCurStage.mStageInfo)
            || SkillStage.IsStageTrait(SkillStageTrait.MoveBreak, mCurStage.mStageInfo))
        {
            Owner.StopMove(false);
        }

        mSkillState = SkillState.Using;

        SkillBegin msg = new SkillBegin();
        msg.uid = Owner.uid;
        msg.skillID = SkillID;
        msg.position = Vector3Packat.FromVector3(Owner.Position);
        msg.direction = Vector3Packat.FromVector3(Owner.Direction);

        Scene scn = Owner.mScene;
        for (int i = 0; i < scn.GetPlayerCount(); ++i)
        {
            Player player = scn.GetPlayerByIndex(i);
            if (player == Owner)
                continue;
            NetWork.NotifyMessage(player.uid, STC.STC_SkillBegin, msg);
        }
    }

    bool IsInRange()
    {
        // 主角追敌相信客户端;
        if (Owner.Type == CharacterType.Player)
        {
            return false;
        }
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

    bool TrackEnemy()
    {
        // 主角追敌相信客户端;
        if (Owner.Type == CharacterType.Player)
        {
            return true;
        }
        Character target = Owner.GetTarget();
        if (target == null)
        {
            return false;
        }
        if (mLastTargetPosition == target.Position)
        {
            return true;
        }
        Owner.SearchMove(target.Position, target.Radius);
        mLastTargetPosition = target.Position;
        return true;
    }

    public void OnMove()
    {
        if (mCurStage != null)
        {
            if (SkillStage.IsStageTrait(SkillStageTrait.MoveBreak, mCurStage.mStageInfo))
            {
                mCurStage.SetBreak(SkillBreakType.Move, true);
            }
        }
        else
        {
            if (mSkillState == SkillState.TrackEnemy)
            {
                Owner.StopMove(false);
            }
            mSkillState = SkillState.Break;
        }
    }

    Character Owner
    {
        get
        {
            return mSkillContext.mOwner;
        }
    }

    public int SkillID
    {
        get
        {
            if (mSkillInfo == null)
                return 0;
            return mSkillInfo.id;
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

    private Vector3 mLastTargetPosition;

    public SkillHandle.OnSkillEnd mSkillEnd = null;
}