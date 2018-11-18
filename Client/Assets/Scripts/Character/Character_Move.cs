﻿using UnityEngine;

public partial class Character : MonoBehaviour
{
	void OnInitMove()
	{
		mDirection = transform.forward;
		mLogicPosition = transform.position;

        NavLayer = NavigationSystem.GetLayer(mLogicPosition);

        Debug.LogError("++++++++++++ " + NavLayer.ToString());
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

            float height;
            if (NavigationSystem.GetLayerHeight(mLogicPosition, NavLayer, out height))
            {
                mLogicPosition.y = height;
                Debug.LogError("**************** " + NavLayer.ToString());
            }

			mLogicPosition += deltaPos;

			transform.position = mLogicPosition;
		}
	}

    string GetAnimDirectory()
    {
        string[] dirs = mChaList.path.Split(new char[] { '/' }, System.StringSplitOptions.None);
        if (dirs.Length < 3)
            return null;
        string dir = dirs[dirs.Length - 2];
        return "Animations/" + dir;
    }

    void UpdateAnim()
    {
        string animDirectory = GetAnimDirectory();

        if (MoveSpeed > 0.0f)
        {
            excel_anim_list animList = excel_anim_list.Find(2);
            string path = animDirectory + "/" + animList.name;
            PlayAnimation(path, AnimPlayType.Base, 1.0f, true, async: false);
        }
        else
        {
            excel_anim_list animList = excel_anim_list.Find(1);
            string path = animDirectory + "/" + animList.name;
            PlayAnimation(path, AnimPlayType.Base, 1.0f, true, async: false);
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

    public uint NavLayer
    {
        private set;
        get;
    }

	float mSpeed = 0.0f;
	Vector3 mDirection = Vector3.forward;
	bool mDirectionChg = false;
	Vector3 mLogicPosition = Vector3.zero;
}