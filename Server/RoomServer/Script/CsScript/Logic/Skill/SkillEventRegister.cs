using System;
using System.Collections.Generic;

public static class SkillEventRegister
{
    static void Hit(Character cha, SkillContext context, excel_skill_event e)
    {
        int hitID = e.evnetParam1;
        SkillHit.Hit(context.mOwner, hitID, context);
    }

    public static void Initialize()
    {
        events[SkillEventType.Hit] = Hit;
    }
    public delegate void SkillEventMethod(Character cha, SkillContext context, excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventMethod> events = new Dictionary<SkillEventType, SkillEventMethod>();
}