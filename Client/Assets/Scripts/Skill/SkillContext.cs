using UnityEngine;
using System;
using System.Collections.Generic;

public class SkillContext
{
    public void Reset()
    {
        mSkillID = 0;
        mOwner = null;
        TargetPos = Vector3.zero;
        SkillTargetID = 0;
        mChaHitCount.Clear();
        mPlayingAnimations.Clear();
    }

    public Character SelectCharactorByType(SkillSelectCharactorType selType)
    {
        switch (selType)
        {
            case SkillSelectCharactorType.Self:
                return mOwner;
            case SkillSelectCharactorType.FightTarget:
                {
                    return mOwner.GetTarget();
                }
            case SkillSelectCharactorType.HitTarget:
                return mHitTarget;
            case SkillSelectCharactorType.SkillTarget:
                return SkillTarget;
        }
        return null;
    }

    public Character SkillTarget
    {
        get
        {
            Character target = SceneSystem.Instance.mCurrentScene.GetCharacter(SkillTargetID);
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
    public Dictionary<int, Dictionary<Character, int>> mChaHitCount = new Dictionary<int, Dictionary<Character, int>>();
    public List<int> mPlayingAnimations = new List<int>();
}