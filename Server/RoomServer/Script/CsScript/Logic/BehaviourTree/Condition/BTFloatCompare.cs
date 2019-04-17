using System;

public class BTFloatCompare : BTCompare
{
    public BTFloatCompare(Character self)
        : base(self)
    {

    }

    protected override BTStatus Update()
    {
        VariableFloat b1 = v1 as VariableFloat;
        VariableFloat b2 = v2 as VariableFloat;
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