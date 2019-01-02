using UnityEngine;
using System;
using System.Collections.Generic;

public static class SkillEventRegister
{
    static void Hit(Character cha, SkillContext context, excel_skill_event e)
    {
        int hitID = e.evnetParam1;
        SkillHit.Hit(context.mOwner, hitID, context);
    }

    static void PlayAnimation(Character cha, SkillContext context, excel_skill_event e)
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

    public static void Initialize()
    {
        events[SkillEventType.Hit]                      = Hit;
        events[SkillEventType.PlayAnimation]            = PlayAnimation;
    }
    public delegate void SkillEventMethod(Character cha, SkillContext context, excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventMethod> events = new Dictionary<SkillEventType, SkillEventMethod>();
}