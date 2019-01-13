﻿using System;

public partial class Character
{
    void LogicTickAction()
    {
        mContainer.LogicTick();
    }

    public void AddAction(IAction action)
    {
        mContainer.AddAction(action);
    }

    public void DelAction(ChaActionType type)
    {
        mContainer.DelAction(type);
    }

    public IAction GetAcion(ChaActionType type)
    {
        return mContainer.GetAction(type);
    }

    public bool HasAction(ChaActionType type)
    {
        return mContainer.HasAction(type);
    }

    ActionContainer mContainer = new ActionContainer();
}