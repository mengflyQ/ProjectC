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
    Hit = 1,
    [EnumDescription("创建子物体")]
    CreateChildObject = 2,
    [EnumDescription("重置目标位置")]
    ResetTargePos = 3,
    [EnumDescription("朝向目标位置")]
    TowardTargetPos = 4,

    // 客户端事件
    [EnumDescription("播放动画")]
    PlayAnimation = 1001,

    // 服务器事件
    [EnumDescription("技能移动")]
    SkillMove = 2001,
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

public enum SkillJumpType
{
    None,
    Stage,
    Skill
}

public enum ChildObjectTrait
{
    [EnumDescription("到达消失")]
    DestroyOnArrive,
    [EnumDescription("命中消失")]
    DestroyOnHit,
    [EnumDescription("技能施放者死亡消失")]
    DestroyOnSrcDead,
    [EnumDescription("目标死亡消失")]
    DestroyOnTargetDead,
}

public enum ChildObjectMoveType
{
    [EnumDescription("静止")]
    None,
    [EnumDescription("绑定技能施放者")]
    Bind,
    [EnumDescription("绑定目标")]
    BindPos,
    [EnumDescription("飞向目标")]
    FlyToTarget,
    [EnumDescription("飞向目标位置")]
    FlyToTargetPos,
    [EnumDescription("贴图飞向目标")]
    FlyToTarget_OnLand,
    [EnumDescription("贴地飞向目标位置")]
    FlyToTargetPos_OnLand,
}

public enum ChildObjectInitDirType
{
    [EnumDescription("技能施放者")]
    SrcDir,
    [EnumDescription("技能施放者挂点")]
    SrcHingeDir,
    [EnumDescription("目标")]
    TargetDir,
    [EnumDescription("目标挂点")]
    TargetHingeDir,
    [EnumDescription("大世界正前方")]
    WorldZ,
    [EnumDescription("上一个子物体")]
    CurSkillObjectDir,
}

public enum ChildObjectInitPosType
{
    [EnumDescription("技能施放者")]
    Src,
    [EnumDescription("技能施放者挂点")]
    SrcHinge,
    [EnumDescription("目标")]
    Target,
    [EnumDescription("目标挂点")]
    TargetHinge,
    [EnumDescription("目标位置")]
    TargetPos,
    [EnumDescription("上一个子物体")]
    CurSkillObject,
}

public enum TargetPosTestType
{
    None,
    LineTest,
    TargetInNav,
}

public enum SkillPreOpType
{
    [EnumDescription("点击施放")]
    Click,
    [EnumDescription("目标方向(直线)")]
    TargetDirLine,
    [EnumDescription("目标方向(扇形)")]
    TargetDirFan,
    [EnumDescription("目标位置")]
    TargetPos,
}

public enum SkillHurtType
{
    [EnumDescription("物理伤害")]
    PhyDamage,
    [EnumDescription("法术伤害")]
    MagDamage,
    [EnumDescription("治疗")]
    Cure
}