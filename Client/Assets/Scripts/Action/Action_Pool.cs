using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class IAction
{
    public static T CreateAction<T>(ChaActionType type) where T : IAction, new()
    {
        Queue<IAction> pool = null;
        if (!ActionPool.TryGetValue(type, out pool))
        {
            pool = new Queue<IAction>();
            ActionPool.Add(type, pool);
        }

        IAction action = null;
        if (pool.Count == 0)
        {
            action = new T();
            action.SetType(type);
        }
        else
        {
            action = pool.Dequeue();
        }
        if (action != null)
            return action as T;

        return null;
    }

    public static void DeleteAction(IAction action)
    {
        ChaActionType type = action.mType;
        Queue<IAction> pool = null;
        if (!ActionPool.TryGetValue(type, out pool))
        {
            return;
        }
        pool.Enqueue(action);
    }

    public static Dictionary<ChaActionType, Queue<IAction>> ActionPool = new Dictionary<ChaActionType, Queue<IAction>>();
}