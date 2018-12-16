using MathLib;
using System;
using System.Collections.Generic;

public class SkillContext
{
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

    public Character mOwner = null;
    public Character mHitTarget = null;
}