using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public partial class IAction
{
    public static T CreateAction<T>() where T : IAction, new()
    {
        Queue<IAction> pool = null;
        Type type = typeof(T);
        if (!ActionPool.TryGetValue(type, out pool))
        {
            pool = new Queue<IAction>();
            ActionPool.Add(type, pool);
        }

        IAction action = null;
        if (pool.Count == 0)
        {
            action = new T();
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
        Type type = action.GetType();
        Queue<IAction> pool = null;
        if (!ActionPool.TryGetValue(type, out pool))
        {
            return;
        }
        pool.Enqueue(action);
    }

    public static Dictionary<Type, Queue<IAction>> ActionPool = new Dictionary<Type, Queue<IAction>>();
}