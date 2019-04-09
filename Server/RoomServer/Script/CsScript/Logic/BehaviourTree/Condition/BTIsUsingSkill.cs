using System;

public class BTIsUsingSkill : BTCondition
{
    public BTIsUsingSkill(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        skillID = json["SkillID"].AsInt;
    }

    protected override BTStatus Update()
    {
        Skill skill = self.GetSkill();
        if (skill == null)
            return FailureResult;
        if (skillID > 0)
        {
            if (skill.SkillID == skillID)
            {
                return SucessResult;
            }
        }
        return SucessResult;
    }

    int skillID;
}