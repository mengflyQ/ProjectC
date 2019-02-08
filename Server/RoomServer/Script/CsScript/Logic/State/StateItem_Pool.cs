using System;
using System.Collections.Generic;

public partial class BaseStateItem
{
    private static BaseStateItem NewStateItem(StateItemType type)
    {
        BaseStateItem stateItem = null;
        switch (type)
        {
            case StateItemType.ModifyHp:
                stateItem = new StateItemModifyHp();
                break;
        }
        stateItem.stateItemType = type;

        return stateItem;
    }

    public static BaseStateItem CreateStateItem(StateItemType type)
    {
        Queue<BaseStateItem> pool = null;
        if (!StateItemPool.TryGetValue(type, out pool))
        {
            pool = new Queue<BaseStateItem>();
            StateItemPool.Add(type, pool);
        }

        BaseStateItem stateItem = null;
        if (pool.Count == 0)
        {
            stateItem = NewStateItem(type);
        }
        else
        {
            stateItem = pool.Dequeue();
        }
        if (stateItem != null)
            return stateItem;

        return null;
    }

    public static void DeleteAction(BaseStateItem stateItem)
    {
        StateItemType type = stateItem.stateItemType;
        Queue<BaseStateItem> pool = null;
        if (!StateItemPool.TryGetValue(type, out pool))
        {
            return;
        }
        pool.Enqueue(stateItem);
    }

    public static Dictionary<StateItemType, Queue<BaseStateItem>> StateItemPool = new Dictionary<StateItemType, Queue<BaseStateItem>>();
}