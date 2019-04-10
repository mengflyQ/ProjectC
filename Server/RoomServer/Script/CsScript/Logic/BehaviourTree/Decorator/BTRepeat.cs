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

        Variable.LoadVariable(json, self, repeatTime, out repeatTime);
    }

    protected override BTStatus Update()
    {
        while (child != null)
        {
            child.Tick();
            if (child.IsRunning()) return BTStatus.Success;
            if (child.IsFailure()) return BTStatus.Failure;
            VariableInt repeatTimeInt = repeatTime as VariableInt;
            if (repeatTimeInt == null) return BTStatus.Invalid;
            if (++count == repeatTimeInt.value) return BTStatus.Success;
            child.Reset();
        }
        return BTStatus.Invalid;
    }

    public Variable repeatTime;

    protected int count;
}