using System;
using System.Collections.Generic;

public enum ChaActionType
{
    SkillMove,
    Count
}

public partial class IAction
{
    public IAction()
    {
        
    }

    public virtual void SetType(ChaActionType etype) { mType = etype; }

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