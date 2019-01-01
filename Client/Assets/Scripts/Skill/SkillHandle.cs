using UnityEngine;
using System;
using System.Collections.Generic;

public struct SkillHandle
{
    public int skillID;
    public Character caster;
    public bool autoTargetPos;
    public Vector3 targetPos;
    public int skillTargetID;

    public static SkillResult UseSkill(SkillHandle handle)
    {
        if (handle.caster == null || handle.caster.IsCannotFlag(CannotFlag.CannotSkill))
            return SkillResult.InvalidCaster;
        SkillContext context = new SkillContext();
        context.mOwner = handle.caster;
        context.SkillTargetID = handle.skillTargetID;
        if (handle.autoTargetPos)
        {
            Character target = context.mOwner.GetTarget();
            if (target == null)
            {
                context.TargetPos = context.mOwner.Position;
            }
            else
            {
                context.TargetPos = target.Position;
            }
        }
        else
        {
            context.TargetPos = handle.targetPos;
        }

        Skill skill = new Skill(handle.skillID, context);
        handle.caster.SetSkill(skill);
        return SkillResult.Success;
    }
}