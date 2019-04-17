using System;

public class BTStringCompare : BTCompare
{
    public BTStringCompare(Character self)
        : base(self)
    {

    }

    protected override BTStatus Update()
    {
        VariableString b1 = v1 as VariableString;
        VariableString b2 = v2 as VariableString;
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