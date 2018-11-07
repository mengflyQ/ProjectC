using UnityEngine;

public partial class Character : MonoBehaviour
{
	void OnInitMove()
	{
		mDirection = transform.forward;
		mLogicPosition = transform.position;
	}

	void UpdateMove()
	{
		if (mDirectionChg)
		{
			transform.forward = mDirection;
			mDirectionChg = false;
		}
		if (MoveSpeed > 0.0f)
		{
			Vector3 deltaPos = mSpeed * Time.fixedDeltaTime * mDirection;
			deltaPos.y = 0.0f;
			mLogicPosition += deltaPos;

			transform.position = mLogicPosition;
		}
	}

	public Vector3 Direction
	{
		set
		{
			if (mDirection == value)
				return;
			mDirection = value;
			mDirection.y = 0.0f;
			mDirection.Normalize();
			if (mDirection != Vector3.zero)
			{
				transform.forward = mDirection;
			}
			mDirectionChg = true;
		}
		get
		{
			return mDirection;
		}
	}

	public float MoveSpeed
	{
		set
		{
			mSpeed = value;
		}
		get
		{
			return mSpeed;
		}
	}

	float mSpeed = 0.0f;
	Vector3 mDirection = Vector3.forward;
	bool mDirectionChg = false;
	Vector3 mLogicPosition = Vector3.zero;
}