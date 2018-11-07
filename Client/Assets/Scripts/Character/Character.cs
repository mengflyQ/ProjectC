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
	}

	public Animation mAnimation;
}