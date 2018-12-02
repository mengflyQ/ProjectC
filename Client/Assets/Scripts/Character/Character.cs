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
		OnInitMove();
	}

    protected virtual void Initialize()
    {
    }

	void FixedUpdate()
	{
		UpdateMove();
        UpdateAnim();
	}

    public CharacterType mType;
    public Animation mAnimation;
    public excel_cha_list mChaList;
}