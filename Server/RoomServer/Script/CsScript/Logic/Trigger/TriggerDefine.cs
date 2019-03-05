using System;

public enum TriggerBindType
{
    [EnumDescription("场景")]
    Scene,
    [EnumDescription("指定玩家")]
    Player,
    [EnumDescription("指定NPC")]
    NPC,
}

public enum TriggerType
{
    [EnumDescription("场景时间")]
    ScnTime = 1,
    [EnumDescription("进入场景区域")]
    ScnAreaEnter,
    [EnumDescription("退出场景区域")]
    ScnAreaExit,

    [EnumDescription("角色出生")]
    CharacterBorn = 2001,
    [EnumDescription("角色死亡")]
    CharacterDie,
    [EnumDescription("角色进战")]
    CharacterEnterFight,
    [EnumDescription("角色脱战")]
    CharacterExitFight,
    [EnumDescription("角色技能开始")]
    CharacterBeginSkill,
    [EnumDescription("角色技能结束")]
    CharacterExitSkill,
    [EnumDescription("角色属性改变")]
    CharacterAtbChg,
    [EnumDescription("角色受伤")]
    CharacterHurt,
    [EnumDescription("角色被治疗")]
    CharacterCure,
    [EnumDescription("角色添加状态")]
    CharacterCure,
    [EnumDescription("角色删除状态")]
    CharacterCure,

    [EnumDescription("玩家点击技能")]
    PlayerClickSkill = 4001,
    [EnumDescription("玩家学习技能")]
    PlayerLearnSkill,
    [EnumDescription("玩家获得经验")]
    PlayerAddExp,
    [EnumDescription("玩家改变等级")]
    PlayerSetLevel,

    [EnumDescription("NPC刷新")]
    NPCRefresh = 6001,
}

public enum TriggerConditionType
{
    [EnumDescription("无")]
    None = 0,
    [EnumDescription("并且")]
    AND = 1,
    [EnumDescription("或者")]
    OR,
    [EnumDescription("否定")]
    NOT,

    [EnumDescription("角色有状态")]
    CharacterHasState = 2001,
    [EnumDescription("角色没有状态")]
    CharacterNotHasState,
    [EnumDescription("角色在放技能")]
    CharacterUsingSkill,
    [EnumDescription("角色不在放技能")]
    CharacterNotUsingSkill,
    [EnumDescription("角色生命大于")]
    CharacterHPMoreThan,
    [EnumDescription("角色生命小于")]
    CharacterHPLessThan,
}

public enum TriggerEventType
{
    [EnumDescription("场景色调")]
    SceneAmbient = 1,
    [EnumDescription("刷新NPC")]
    SceneRefreshNPC,

    [EnumDescription("添加状态")]
    CharacterAddState = 2001,
    [EnumDescription("删除状态")]
    CharacterDelState,
    [EnumDescription("使用技能")]
    CharacterUseSkill,

    [EnumDescription("玩家学习技能")]
    PlayerLearnSkill = 4001,
    [EnumDescription("玩家获得经验")]
    PlayerAddExp,
    [EnumDescription("玩家改变等级")]
    PlayerSetLevel,
}