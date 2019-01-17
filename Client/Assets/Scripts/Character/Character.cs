using UnityEngine;
using ProtoBuf;
using ZyGames.Framework.Common.Serialization;

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

public partial class Character : MonoBehaviour
{
	void Awake()
	{
		
	}

	void Start()
	{
        Initialize();
        OnInitMove();

        NetWork.SendPacket(CTS.CTS_ChaFinishInit, UserID, null);
        MessageSystem.Instance.MsgDispatch(MessageType.InitHeadBar, this);
	}

    private void OnDestroy()
    {
        Uninitialize();
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
    }

    protected virtual void Uninitialize()
    {
        if (HingePoints != null)
        {
            HingePoints.Uninitialize();
        }
    }

    public void SetTarget(Character target, bool msg = true)
    {
        int tid = 0;
        if (target != null)
        {
            tid = target.UserID;
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
                targetChg.uid = UserID;
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

    public int UserID
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

    public delegate void OnEvent(CharacterEventType evtType, Character self);
    public OnEvent mEvent = null;

    public UIHeadBar headBar = null;
}