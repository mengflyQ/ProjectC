using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenRotation))]
public class TweenRotationEditor : TweenBaseEditor
{
    public override void OnInspectorGUI()
    {
        TweenRotation tween = target as TweenRotation;
        tween.transferType = (TweenTransformType)EditorGUILayout.EnumPopup("Transfer Type", tween.transferType);
        tween.from = EditorGUILayout.Vector3Field("From", tween.from);
        tween.to = EditorGUILayout.Vector3Field("To", tween.to);

        base.OnInspectorGUI();
    }
}