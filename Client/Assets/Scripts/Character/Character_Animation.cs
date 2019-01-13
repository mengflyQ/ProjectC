using UnityEngine;
using System.Collections.Generic;

public enum AnimPlayType
{
	Addition,
	Base,
	General,
	Up,
	Down,
	Priority,

	All
}

// 越靠前的动画优先级越高;
public enum AnimPriority
{
	Die,		// 死亡动画
	Forcedly,	// 硬直动画
	Buff,		// Buff动画
	Skill,		// 技能动画

	Count,
}

public partial class Character : MonoBehaviour
{
	public static string mBoneUpFull = "FootCenter/Bip01/Bip01 Pelvis/Bip01 Spine/Bip01 Spine1";
	public static string[] mBoneRoot = { "FootCenter", "Bip01", "Bip01 Pelvis" };
	public static int[] mAnimTypeLayer = { 4, 0, 1, 2, 2, 3 };
    // 上次播的某个名字的动画的类型;
    protected Dictionary<string, AnimPlayType> mLastPlayTypes = new Dictionary<string, AnimPlayType>();
	// 正在播放的动画;
	protected string[] mCurAnimNames = new string[(int)AnimPlayType.All];
    // 高优先级动画的优先级;
    protected float[] mPriorityStartTimes = new float[(int)AnimPriority.Count];
    protected float[] mPriorityLength = new float[(int)AnimPriority.Count];
    protected uint[] mAnimBaseMachineStates = null;

	public uint AnimBaseMachineID = 1;
	private float mAnimSpeed = 1.0f;
    private string mAnimPath = string.Empty;

    void InitAnim()
    {
        if (mChaList != null)
        {
            string path = mChaList.path;
            int first = path.IndexOf('/');
            int last = path.LastIndexOf('/');
            if (first < 0 || last < 0 || first >= last)
            {
                return;
            }
            mAnimPath = path.Substring(first, last - first);
            mAnimPath = "Animations" + mAnimPath + "/";
        }
    }

	public bool PlayPriorityAnimation(string name, AnimPriority priority, float speed = 1.0f, bool loop = false, bool reverse = false, float fadeLength = 0.15f, float time = 0.0f, bool async = true)
	{
		float curTime = Time.realtimeSinceStartup;
		for (int i = 0; i < (int)priority; ++i)
		{
			float length = mPriorityLength[i];
			if (length < 0)
				return false;
			float startTime = mPriorityStartTimes[i];
			if (length >= curTime - startTime)
			{
				return false;
			}
		}
		if (async)
		{
			// 未来Lambda改成Request
			ResourceSystem.LoadAsync<AnimationClip>(name, (o) => {
				AnimationClip c = o as AnimationClip;
				if (c == null)
					return;
				if (loop)
				{
					mPriorityLength[(int)priority] = -1.0f;
				}
				else
				{
					mPriorityLength[(int)priority] = c.length;
				}
				mPriorityStartTimes[(int)priority] = Time.realtimeSinceStartup;
				var state = PlayClipAnimation(c, c.name, AnimPlayType.Priority, speed, loop, reverse, fadeLength, time);
				if (state != null)
				{
					float sign = 1.0f;
					if (state.speed < 0.0f)
						sign = -1.0f;
					state.speed = mAnimSpeed * sign;
				}
			});
		}
		else
		{
            AnimationClip c = ResourceSystem.Load<AnimationClip>(name);
			if (c == null)
				return false;
			if (loop)
			{
				mPriorityLength[(int)priority] = -1.0f;
			}
			else
			{
				mPriorityLength[(int)priority] = c.length;
			}
			mPriorityStartTimes[(int)priority] = Time.realtimeSinceStartup;
			var state = PlayClipAnimation(c, c.name, AnimPlayType.Priority, speed, loop, reverse, fadeLength, time);
			if (state != null)
			{
				float sign = 1.0f;
				if (state.speed < 0.0f)
					sign = -1.0f;
				state.speed = mAnimSpeed * sign;
			}
		}
		return true;
	}

	public bool PlayAnimation(string path, AnimPlayType type, float speed = 1.0f, bool loop = false, bool reverse = false, float fadeLength = 0.15f, float time = 0.0f, bool async = true)
	{
		if (type == AnimPlayType.Priority)
		{
			Debug.LogError("高优先级动画请用PlayPriorityAnimation播放！");
			return false;
		}
		if (string.IsNullOrEmpty(path))
			return false;
		if (async)
		{
			// 未来Lambda改成Request
            ResourceSystem.LoadAsync<AnimationClip>(path, (o) =>
            {
                AnimationClip c = o as AnimationClip;
                if (c == null)
                    return;
                PlayClipAnimation(c, c.name, type, speed * mAnimSpeed, loop, reverse, fadeLength, time);
            });
		}
		else
		{
            AnimationClip c = ResourceSystem.Load<AnimationClip>(path);
			if (c == null)
				return false;
			PlayClipAnimation(c, c.name, type, speed * mAnimSpeed, loop, reverse, fadeLength, time);
		}
		return true;
	}

    public bool PlayAnimation(int id, AnimPlayType type, float speed = 1.0f, bool loop = false, bool reverse = false, float fadeLength = 0.15f, float time = 0.0f, bool async = true)
    {
        excel_anim_list animList = excel_anim_list.Find(id);
        if (animList == null)
        {
            Debug.LogError("未找到ID为" + id + "的动画表");
            return false;
        }
        string path = mAnimPath + animList.name;
        return PlayAnimation(path, type, speed, loop, reverse, fadeLength, time, async);
    }

    public bool PlayPriorityAnimation(int id, AnimPriority priority, float speed = 1.0f, bool loop = false, bool reverse = false, float fadeLength = 0.15f, float time = 0.0f, bool async = true)
    {
        excel_anim_list animList = excel_anim_list.Find(id);
        if (animList == null)
        {
            Debug.LogError("未找到ID为" + id + "的动画表");
            return false;
        }
        string path = mAnimPath + animList.name;
        return PlayPriorityAnimation(path, priority, speed, loop, reverse, fadeLength, time, async);
    }

    public AnimationState PlayClipAnimation(AnimationClip clip, string animName, AnimPlayType type = AnimPlayType.General,
		float speed = 1.0f, bool loop = false, bool reverse = false, float fadeLength = 0.3f, float time = 0.0f)
	{
		if (mAnimation == null)
			return null;
		if (!clip.legacy)
			clip.legacy = true;
		if (mLastPlayTypes.ContainsKey(animName))
		{
			AnimPlayType lastType = mLastPlayTypes[animName];
			if (lastType != type)
			{
				RemoveClip(animName, false);
			}
		}

		int layer = mAnimTypeLayer[(int)type];
		AnimationState state = mAnimation[animName];
		if (state != null && state.clip != clip)
		{
			RemoveClip(animName, false);
			state = mAnimation[animName];
		}

		if (state == null)
		{
			mAnimation.AddClip(clip, animName);
			mLastPlayTypes[animName] = type;
			state = mAnimation[animName];
			if (layer == 2)
			{
				MixingClip(state, type);
			}
		}
		else if (loop && state.wrapMode == WrapMode.Loop && mCurAnimNames[(int)type] == animName)
		{
			if ((reverse && state.speed < 0.0f) || (!reverse && state.speed > 0.0f))
			{
				return state;
			}
		}

		state.layer = layer;

		if (type == AnimPlayType.Addition)
		{
			state.blendMode = AnimationBlendMode.Additive;
			state.wrapMode = WrapMode.Once;
			mCurAnimNames[(int)type] = animName;
		}
		else
		{
			state.blendMode = AnimationBlendMode.Blend;
			if (loop)
			{
				state.wrapMode = WrapMode.Loop;
			}
			else if (type == AnimPlayType.Base)
			{
				// 播完停最后一帧;
				state.wrapMode = WrapMode.ClampForever;
			}
			else
			{
				state.wrapMode = WrapMode.Once;
			}

			if (layer == 1)
			{
				StopClip(type);
			}
			mCurAnimNames[(int)type] = animName;
		}

		if (reverse)
		{
			state.time = clip.length - time;
			state.speed = -speed;
		}
		else
		{
			state.time = time;
			state.speed = speed;
		}

		if (type == AnimPlayType.Addition)
		{
			state.weight = 1.0f;
			state.enabled = true;
		}
		else
		{
			if (layer == 2)
			{
				mAnimation.Blend(animName, 1, fadeLength);
			}
			else
			{
				mAnimation.CrossFade(animName, fadeLength);
			}
		}
		return state;
	}

	public void RemoveClip(string animName, bool unloadClip)
	{
		if (!string.IsNullOrEmpty(animName))
		{
			mAnimation.RemoveClip(animName);
		}
	}

	public void StopClip(AnimPlayType type = AnimPlayType.All)
	{
		if (type == AnimPlayType.All)
		{
			for (int i = 0; i < mCurAnimNames.Length; ++i)
			{
				string sAnimName = mCurAnimNames[i];
				StopClip(sAnimName);
				mCurAnimNames[i] = "";
			}
			return;
		}
		string animName = mCurAnimNames[(int)type];
		StopClip(animName);
		mCurAnimNames[(int)type] = "";
	}

	public void StopClip(string animName)
	{
		if (!string.IsNullOrEmpty(animName))
		{
			for (int i = 0; i < mCurAnimNames.Length; ++i)
			{
				string n = mCurAnimNames[i];
				if (n == animName)
				{
					mCurAnimNames[i] = "";
					break;
				}
			}
			mAnimation.Stop(animName);
		}
	}

    public void StopClip(int id)
    {
        excel_anim_list animList = excel_anim_list.Find(id);
        if (animList == null)
            return;
        StopClip(animList.name);
    }

    public void SetAnimTime(AnimPlayType type, float time)
    {
        string animName = mCurAnimNames[(int)type];
        if (string.IsNullOrEmpty(animName))
            return;
        AnimationState state = mAnimation[animName];
        if (state == null)
            return;
        state.time = time;
    }

	protected void MixingClip(AnimationState state, AnimPlayType type)
	{
		if (type == AnimPlayType.Up)
		{
			Transform up = mAnimation.transform.Find(mBoneUpFull);
			if (up != null)
			{
				state.AddMixingTransform(up);
			}
		}
		else if (type == AnimPlayType.Down)
		{
			int i = 0;
			Transform root = mAnimation.transform;
			do
			{
				Transform t = root.Find(mBoneRoot[i]);
				if (t != null)
				{
					state.AddMixingTransform(t, false);
				}
				++i;
				root = t;
			}
			while (root != null && i < mBoneRoot.Length);
			if (root != null)
			{
				for (i = 0; i < root.childCount; ++i)
				{
					Transform t = root.GetChild(i);
					if (t.name.Equals("Bip01 Spine"))
					{
						state.AddMixingTransform(t, false);
						for (int j = 0; j < t.childCount; ++j)
						{
							Transform linkspine = t.GetChild(j);
							if (linkspine.name.Contains("Bip"))
							{
								if (!linkspine.name.Contains("Spine1"))
								{
									state.AddMixingTransform(linkspine);
								}
							}
						}
					}
					else if (t.name.Contains("Bone"))
					{
						state.AddMixingTransform(t);
					}	
				}
			}
		}
	}
}