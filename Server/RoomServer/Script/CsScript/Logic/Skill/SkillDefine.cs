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