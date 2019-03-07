using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public static class TriggerConditionEditorRegister
{
    static void CheckParamCount(excel_trigger_condition condtion, int count)
    {
        if (condtion.condParams == null || condtion.condParams.Length != count)
        {
            condtion.condParams = new int[count];
        }
    }

    static void AND(excel_trigger_condition condition, TriggerEditor editor)
    {
        CheckParamCount(condition, 2);

        condition.condParams[0] = EditorGUILayout.IntField("如果", condition.condParams[0]);

        if (condition.condParams[0] == condition.id)
        {
            Debug.LogError("触发条件AND死循环");
            return;
        }

        excel_trigger_condition c1 = excel_trigger_condition.Find(condition.condParams[0]);
        if (c1 != null)
        {
            EditorGUILayout.LabelField("{");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(16.0f));
            EditorGUILayout.BeginVertical();

            TriggerConditionType type = (TriggerConditionType)c1.condition;
            TriggerConditionEditorMethod method = null;
            if (mTriggerConditionMethods.TryGetValue(type, out method))
            {
                method(c1, editor);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("}");
        }

        condition.condParams[1] = EditorGUILayout.IntField("并且", condition.condParams[1]);

        if (condition.condParams[1] == condition.id)
        {
            Debug.LogError("触发条件AND死循环");
            return;
        }

        excel_trigger_condition c2 = excel_trigger_condition.Find(condition.condParams[1]);
        if (c2 != null)
        {
            EditorGUILayout.LabelField("{");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(16.0f));
            EditorGUILayout.BeginVertical();

            TriggerConditionType type = (TriggerConditionType)c2.condition;
            TriggerConditionEditorMethod method = null;
            if (mTriggerConditionMethods.TryGetValue(type, out method))
            {
                method(c2, editor);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("}");
        }
    }

    static TriggerConditionEditorRegister()
    {
        mTriggerConditionMethods[TriggerConditionType.AND] = AND;
    }

    public delegate void TriggerConditionEditorMethod(excel_trigger_condition e, TriggerEditor editor);
    public static Dictionary<TriggerConditionType, TriggerConditionEditorMethod> mTriggerConditionMethods = new Dictionary<TriggerConditionType, TriggerConditionEditorMethod>();
}