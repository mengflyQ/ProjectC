using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;
using MathLib;
using GameServer.RoomServer;

public enum CannotFlag
{
    CannotMove,
    CannotControl,
    CannotSkill,
    CannotSelected,
    CannotEnemySelected,
    CannotFriendSelected,
    CannotHurtByMagic,
    CannotHurtByPhysic,
    CannotBeHit,

    Count
}

public enum OptType
{
    Unknown,
    Skill,
    State,
    AI,
    Dead,

    Count
}

public enum FlagMemory
{
    SearchTargetType,
    SearchTargetCondition,

    Count
}

public partial class Character : GameObject
{
    void InitFlag()
    {
        for (int i = 0; i < (int)FlagMemory.Count; ++i)
        {
            mFlagMemory[i] = 0;
        }
    }

    public int GetFlagMemory(FlagMemory flag)
    {
        if ((int)flag >= (int)FlagMemory.Count) return 0;
        return mFlagMemory[(int)flag];
    }

    public void SetFlagMemory(FlagMemory flag, int value)
    {
        if ((int)flag >= (int)FlagMemory.Count) return;
        mFlagMemory[(int)flag] = value;
    }

    public virtual void SetCannotFlag(CannotFlag flag, OptType type, bool cannot)
    {
        int mask = mCannotFlag[(int)flag];
        if (cannot)
        {
            mask |= (1 << (int)type);
        }
        else
        {
            mask &= ~(1 << (int)type);
        }
        mCannotFlag[(int)flag] = mask;
    }

    public bool IsCannotFlag(CannotFlag flag)
    {
        int mask = mCannotFlag[(int)flag];
        return mask != 0;
    }

    public bool IsMaySelected(Character src)
    {
        if (IsCannotFlag(CannotFlag.CannotSelected))
        {
            return false;
        }
        return true;
    }

    private int[] mFlagMemory = new int[(int)FlagMemory.Count];
    private int[] mCannotFlag = new int[(int)CannotFlag.Count];
}