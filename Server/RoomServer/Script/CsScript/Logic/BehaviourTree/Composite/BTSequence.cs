using System;

//顺序器：依次执行所有节点直到其中一个失败或者全部成功位置;
public class BTSequence : BTComposite
{
    public BTSequence(Character self)
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

        loop = json["Loop"].AsBool;
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
            // 如果执行成功了就继续执行，否则返回;
            if (s != BTStatus.Success)
                return s;
            if (++curChildIndex >= children.Count)
            {
                if (loop)
                {
                    Enter();
                }
                else
                {
                    return BTStatus.Success;
                }
            }
        }
        return BTStatus.Invalid; // 循环意外终止;
    }

    protected int curChildIndex = 0;
    protected bool loop = false;
}