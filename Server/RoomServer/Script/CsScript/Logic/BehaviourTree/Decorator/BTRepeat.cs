using System;

//重复执行子节点的装饰器;
public class BTRepeat : BTDecorator
{
    public BTRepeat(Character self)
        : base(self)
    {

    }

    protected override void Enter()
    {
        count = 0;
    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        repeatTime.Load(json, self);
    }

    protected override BTStatus Update()
    {
        while (child != null)
        {
            child.Tick();
            if (child.IsRunning()) return BTStatus.Success;
            if (child.IsFailure()) return BTStatus.Failure;
            if (++count == repeatTime.value) return BTStatus.Success;
            child.Reset();
        }
        return BTStatus.Invalid;
    }

    public VariableInt repeatTime;

    protected int count;
}