using System;
using MathLib;
using System.Collections.Generic;

public static class SkillEventRegister
{
    static void Hit(Character cha, SkillContext context, excel_skill_event e)
    {
        int hitID = e.evnetParam1;
        SkillHit.Hit(context.mOwner, hitID, context);
    }

    static void SkillMove(Character cha, SkillContext context, excel_skill_event e)
    {
        if (cha == null)
            return;
        SkillMoveDataType type = (SkillMoveDataType)e.evnetParam1;
        if (type == SkillMoveDataType.MoveType1)
        {
            float time = (float)e.evnetParam2 * 0.001f;
            SkillMove skillMove = IAction.CreateAction<SkillMove>(ChaActionType.SkillMove);
            skillMove.Init1(cha, context.TargetPos, time);
            cha.AddAction(skillMove);
        }
        else if (type == SkillMoveDataType.MoveType2)
        {
            float time = (float)e.evnetParam2 * 0.001f;
            float speed = (float)e.evnetParam3 * 0.001f;

            Vector3 dir = context.TargetPos - cha.Position;

            SkillMove skillMove = IAction.CreateAction<SkillMove>(ChaActionType.SkillMove);
            skillMove.Init2(cha, dir, speed, time);
            cha.AddAction(skillMove);
        }
    }

    static void TowardTargetPos(Character cha, SkillContext context, excel_skill_event e)
    {
        Vector3 ownerPos = Vector3.zero;
        if (cha != null)
            ownerPos = cha.Position;
        //else if (childObject != null)
        //    ownerPos = childObject.Position;
        else
            return;
        Vector3 dir = context.TargetPos - ownerPos;
        if (cha != null)
            cha.Direction = dir;
        //else if (childObject != null)
        //    childObject.Direction = dir;
    }

    public static void Initialize()
    {
        events[SkillEventType.Hit] = Hit;
        events[SkillEventType.SkillMove] = SkillMove;
        events[SkillEventType.TowardTargetPos] = TowardTargetPos;
    }
    public delegate void SkillEventMethod(Character cha, SkillContext context, excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventMethod> events = new Dictionary<SkillEventType, SkillEventMethod>();
}