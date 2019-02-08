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

public enum CharacterEventType
{
    OnTargetChg,
}

public enum CannotFlag
{
    CannotMove,
    CannotControl,
    CannotSkill,
    CannotSelected,

    Count
}

public enum OptType
{
    Unknown,
    Skill,

    Count
}

public partial class Character : GameObject
{
    public Character() : base()
    {
        mStateMgr = new StateMgr(this);
    }

    public virtual void Initialize()
    {
        InitAtb();
        InitMove();
    }

    public override void Update()
    {
        if (mCurSkill != null)
        {
            if (!mCurSkill.LogicTick())
            {
                SetSkill(null);
            }
        }
        UpdateMove();
        LogicTickAction();
        if (mAtb != null)
        {
            mAtb.Update();
        }
        if (mStateMgr != null)
        {
            mStateMgr.LogicTick();
        }
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

            if (mEvent != null)
            {
                mEvent(CharacterEventType.OnTargetChg, this);
            }

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

    protected int TargetID
    {
        set;
        get;
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
            transform.forward = value;
        }
        get
        {
            return transform.forward;
        }
    }

    public Vector3 Position
    {
        set
        {
            if (transform.position != value)
            {
                mPosDirty = true;
            }
            transform.position = value;
        }
        get
        {
            return transform.position;
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

    public float Height
    {
        get
        {
            return 1.8f;
        }
    }

    public bool IsDead
    {
        set;
        get;
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
    public StateMgr mStateMgr;

    protected float mSpeed;
    private bool mPosDirty = true;
    protected Skill mCurSkill = null;
    private int[] mCannotFlag = new int[(int)CannotFlag.Count];
    public delegate void OnEvent(CharacterEventType evtType, Character self);
    public OnEvent mEvent = null;
}