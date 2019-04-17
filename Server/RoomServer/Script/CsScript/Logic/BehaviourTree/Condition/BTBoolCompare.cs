using System;

public class BTBoolCompare : BTCompare
{
    public BTBoolCompare(Character self)
        : base(self)
    {

    }

    protected override BTStatus Update()
    {
        VariableBool b1 = v1 as VariableBool;
        VariableBool b2 = v2 as VariableBool;
        if (op == BTCompareOp.Equal)
        {
            return b1.value == b2.value ? SucessResult : FailureResult;
        }
        else if (op == BTCompareOp.NotEqual)
        {
            return b1.value != b2.value ? SucessResult : FailureResult;
        }
        return base.Update();
    }
}