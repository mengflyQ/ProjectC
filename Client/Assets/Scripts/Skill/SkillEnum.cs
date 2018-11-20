using System;

public enum SkillSelectCharactorType
{
    [EnumDescription("自己")]
    Self,
    [EnumDescription("目标")]
    FightTarget,
    [EnumDescription("命中目标")]
    HitTarget,
    [EnumDescription("技能目标")]
    SkillTarget,

    Count
}