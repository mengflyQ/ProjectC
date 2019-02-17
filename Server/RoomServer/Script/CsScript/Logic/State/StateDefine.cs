using System;

public enum StateType
{
    System,
    Buff,
    Debuff
}

public enum StateMutexScope
{
    All,
    SameSrc,
}

public enum StateItemType
{
    CannotFlag,
    ModifyHp,

    Count
}