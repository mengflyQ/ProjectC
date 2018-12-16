using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;
using MathLib;
using GameServer.RoomServer;

public class Character
{
    public virtual void Update()
    {
        if (mCurSkill != null)
        {
            if (!mCurSkill.LogicTick())
            {
                SetSkill(null);
            }
        }
        if (Speed > 0.0f)
        {
            Postion += Time.DeltaTime * mDirection;
        }
    }

    public void SetSkill(Skill skill)
    {
        Skill lastSkill = mCurSkill;
        mCurSkill = skill;
        if (lastSkill != null)
        {
            lastSkill.Exit();
        }
        if (mCurSkill != null)
        {
            mCurSkill.Enter();
        }
    }

    public int TargetID
    {
        set;
        get;
    }

    public Character Target
    {
        set
        {
            if (value == null)
            {
                targetID = 0;
                return;
            }
            targetID = value.uid;
        }
        get
        {
            Character target = mScene.FindCharacter(targetID);
            return target;
        }
    }

    public float Speed
    {
        set { mSpeed = value; }
        get { return mSpeed; }
    }

    public Vector3 Direction
    {
        set
        {
            mDirection = value;
            mDirection.y = 0.0f;
            mDirection.Normalize();
        }
        get
        {
            return mDirection;
        }
    }

    public Vector3 Postion
    {
        set
        {
            mPosition = value;
        }
        get
        {
            return mPosition;
        }
    }

    public int uid;
    public string mNickName;
    public Scene mScene = null;
    public excel_cha_list mChaList = null;
    public int targetID;

    protected float mSpeed;
    protected Vector3 mDirection;
    protected Vector3 mPosition;
    protected Skill mCurSkill = null;
}