using UnityEngine;
using ProtoBuf;
using System.Collections.Generic;
using ZyGames.Framework.Common.Serialization;

public enum CharacterType
{
    Player,
    Monster,
    NPC,
}

public enum CharacterEventType
{
    OnInit,
    OnDead,
    OnDestory,
    OnTargetChg,
    OnAtbChg,
}

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

public enum DeadType
{
    Alive,
    Kill,
    Fadeout,
    Dissolve,
    Vanish
}

public partial class Character : MonoBehaviour
{
	void Awake()
	{
        mStateMgr = new StateMgr(this);
	}

	void Start()
	{
        Initialize();
        OnInitMove();
        InitGraphic();

        NetWork.SendPacket(CTS.CTS_ChaFinishInit, gid, null);
        MessageSystem.Instance.MsgDispatch(MessageType.InitHeadBar, this);
	}

    private void OnDestroy()
    {
        Uninitialize();
    }

    public virtual void Destroy()
    {
        if (mEvent != null)
        {
            mEvent(CharacterEventType.OnDestory, this);
        }
        if (headBar != null)
        {
            headBar.Destroy();
        }
        GameObject.Destroy(this);
    }

    protected virtual void Initialize()
    {
        HingePoints = GetComponent<HingePoints>();
        if (HingePoints != null)
        {
            HingePoints.Initialize();
        }
        InitAnim();
        InitAtb();

        if (mEvent != null)
        {
            mEvent(CharacterEventType.OnInit, this);
        }
    }

    protected virtual void Uninitialize()
    {
        if (HingePoints != null)
        {
            HingePoints.Uninitialize();
        }
        mTriggers.Clear();
    }

    public void SetTarget(Character target, bool msg = true)
    {
        int tid = 0;
        if (target != null)
        {
            tid = target.gid;
        }
        if (TargetID != tid)
        {
            TargetID = tid;
            if (mEvent != null)
            {
                mEvent(CharacterEventType.OnTargetChg, this);
            }

            if (msg)
            {
                ReqTargetChg targetChg = new ReqTargetChg();
                targetChg.uid = gid;
                targetChg.targetID = tid;
                NetWork.SendPacket<ReqTargetChg>(CTS.CTS_TargetChg, targetChg, null);
            }
        }
    }

    public Character GetTarget()
    {
        Scene scn = SceneSystem.Instance.mCurrentScene;
        return scn.GetCharacter(TargetID);
    }

	void FixedUpdate()
	{
        if (mCurSkill != null)
        {
            if (!mCurSkill.LogicTick())
            {
                SetSkill(null);
            }
        }
        if (mStateMgr != null)
        {
            mStateMgr.LogicTick();
        }
        UpdateMove();
        UpdateAnim();
        LogicTickAction();
	}

    private void Update()
    {
        RenderTickAction();
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

    public void SetDead(DeadType deadType)
    {
        if (deadType != DeadType.Alive)
        {
            IsDead = true;
        }
        else
        {
            IsDead = false;
        }

        DeadAction deadAction = IAction.CreateAction<DeadAction>();
        deadAction.Init(this, deadType);
        AddAction(deadAction);

        if (mEvent != null)
        {
            mEvent(CharacterEventType.OnDead, this);
        }
    }

    public float Radius
    {
        get
        {
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

    public CharacterType Type
    {
        set;
        get;
    }

    public int gid
    {
        set;
        get;
    }

    protected int TargetID
    {
        set;
        get;
    }

    public bool IsDead
    {
        set;
        get;
    }

    public HingePoints HingePoints
    {
        private set;
        get;
    }
    
    public Animation mAnimation;
    public excel_cha_list mChaList;

    protected Skill mCurSkill = null;

    private int[] mCannotFlag = new int[(int)CannotFlag.Count];

    public delegate void OnEvent(CharacterEventType evtType, Character self, params object[] datas);
    public OnEvent mEvent = null;

    public UIHeadBar headBar = null;
    public StateMgr mStateMgr;

    public List<Trigger> mTriggers = new List<Trigger>();
}