using System;

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
    [EnumDescription("帧触发")]
    Hit = 1,
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
    InvalidCaster,
    CannotBreak,
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
    All,
    Self,
    Enemy,
    Friend,
    FriendDead,
    Neutral
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
    FirstStage,
    MoveBreak,
    HitBreak,
    AllowMove,
    WaitBreakStage,
    SkillBreak,
    BreakSelf
}


public enum SkillEventTrait
{
    Client,
    Server
}

public enum SkillJumpType
{
    None,
    Stage,
    Skill
}

public enum SkillHitShape
{
    [EnumDescription("矩形单体")]
    RectSingle,
    [EnumDescription("扇形单体")]
    FanSingle,
    [EnumDescription("圆形单体")]
    CircleSingle,

    [EnumDescription("矩形群体")]
    RectMultiple,
    [EnumDescription("扇形群体")]
    FanMultiple,
    [EnumDescription("圆形群体")]
    CircleMultiple,
}