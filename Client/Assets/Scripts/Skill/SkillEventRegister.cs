﻿using UnityEngine;
using System;
using System.Collections.Generic;

public static class SkillEventRegister
{
    static void Hit(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e)
    {
        int hitID = e.evnetParam1;
        SkillHit.Hit(context.mOwner, hitID, context);
    }

    static void PlayAnimation(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e)
    {
        if (cha == null)
            return;

        int id = e.evnetParam1;
        int trait = e.evnetParam2;

        bool loop = ((trait & 0x1) != 0);
        bool reverse = ((trait & 0x2) != 0);
        bool onlyup = ((trait & 0x4) != 0);
        bool highPriority = ((trait & 0x8) != 0);

        float time = 0.03333f * (float)e.evnetParam3;
        float speed = 1.0f;
        if (e.evnetParam4 > 0)
        {
            speed = (float)e.evnetParam5 * 0.001f;
        }

        AnimPlayType animType = AnimPlayType.General;
        if (onlyup)
        {
            animType = AnimPlayType.Up;
            cha.PlayAnimation(id, animType, speed, loop, reverse, 0.0f, time);
        }
        else
        {
            if (highPriority)
            {
                cha.PlayPriorityAnimation(id, AnimPriority.Skill, speed, loop, reverse, 0.0f, time);
            }
            else
            {
                cha.PlayAnimation(id, AnimPlayType.General, speed, loop, reverse, 0.0f, time);
            }
        }
        context.mPlayingAnimations.Add(id);
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
            co.Position += co.transform.rotation * offset;
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

            float targetPosDist = (context.TargetPos - context.mOwner.Position).magnitude;

            Vector3 origTargetPos = context.TargetPos;

            for (int i = 0; i < num; ++i)
            {
                float yaw = angle * (float)i * div - half;
                Quaternion q = Quaternion.AngleAxis(yaw, Vector3.up);

                Vector3 dir = q * context.mOwner.Direction;
                context.TargetPos = context.mOwner.Position + dir.normalized * targetPosDist;

                ChildObject co = ChildObject.CreateChildObject(e.evnetParam1, context, t);
                if (co == null)
                {
                    Debug.LogError("子物体发射失败，技能事件ID: " + e.id + "; 子物体ID: " + e.evnetParam1);
                    return;
                }
                co.transform.localRotation *= q;
                co.Position += co.transform.rotation * offset;
                // context.mChildObjects.Add(co);
            }
        }
    }

    static void ResetTargePos(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e)
    {
        SkillSelectCharactorType selType = (SkillSelectCharactorType)e.evnetParam1;
        Character selTarget = context.SelectCharactorByType(selType);
        if (selTarget == null)
        {
            return;
        }

        float fAngle = (float)e.evnetParam2;
        if (e.evnetParam3 > 0)
        {
            float fAngleR = (float)e.evnetParam3 * 2.0f;
            float fPct = UnityEngine.Random.Range(0.0f, 1.0f);
            fAngle += fPct * fAngleR - fAngleR;
        }

        Quaternion q = Quaternion.AngleAxis(fAngle, Vector3.up);
        Vector3 dir = selTarget.Direction;
        if (e.evnetParam4 > 0 && context.mOwner != selTarget)
        {
            dir = selTarget.Position - context.mOwner.Position;
            dir.y = 0.0f;
            dir.Normalize();
        }
        dir = q * dir;
        float dist = (float)e.evnetParam5 * 0.001f;
        Vector3 targetPos = selTarget.Position + dir * dist;

        TargetPosTestType testType = (TargetPosTestType)e.evnetParam6;
        switch (testType)
        {
            case TargetPosTestType.None:
                break;
            case TargetPosTestType.LineTest:
                {
                    uint layer = NavigationSystem.GetLayer(context.mOwner.Position);
                    if (NavigationSystem.LineCast(context.mOwner.Position, targetPos, layer, out targetPos))
                        break;
                    break;
                }
            case TargetPosTestType.TargetInNav:
                {
                    if (!NavigationSystem.IsInNavigation(targetPos))
                    {
                        targetPos = selTarget.Position;
                    }
                    break;
                }
        }
        context.TargetPos = targetPos;
    }

    static void SkillMoveEvent(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e)
    {
        if (cha == null)
            return;
        if (!GameApp.Instance.directGame)
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

    public static void Initialize()
    {
        events[SkillEventType.Hit]                      = Hit;
        events[SkillEventType.PlayAnimation]            = PlayAnimation;
        events[SkillEventType.CreateChildObject]        = CreateChildObject;
        events[SkillEventType.ResetTargePos]            = ResetTargePos;
        events[SkillEventType.SkillMove]                = SkillMoveEvent;
    }
    public delegate void SkillEventMethod(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventMethod> events = new Dictionary<SkillEventType, SkillEventMethod>();
}