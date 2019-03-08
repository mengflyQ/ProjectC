using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public static class TriggerEventEditorRegister
{
    static void CheckParamCount(excel_trigger_event e, int count)
    {
        if (e.eventParams == null || e.eventParams.Length != count)
        {
            e.eventParams = new int[count];
        }
    }

    static int[] IntArrayField(string label, int[] array)
    {
        if (array == null)
        {
            array = new int[0];
        }
        EditorGUILayout.LabelField(label);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(16.0f);
        EditorGUILayout.BeginVertical();
        int len = array.Length;
        int oldLen = len;
        len = EditorGUILayout.IntField("数组大小", len);
        if (len != oldLen)
        {
            int[] newArray = new int[len];
            int minLen = Mathf.Min(len, oldLen);
            for (int i = 0; i < minLen; ++i)
            {
                newArray[i] = array[i];
            }
            array = newArray;
        }
        for (int i = 0; i < array.Length; ++i)
        {
            array[i] = EditorGUILayout.IntField("元素 " + i, array[i]);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        return array;
    }

    static void SceneRefreshNPC(excel_trigger_event e)
    {
        e.eventParams = IntArrayField("刷新ID", e.eventParams);
    }

    static TriggerEventEditorRegister()
    {
        mTriggerEventMethods[TriggerEventType.SceneRefreshNPC] = SceneRefreshNPC;
    }

    public delegate void TriggerEventEditorMethod(excel_trigger_event e);
    public static Dictionary<TriggerEventType, TriggerEventEditorMethod> mTriggerEventMethods = new Dictionary<TriggerEventType, TriggerEventEditorMethod>();
}
