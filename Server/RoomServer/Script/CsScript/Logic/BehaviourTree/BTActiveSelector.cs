using System;

public class BTActiveSelector : BTSelector
{
    public BTActiveSelector(Character self)
        : base(self)
    {

    }

    protected override void Enter()
    {
        curChildIndex = children.Count - 1;
    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);
    }

    protected override BTStatus Update()
    {
        // 每次执行前先保存的当前节点;
        int preChildIndex = curChildIndex;
        // 调用父类Enter函数让选择器每次重新选取节点;
        base.Enter();
        BTStatus result = base.Update();
        // 如果优先级更高的节点成功执行或者原节点执行失败则终止当前节点;
        if (preChildIndex < children.Count & curChildIndex != preChildIndex)
        {
            BTBehavior pre = children[preChildIndex];
            pre.Abort();
        }
        return result;
    }
}