using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

public enum SkillTargetType
{
    All,
    Self,
    Enemy,
    Friend,
    FriendDead,
    Neutral
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