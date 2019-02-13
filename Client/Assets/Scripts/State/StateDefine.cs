using System;

public enum StateType
{
    System,
    Buff,
    Debuff
}

public enum StateMutexScope
{
    [EnumDescription("所有状态")]
    All,
    [EnumDescription("同一来源的状态")]
    SameSrc,
}

public enum StateItemType
{
    [EnumDescription("修改角色CannotFlag")]
    CannotFlag,
    [EnumDescription("恢复\\流失生命")]
    ModifyHp,

    Count
}