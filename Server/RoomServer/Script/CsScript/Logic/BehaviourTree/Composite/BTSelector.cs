using System;

//选择器:依次执行每个子节点直到其中一个执行成功或者返回Running状态;
public class BTSelector : BTComposite
{
    public BTSelector(Character self)
        : base(self)
    {

    }

    protected override void Enter()
    {
        curChildIndex = 0;
    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);
    }

    protected override BTStatus Update()
    {
        while (curChildIndex < children.Count)
        {
            BTBehavior curChild = children[curChildIndex];
            if (curChild == null)
            {
                break;
            }
            BTStatus s = curChild.Tick();
            // 如果执行失败了就继续执行，否则返回;
            if (s != BTStatus.Failure)
                return s;
            if (++curChildIndex >= children.Count)
            {
                return BTStatus.Failure;
            }
        }
        return BTStatus.Invalid; // 循环意外终止;
    }

    protected int curChildIndex = 0;
}