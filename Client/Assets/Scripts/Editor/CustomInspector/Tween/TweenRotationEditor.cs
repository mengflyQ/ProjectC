using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenRotation))]
public class TweenRotationEditor : TweenBaseEditor
{
    public override void OnInspectorGUI()
    {
        TweenRotation tween = target as TweenRotation;
        EditorGUILayout.Vector3Field("From", tween.from);
        EditorGUILayout.Vector3Field("To", tween.to);

        base.OnInspectorGUI();
    }
}