using System;

public enum BTTargetType
{
    All,
    Firend,
    Enemy,
}

public class BTHaveTarget : BTCondition
{
    public BTHaveTarget(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        targetType = (BTTargetType)json["TargetType"].AsInt;
    }

    protected override BTStatus Update()
    {
        Character target = self.GetTarget();
        if (target == null)
        {
            return FailureResult;
        }
        if (targetType == BTTargetType.Enemy && CampSystem.IsEnemy(self, target))
        {
            return SucessResult;
        }
        else if (targetType == BTTargetType.Firend && CampSystem.IsFriend(self, target))
        {
            return SucessResult;
        }
        else if (targetType == BTTargetType.All)
        {
            return SucessResult;
        }
        return FailureResult;
    }

    BTTargetType targetType;
}