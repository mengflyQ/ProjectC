using System;
using MathLib;
using System.Collections.Generic;

public static class SkillEventRegister
{
    static void Hit(Character cha, ChildObject childObj, SkillContext context, excel_skill_event e)
    {
        int hitID = e.evnetParam1;
        SkillHit.Hit(context.mOwner, hitID, context);
    }

    static void SkillMove(Character cha, ChildObject childObj, SkillContext context, excel_skill_event e)
    {
        if (cha == null)
            return;
        SkillMoveDataType type = (SkillMoveDataType)e.evnetParam1;
        if (type == SkillMoveDataType.MoveType1)
        {
            float time = (float)e.evnetParam2 * 0.001f;
            SkillMove skillMove = IAction.CreateAction<SkillMove>();
            skillMove.Init1(cha, context.TargetPos, time);
            cha.AddAction(skillMove);
        }
        else if (type == SkillMoveDataType.MoveType2)
        {
            float time = (float)e.evnetParam2 * 0.001f;
            float speed = (float)e.evnetParam3 * 0.001f;

            Vector3 dir = context.TargetPos - cha.Position;

            SkillMove skillMove = IAction.CreateAction<SkillMove>();
            skillMove.Init2(cha, dir, speed, time);
            cha.AddAction(skillMove);
        }
        context.SetSkillContextFlag(SkillContextFlag.SyncPosOnSkillEnd, true);
    }

    static void TowardTargetPos(Character cha, ChildObject childObj, SkillContext context, excel_skill_event e)
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

    static void CreateChildObject(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e)
    {
        Vector3 offset = new Vector3(
            (float)e.evnetParam5 * 0.001f,
            (float)e.evnetParam6 * 0.001f,
            (float)e.evnetParam7 * 0.001f
        );

        Transform t = null;
        if (cha != null)
            t = cha.transform;
        else if (childObject != null)
            t = childObject.transform;
        if (t == null)
            return;

        if (e.evnetParam2 == 0)
        {
            ChildObject co = ChildObject.CreateChildObject(e.evnetParam1, context, t);
            if (co == null)
            {
                Debug.LogError("子物体发射失败，技能事件ID: " + e.id + "; 子物体ID: " + e.evnetParam1);
                return;
            }
            co.Position += co.transform.forward * offset.z;
            co.Position += co.transform.right * offset.x;
            co.Position += co.transform.up * offset.y;
            // context.mChildObjects.Add(co);
        }
        else
        {
            int num = e.evnetParam3;
            if (num <= 1)
            {
                Debug.LogError("发射数量小于等于1，请修改事件为发射单个子物体，技能事件ID: " + e.id);
            }
            float angle = (float)e.evnetParam4;
            float div = (num <= 1 ? 0.0f : 1.0f / (float)(num - 1));
            float half = (num <= 1 ? 0.0f : angle * 0.5f);

            float targetPosDist = (context.TargetPos - context.mOwner.Position).Length();

            Vector3 origTargetPos = context.TargetPos;

            for (int i = 0; i < num; ++i)
            {
                float yaw = angle * (float)i * div - half;
                Quaternion q = Quaternion.AngleAxis(yaw, Vector3.up);

                Vector3 dir = q * context.mOwner.Direction;
                context.TargetPos = context.mOwner.Position + dir.normalize * targetPosDist;

                ChildObject co = ChildObject.CreateChildObject(e.evnetParam1, context, t);
                if (co == null)
                {
                    Debug.LogError("子物体发射失败，技能事件ID: " + e.id + "; 子物体ID: " + e.evnetParam1);
                    return;
                }
                co.transform.forward = q * co.transform.forward;

                co.Position += co.transform.forward * offset.z;
                co.Position += co.transform.right * offset.x;
                co.Position += co.transform.up * offset.y;

                // context.mChildObjects.Add(co);
            }
        }
    }

    static void AddState(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e)
    {

    }

    public static void Initialize()
    {
        events[SkillEventType.Hit] = Hit;
        events[SkillEventType.SkillMove] = SkillMove;
        events[SkillEventType.TowardTargetPos] = TowardTargetPos;
        events[SkillEventType.CreateChildObject] = CreateChildObject;
    }
    public delegate void SkillEventMethod(Character cha, ChildObject childObj, SkillContext context, excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventMethod> events = new Dictionary<SkillEventType, SkillEventMethod>();
}