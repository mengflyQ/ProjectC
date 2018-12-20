using MathLib;
using System;
using System.Collections.Generic;

public static class SkillHandle
{
    public static SkillResult UseSkill(Character cha, int id, int targetID, Vector3 targetPos)
    {
        excel_skill_list skillExcel = excel_skill_list.Find(id);
        if (skillExcel == null)
        {
            return SkillResult.ExcelNotExist;
        }
        Character target = cha.mScene.FindCharacter(targetID);

        SkillTargetType targetType = (SkillTargetType)skillExcel.targetType;
        if (targetType != SkillTargetType.All)
        {
            if (target == null)
            {
                return SkillResult.InvalidTarget;
            }
            if (targetType == SkillTargetType.Enemy && !CampSystem.IsEnemy(cha, target))
            {
                return SkillResult.InvalidTarget;
            }
            if (targetType == SkillTargetType.Friend && !CampSystem.IsFriend(cha, target))
            {
                return SkillResult.InvalidTarget;
            }
        }
        if (target == null && skillExcel.maxDistance > 0.0f)
            return SkillResult.InvalidTarget;

        SkillContext context = new SkillContext();
        context.mOwner = cha;
        context.SkillTargetID = targetID;
        context.TargetPos = targetPos;
        Skill skill = new Skill(id, context);
        cha.SetSkill(skill);

        return SkillResult.Success;
    }
}