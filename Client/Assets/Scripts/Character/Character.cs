using UnityEngine;

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

public partial class Character : MonoBehaviour
{
	void Awake()
	{
		
	}

	void Start()
	{
        Initialize();
        OnInitMove();
	}

    protected virtual void Initialize()
    {

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

    public float Radius
    {
        get
        {
            return mChaList.radius;
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
    
    public Animation mAnimation;
    public excel_cha_list mChaList;

    protected Skill mCurSkill = null;

    public delegate void OnEvent(CharacterEventType evtType, Character self);
    public OnEvent mEvent = null;
}