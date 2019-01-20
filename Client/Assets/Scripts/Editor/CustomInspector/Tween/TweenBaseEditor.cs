using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TweenBase))]
public class TweenBaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TweenBase tween = target as TweenBase;

        if (GUIEditorTools.DrawHeader(guiTitle))
        {
            GUIEditorTools.BeginContents();
            GUIEditorTools.SetLabelWidth(110f);

            tween.style = (TweenStyle)EditorGUILayout.EnumPopup("Play Style", tween.style);

            tween.lerpMethod = (LerpMethod)EditorGUILayout.EnumPopup("Lerp Method", tween.lerpMethod);
            
            if (tween.lerpMethod == LerpMethod.Curve)
            {
                tween.curve = EditorGUILayout.CurveField("Animation Curve", tween.curve, GUILayout.Width(170f), GUILayout.Height(62f));
            }

            GUILayout.BeginHorizontal();
            tween.duration = EditorGUILayout.FloatField("Duration", tween.duration, GUILayout.Width(170f));
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            tween.stateDelay = EditorGUILayout.FloatField("Start Dlay", tween.stateDelay, GUILayout.Width(170f));
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();

            SerializedProperty propertyOnFinished = serializedObject.FindProperty("onFinished");
            EditorGUILayout.PropertyField(propertyOnFinished);

            GUIEditorTools.EndContents();
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected string guiTitle = "Base Tween";
}