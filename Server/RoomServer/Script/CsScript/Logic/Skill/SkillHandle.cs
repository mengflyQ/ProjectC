using MathLib;
using System;
using System.Collections.Generic;
using GameServer.RoomServer;

public struct SkillHandle
{
    public int skillID;
    public Character caster;
    public bool autoTargetPos;
    public Vector3 targetPos;
    public int skillTargetID;
    public OnSkillEnd skillEndMethod;

    public delegate void OnStageEnd(int skillID, int stageID);
    public delegate void OnSkillEnd(int skillID);

    public static SkillResult UseSkill(SkillHandle handle)
    {
        if (handle.caster == null || handle.caster.IsCannotFlag(CannotFlag.CannotSkill))
        {
            return SkillResult.InvalidCaster;
        }
        Skill lastSkill = handle.caster.GetSkill();
        if (lastSkill != null)
        {
            SkillStage stage = lastSkill.mCurStage;
            if (stage != null)
            {
                if (SkillStage.IsStageTrait(SkillStageTrait.WaitBreakStage, stage.mStageInfo))
                {
                    if (stage.mStageEnd == null)
                    {
                        WaitBreakStage onStageEnd = new WaitBreakStage();
                        onStageEnd.nextSkillHandle = handle;
                        stage.SetBreak(SkillBreakType.OtherSkill, false, onStageEnd.OnStageEnd);
                    }

                    return SkillResult.CannotBreak;
                }
                if (!SkillStage.IsStageTrait(SkillStageTrait.BreakSelf, stage.mStageInfo)
                    && lastSkill.SkillID == handle.skillID)
                {
                    return SkillResult.CannotBreak;
                }
                if (!SkillStage.IsStageTrait(SkillStageTrait.SkillBreak, stage.mStageInfo))
                {
                    return SkillResult.CannotBreak;
                }
            }
        }

        excel_skill_list skillExcel = excel_skill_list.Find(handle.skillID);
        if (skillExcel == null)
        {
            return SkillResult.ExcelNotExist;
        }
        Character target = handle.caster.mScene.FindCharacter(handle.skillTargetID);

        SkillTargetType targetType = (SkillTargetType)skillExcel.targetType;
        if (targetType != SkillTargetType.All)
        {
            if (target == null)
            {
                return SkillResult.InvalidTarget;
            }
            if (targetType == SkillTargetType.Enemy && !CampSystem.IsEnemy(handle.caster, target))
            {
                return SkillResult.InvalidTarget;
            }
            if (targetType == SkillTargetType.Friend && !CampSystem.IsFriend(handle.caster, target))
            {
                return SkillResult.InvalidTarget;
            }
        }
        if (target == null && skillExcel.maxDistance > 0.0f)
            return SkillResult.InvalidTarget;

        SkillContext context = new SkillContext();
        context.mSkillID = handle.skillID;
        context.mOwner = handle.caster;
        context.SkillTargetID = handle.skillTargetID;
        if (!handle.autoTargetPos)
        {
            context.TargetPos = handle.targetPos;
        }
        else
        {
            context.TargetPos = target == null ? Vector3.zero : target.Position;
        }
        Skill skill = new Skill(handle.skillID, context);
        handle.caster.SetSkill(skill);
        skill.mSkillEnd = handle.skillEndMethod;

        ReqSkill notifyMsg = new ReqSkill();
        notifyMsg.skillID = handle.skillID;
        notifyMsg.casterID = handle.caster.gid;
        notifyMsg.targetID = handle.skillTargetID;
        notifyMsg.autoTargetPos = handle.autoTargetPos;
        notifyMsg.targetPos = Vector3Packat.FromVector3(handle.targetPos);
        notifyMsg.position = Vector3Packat.FromVector3(handle.caster.Position);
        notifyMsg.direction = Vector3Packat.FromVector3(handle.caster.Direction);

        for (int i = 0; i < handle.caster.mScene.GetPlayerCount(); ++i)
        {
            Player player = handle.caster.mScene.GetPlayerByIndex(i);
            NetWork.NotifyMessage(player.UserID, STC.STC_SkillNotify, notifyMsg);
        }

        return SkillResult.Success;
    }

    public class WaitBreakStage
    {
        public void OnStageEnd(int skillID, int stageID)
        {
            SkillHandle.UseSkill(nextSkillHandle);
        }

        public SkillHandle nextSkillHandle;
    }
}