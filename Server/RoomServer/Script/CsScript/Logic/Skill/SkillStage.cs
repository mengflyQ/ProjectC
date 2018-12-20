using System;
using System.Collections.Generic;

public class SkillStage
{
    public SkillStage(Skill skill, int stageID, SkillContext context)
    {
        mStageInfo = excel_skill_stage.Find(stageID);

    }

    public void Enter()
    {

    }

    public bool LogicTick()
    {
        return true;
    }

    public void Exit()
    {

    }

    public static bool IsTrait(SkillStageTrait trait, excel_skill_stage stageInfo)
    {
        int flag = 1 << (int)trait;
        if ((stageInfo.trait & flag) != 0)
        {
            return true;
        }
        return false;
    }

    public excel_skill_stage mStageInfo = null;
}