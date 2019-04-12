using System;

public enum BTNodeType
{
    Entry = 0,

    Selector = 1,
    ActiveSelector,
    Sequence,
    Parallel,
    RandomSelector,
    RandomSequence,
    WeightRandomSelector,
    WeightRandomSequence,

    Repeat = 101,
    Success,
    Inverter,

    HaveTarget = 1001,
    IsUsingSkill,

    MoveTo = 2001,
    PatrolRange,
    PatrolPath,
    SearchTarget,
    CastSkill,
    Wait,
}

public enum BTStatus
{
    Invalid,    // 初始状态;
    Success,    // 成功;
    Failure,    // 失败;
    Running,    // 运行;
    Aborted,    // 终止;
}

public class BTBehavior
{
    public BTBehavior(Character self)
    {
        this.self = self;
    }

    public BTStatus Status
    {
        get
        {
            return status;
        }
    }

    public BTStatus Tick()
    {
        if (Status != BTStatus.Running)
        {
            Enter();
        }

        status = Update();

        if (Status !=BTStatus.Running)
        {
            Exit(status);
        }
        return status;
    }

    public void Reset() { status = BTStatus.Invalid; }
    public void Abort() { Exit(BTStatus.Aborted); status = BTStatus.Aborted; }

    public bool IsExit() { return status == BTStatus.Success || status == BTStatus.Failure; }
    public bool IsRunning() { return status == BTStatus.Running; }
    public bool IsSuccess() { return status == BTStatus.Success; }
    public bool IsFailure() { return status == BTStatus.Failure; }

    public virtual void AddChild(BTBehavior child) { }
    public virtual void Load(LitJson.JsonData json) { }

    protected virtual void Enter() { }
    protected virtual BTStatus Update() { return status; }
    protected virtual void Exit(BTStatus s) { }

    protected BTStatus status;
    protected Character self;
}