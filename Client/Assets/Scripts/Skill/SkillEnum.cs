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

public enum SkillEventTriggerType
{
    [EnumDescription("帧触发")]
    Frame = 0,
    [EnumDescription("技能段开始触发")]
    StageBegin = 1,
    [EnumDescription("正常结束触发")]
    NormalEnd = 2,
    [EnumDescription("异常结束触发")]
    ExeptEnd = 3,
    [EnumDescription("最终结束触发")]
    FinalEnd = 4,
    [EnumDescription("循环帧触发")]
    Loop = 5,
    [EnumDescription("判定触发")]
    Hit = 6,

    Count,
}

public enum SkillEventType
{
    // 公共事件
    [EnumDescription("判定事件")]
    Hit                 = 1,
    
    // 客户端事件
    [EnumDescription("播放动画")]
    PlayAnimation       = 1001,

    // 服务器事件

}

public enum SkillState
{
    Failed,
    Break,
    Using,
    TrackEnemy
}

public enum SkillResult
{
    Success,
    ExcelNotExist,
    InvalidTarget,
    Unknown,
}

public enum DistanceCalcType
{
    Center,
    OuterA,
    OuterB,
    OuterAB
}

public enum SkillTargetType
{
    [EnumDescription("任何人")]
    All,
    [EnumDescription("自己")]
    Self,
    [EnumDescription("敌人")]
    Enemy,
    [EnumDescription("友方")]
    Friend,
    [EnumDescription("友方死亡")]
    FriendDead,
    [EnumDescription("中立")]
    Neutral,
    
    Count
}

public enum SkillBreakType
{
    None = 0,
    Move,
    Jump,
    FightPose,
    OtherSkill,
    AnimatorStateMachine,
}

public enum SkillStageTrait
{
    [EnumDescription("首技能段")]
    FirstStage,
    [EnumDescription("移动中断")]
    MoveBreak,
    [EnumDescription("受击中断")]
    HitBreak,
    [EnumDescription("允许移动")]
    AllowMove,
    [EnumDescription("技能段中断")]
    WaitBreakStage,
    [EnumDescription("技能中断技能")]
    SkillBreak,
    [EnumDescription("技能自打断")]
    BreakSelf,

    Count
}

public enum SkillEventTrait
{
    [EnumDescription("客户端事件")]
    Client,
    [EnumDescription("服务器事件")]
    Server,
    
    Count
}

public enum SkillJumpType
{
    None,
    Stage,
    Skill
}