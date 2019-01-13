using System;
using System.Collections.Generic;

public class ActionContainer
{
    public void AddAction(IAction action)
    {
        if (action == null)
            return;
        IAction lastAction = mActions[(int)action.mType];
        mActions[(int)action.mType] = action;
        if (lastAction != null)
        {
            lastAction.Exit();
            IAction.DeleteAction(lastAction);
        }
        action.Enter();
    }

    public void DelAction(ChaActionType type)
    {
        IAction action = mActions[(int)type];
        if (action == null)
            return;
        action.Exit();
        mActions[(int)type] = null;
        IAction.DeleteAction(action);
    }

    public IAction GetAction(ChaActionType type)
    {
        return mActions[(int)type];
    }

    public bool HasAction(ChaActionType type)
    {
        IAction action = mActions[(int)type];
        return action != null;
    }

    public void LogicTick()
    {
        for (int i = 0; i < mActions.Length; ++i)
        {
            IAction action = mActions[i];
            if (action == null)
                continue;
            if (!action.LogicTick())
            {
                action.Exit();
                mActions[(int)action.mType] = null;
                IAction.DeleteAction(action);
            }
        }
    }

    public void RenderTick()
    {
        for (int i = 0; i < mActions.Length; ++i)
        {
            IAction action = mActions[i];
            if (action == null)
                continue;
            if (!action.RenderTick())
            {
                action.Exit();
                mActions[(int)action.mType] = null;
                IAction.DeleteAction(action);
            }
        }
    }

    IAction[] mActions = new IAction[(int)ChaActionType.Count];
}