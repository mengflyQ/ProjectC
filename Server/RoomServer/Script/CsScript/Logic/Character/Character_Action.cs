using System;

public partial class Character : GameObject
{
    void LogicTickAction()
    {
        if (mContainer != null)
        {
            mContainer.LogicTick();
        }
    }

    public void AddAction(IAction action)
    {
        if (mContainer != null)
        {
            mContainer.AddAction(action);
        }
    }

    public void DelAction(ChaActionType type)
    {
        if (mContainer != null)
        {
            mContainer.DelAction(type);
        }
    }

    public IAction GetAcion(ChaActionType type)
    {
        if (mContainer != null)
        {
            return mContainer.GetAction(type);
        }
        return null;
    }

    public bool HasAction(ChaActionType type)
    {
        if (mContainer != null)
        {
            return mContainer.HasAction(type);
        }
        return false;
    }

    ActionContainer mContainer = new ActionContainer();
}