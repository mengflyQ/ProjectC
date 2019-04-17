using System;

public class BTCharacterCompare : BTCompare
{
    public BTCharacterCompare(Character self)
        : base(self)
    {

    }

    protected override BTStatus Update()
    {
        VariableCharacter b1 = v1 as VariableCharacter;
        VariableCharacter b2 = v2 as VariableCharacter;
        if (op == BTCompareOp.Equal)
        {
            return b1.gid == b2.gid ? SucessResult : FailureResult;
        }
        else if (op == BTCompareOp.NotEqual)
        {
            return b1.gid != b2.gid ? SucessResult : FailureResult;
        }
        return base.Update();
    }
}