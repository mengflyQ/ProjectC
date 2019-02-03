using MathLib;
using System;
using System.Collections.Generic;

public enum SkillContextFlag
{
    SyncPosOnSkillEnd
}

public class SkillContext
{
    public void Reset()
    {
        mSkillID = 0;
        mOwner = null;
        mChildObject = null;
        mHitTarget = null;
        mChaHitCount.Clear();
        mContextFlag = 0;

        SkillTargetID = 0;
        TargetPos = Vector3.zero;
    }

    public void SetSkillContextFlag(SkillContextFlag flag, bool v)
    {
        if (v)
        {
            mContextFlag |= (1 << (int)flag);
        }
        else
        {
            mContextFlag &= ~(1 << (int)flag);
        }
    }

    public bool IsSkillContextFlag(SkillContextFlag flag)
    {
        int f = 1 << (int)flag;
        return (f & mContextFlag) != 0;
    }

    public Character SkillTarget
    {
        get
        {
            Character target = mOwner.mScene.FindCharacter(SkillTargetID);
            return target;
        }
    }

    public Vector3 TargetPos
    {
        set;
        get;
    }

    public int SkillTargetID
    {
        set;
        get;
    }

    public int mSkillID = 0;
    public Character mOwner = null;
    public ChildObject mChildObject = null;
    public Character mHitTarget = null;
    public Dictionary<int, Dictionary<Character, int>> mChaHitCount = new Dictionary<int,Dictionary<Character,int>>();
    public int mContextFlag = 0;
}