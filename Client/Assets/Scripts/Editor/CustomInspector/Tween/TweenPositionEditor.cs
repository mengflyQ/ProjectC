using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenPosition))]
public class TweenPositionEditor : TweenBaseEditor
{
    public override void OnInspectorGUI()
    {
        TweenPosition tween = target as TweenPosition;
        EditorGUILayout.Vector3Field("From", tween.from);
        EditorGUILayout.Vector3Field("To", tween.to);

        base.OnInspectorGUI();
    }
}