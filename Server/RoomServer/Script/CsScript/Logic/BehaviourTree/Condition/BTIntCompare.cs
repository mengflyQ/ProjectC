using System;

public class BTIntCompare : BTCompare
{
    public BTIntCompare(Character self)
        : base(self)
    {

    }

    protected override BTStatus Update()
    {
        VariableInt b1 = v1 as VariableInt;
        VariableInt b2 = v2 as VariableInt;
        if (op == BTCompareOp.Equal)
        {
            return b1.value == b2.value ? SucessResult : FailureResult;
        }
        else if (op == BTCompareOp.NotEqual)
        {
            return b1.value != b2.value ? SucessResult : FailureResult;
        }
        else if (op == BTCompareOp.Less)
        {
            return b1.value < b2.value ? SucessResult : FailureResult;
        }
        else if (op == BTCompareOp.LessEqual)
        {
            return b1.value <= b2.value ? SucessResult : FailureResult;
        }
        else if (op == BTCompareOp.More)
        {
            return b1.value > b2.value ? SucessResult : FailureResult;
        }
        else if (op == BTCompareOp.MoreEqual)
        {
            return b1.value >= b2.value ? SucessResult : FailureResult;
        }
        return base.Update();
    }
}