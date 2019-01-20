using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenUIAlpha))]
public class TweenUIAlphaEditor : TweenBaseEditor
{
    public override void OnInspectorGUI()
    {
        TweenUIAlpha tween = target as TweenUIAlpha;
        tween.from = EditorGUILayout.FloatField("From", tween.from);
        tween.to = EditorGUILayout.FloatField("To", tween.to);

        base.OnInspectorGUI();
    }
}