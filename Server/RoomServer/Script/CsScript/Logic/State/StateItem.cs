using System;
using System.Collections.Generic;

public partial class BaseStateItem
{
    public virtual void OnOverlay() { }

    public virtual void Enter() { }

    public virtual void LogicTick() { }

    public virtual void Exit() { }

    public StateItemType stateItemType;

    public excel_state_effect excel;

    public StateGroup stateGroup;
}