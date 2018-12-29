using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;
using MathLib;
using GameServer.RoomServer;

public enum CharacterType
{
    Player,
    Monster,
    NPC,
}

public partial class Character
{
    public Character()
    {
        
    }

    public virtual void Initialize()
    {
        InitMove();
    }

    public virtual void Update()
    {
        if (mCurSkill != null)
        {
            if (!mCurSkill.LogicTick())
            {
                SetSkill(null);
            }
        }
        UpdateMove();
    }

    public void SetTarget(Character target, bool sendToSelf = true)
    {
        int tid = 0;
        if (target != null)
        {
            tid = target.uid;
        }
        if (targetID != tid)
        {
            targetID = tid;

            for (int i = 0; i < mScene.GetPlayerCount(); ++i)
            {
                Player player = mScene.GetPlayerByIndex(i);
                if (player == null)
                    continue;
                if (!sendToSelf && player == this)
                    continue;
                ReqTargetChg targetChg = new ReqTargetChg();
                targetChg.uid = uid;
                targetChg.targetID = tid;
                NetWork.NotifyMessage<ReqTargetChg>(player.uid, STC.STC_TargetChg, targetChg);
            }
        }
    }

    public Character GetTarget()
    {
        return mScene.FindCharacter(TargetID);
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

    public Skill GetSkill()
    {
        return mCurSkill;
    }

    protected int TargetID
    {
        set;
        get;
    }

    public Character Target
    {
        set
        {
            
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

    public Vector3 Position
    {
        set
        {
            if (mPosition != value)
            {
                mPosDirty = true;
            }
            mPosition = value;
        }
        get
        {
            return mPosition;
        }
    }

    public float Radius
    {
        get
        {
            if (mChaList == null)
                return 0.0f;
            return mChaList.radius;
        }
    }

    public CharacterType Type
    {
        set;
        get;
    }

    public int uid;
    public string mNickName;
    public Scene mScene = null;
    public excel_cha_list mChaList = null;
    public int targetID;

    protected float mSpeed;
    protected Vector3 mDirection;
    protected Vector3 mPosition;
    protected bool mPosDirty = true;
    protected Skill mCurSkill = null;
}