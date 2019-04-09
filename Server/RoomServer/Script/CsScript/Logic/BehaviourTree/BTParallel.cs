using System;

public enum BTPolicy
{
    RequireOne, // 一个成功/失败;
    RequireAll, // 全部成功/失败;
}

//并行器：多个行为并行执行;
public class BTParallel : BTComposite
{
    public BTParallel(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        SuccessPolicy = (BTPolicy)json["SuccessPolicy"].AsInt;
        FailurePolicy = (BTPolicy)json["FailurePolicy"].AsInt;
        base.Load(json);
    }

    protected override BTStatus Update()
    {
        int successCount = 0;
        int failureCount = 0;
        for (int i = 0; i < children.Count; ++i)
        {
            BTBehavior behavior = children[i];
            if (!behavior.IsExit())
                behavior.Tick();

            if (behavior.IsSuccess())
            {
                ++successCount;
                if (SuccessPolicy == BTPolicy.RequireOne)
                {
                    behavior.Reset();
                    return BTStatus.Success;
                }
            }
            if (behavior.IsFailure())
            {
                ++failureCount;
                if (FailurePolicy == BTPolicy.RequireOne)
                {
                    behavior.Reset();
                    return BTStatus.Failure;
                }
            }
        }

        if (FailurePolicy == BTPolicy.RequireAll && failureCount == children.Count)
        {
            for (int i = 0; i < children.Count; ++i)
            {
                BTBehavior behavior = children[i];
                behavior.Reset();
            }

            return BTStatus.Failure;
        }

        if (SuccessPolicy == BTPolicy.RequireAll && successCount == children.Count)
        {
            for (int i = 0; i < children.Count; ++i)
            {
                BTBehavior behavior = children[i];
                behavior.Reset();
            }

            return BTStatus.Success;
        }
        return BTStatus.Running;
    }

    protected override void Exit(BTStatus s)
    {
        for (int i = 0; i < children.Count; ++i)
        {
            BTBehavior behavior = children[i];
            if (behavior.IsRunning())
            {
                behavior.Abort();
            }
        }
    }

    public BTPolicy SuccessPolicy // 成功策略;
    {
        set;
        get;
    }

    public BTPolicy FailurePolicy // 失败策略;
    {
        set;
        get;
    }
}