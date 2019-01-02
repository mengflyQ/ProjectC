using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Editor.AssetsTools
{
    public class AnimationTypeTools
    {
        [MenuItem("Assets/AssetsTools/To Legacy")]
        public static void ToLegacy()
        {
            AnimationClip[] clips = Selection.GetFiltered<AnimationClip>(SelectionMode.Deep);

            for (int i = 0; i < clips.Length; ++i)
            {
                AnimationClip clip = clips[i];
                clip.legacy = true;
                EditorUtility.CopySerialized(clip, clip);
                EditorUtility.SetDirty(clip);
            }
        }

    }
}
