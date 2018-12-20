using UnityEngine;

public enum CharacterType
{
    Player,
    Monster,
    NPC,
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
            tid = target.ID;
        }
        if (TargetID != tid)
        {
            TargetID = tid;

            if (msg)
            {
                ReqTargetChg targetChg = new ReqTargetChg();
                targetChg.uid = ID;
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
		UpdateMove();
        UpdateAnim();
	}

    public CharacterType Type
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
            Scene scn = SceneSystem.Instance.mCurrentScene;
            return scn.GetCharacter(TargetID);
        }
    }

    public int ID
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
}