using UnityEngine;

public partial class Character : MonoBehaviour
{
	void Awake()
	{
		
	}

	void Start()
	{
		OnInitMove();
	}

	void FixedUpdate()
	{
		UpdateMove();
        UpdateAnim();
	}

	public Animation mAnimation;
    public excel_cha_list mChaList;
}