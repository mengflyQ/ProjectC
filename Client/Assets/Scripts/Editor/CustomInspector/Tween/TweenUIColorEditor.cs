using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenUIColor))]
public class TweenUIColorEditor : TweenBaseEditor
{
    public override void OnInspectorGUI()
    {
        TweenUIColor tween = target as TweenUIColor;
        tween.from = EditorGUILayout.ColorField("From", tween.from);
        tween.to = EditorGUILayout.ColorField("To", tween.to);

        base.OnInspectorGUI();
    }
}