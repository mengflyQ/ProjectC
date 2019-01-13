using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SkillMove : IAction
{
    public override void SetType(ChaActionType etype)
    {
        base.SetType(ChaActionType.SkillMove);
    }
}