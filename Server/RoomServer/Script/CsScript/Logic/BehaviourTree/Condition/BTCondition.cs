using System;

public class BTCondition : BTBehavior
{
    public BTCondition(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        isNegation = json["Neg"].AsInt > 0;
    }

    protected BTStatus SucessResult
    {
        get
        {
            return !isNegation ? BTStatus.Success : BTStatus.Failure;
        }
    }

    protected BTStatus FailureResult
    {
        get
        {
            return !isNegation ? BTStatus.Failure : BTStatus.Success;
        }
    }

    // 是否取反;
    protected bool isNegation = false;
}