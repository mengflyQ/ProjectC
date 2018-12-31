using UnityEngine;
using System.Collections;

public class FPS : MonoBehaviour
{
	void Awake()
	{
		Application.targetFrameRate = 30;
	}

	void Update ()
	{
		mTime += Time.deltaTime;
		if (mTime >= 1.0f)
		{
			mText.text = string.Format("FPS {0} NetDelay {1}", mTick, GameController.mNetDelay);
			mTime = 0.0f;
			mTick = 0;
			return;
		}
		++mTick;
	}

	float mTime;
	int mTick;
	public UnityEngine.UI.Text mText;
}
