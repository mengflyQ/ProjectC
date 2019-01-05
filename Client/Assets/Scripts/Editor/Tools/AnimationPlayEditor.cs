using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class AnimationPlayEditor : EditorWindow
{
    static AnimationPlayEditor mInstance = null;

    Animation mAnimation = null;
    int mClipIndex = 0;
    AnimationClip mClip = null;
    float mCurTime = 0.0f;

    [MenuItem("Tools/动画查看器")]
    static void Init()
    {
        if (mInstance != null)
        {
            mInstance.Close();
            mInstance = null;
        }
        mInstance = GetWindow<AnimationPlayEditor>();
        mInstance.name = "动画查看器";
        mInstance.Show();
    }

    private void OnEnable()
    {
        EditorApplication.update += EditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= EditorUpdate;
    }

    void EditorUpdate()
    {
        if (mClip == null)
            return;
        mClip.SampleAnimation(mAnimation.gameObject, mCurTime);
    }

    private void OnGUI()
    {
        mAnimation = (Animation)EditorGUILayout.ObjectField("动画", mAnimation, typeof(Animation), true);
        if (mAnimation == null)
        {
            mClipIndex = 0;
            mCurTime = 0.0f;
            return;
        }
        AnimationClip[] clips = AnimationUtility.GetAnimationClips(mAnimation.gameObject);
        string[] clipNames = new string[clips.Length];
        int[] clipIndexs = new int[clips.Length];
        for (int i = 0; i < clipNames.Length; ++i)
        {
            clipNames[i] = clips[i].name;
            clipIndexs[i] = i;
        }
        mClipIndex = EditorGUILayout.IntPopup("动画名", mClipIndex, clipNames, clipIndexs);

        mClip = clips[mClipIndex];
        if (mClip != null)
        {
            mCurTime = EditorGUILayout.Slider("时间轴", mCurTime, 0.0f, mClip.length);
            EditorGUILayout.LabelField("当前帧", string.Format("{0}", mCurTime * mClip.frameRate));
        }
    }
}