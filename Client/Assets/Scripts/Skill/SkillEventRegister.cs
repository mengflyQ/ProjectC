using UnityEngine;
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

        public static void Initialize()
    {
        events[SkillEventType.Hit]                      = Hit;
        events[SkillEventType.PlayAnimation]            = PlayAnimation;
        events[SkillEventType.CreateChildObject]        = CreateChildObject;
    }
    public delegate void SkillEventMethod(Character cha, ChildObject childObject, SkillContext context, excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventMethod> events = new Dictionary<SkillEventType, SkillEventMethod>();
}