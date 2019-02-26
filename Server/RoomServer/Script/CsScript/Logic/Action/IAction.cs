using System;
using System.Collections.Generic;

public enum ChaActionType
{
    SkillMove,
    Dead,
    Count
}

public partial class IAction
{
    public IAction()
    {
        
    }

    public IAction(ChaActionType type)
    {
        mType = type;
    }

    public virtual void Enter() { }

    public virtual void Exit() { }

    //返回false，Action结束
    public virtual bool LogicTick() { return true; }

    public ChaActionType mType
    {
        get;
        private set;
    }
}