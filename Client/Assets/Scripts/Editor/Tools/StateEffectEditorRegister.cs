using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class StateEffectEditorRegister
{
    static void SetIntArrayCount(excel_state_effect e, int count)
    {
        if (e.dataIntArray == null)
        {
            e.dataIntArray = new int[count];
        }
        if (e.dataIntArray.Length != count)
        {
            e.dataIntArray = new int[count];
        }
    }

    ////////////////////////////////////////////////////////////////////////////////

    static void ModifyHp(excel_state_effect e)
    {
        SetIntArrayCount(e, 6);

        GUIContent c = null;

        bool isPct = (e.dataIntArray[0] != 0);
        isPct = EditorGUILayout.ToggleLeft("按百分比改变血量", isPct);
        e.dataIntArray[0] = isPct ? 1 : 0;

        if (isPct)
        {
            c = new GUIContent("最大血量百分比(%)", "填100表示100%，正数表示增加，负数表示减少");
            float pct = (float)e.dataIntArray[1] * 0.01f;
            pct = EditorGUILayout.FloatField(c, pct);
            e.dataIntArray[1] = (int)(pct * 100.0f);
        }
        else
        {
            c = new GUIContent("改变血量", "正数表示增加，负数表示减少");
            e.dataIntArray[1] = EditorGUILayout.IntField(c, e.dataIntArray[1]);
        }

        c = new GUIContent("改变间隔", "单位毫秒，填0则只改变一次");
        e.dataIntArray[5] = EditorGUILayout.IntField(c, e.dataIntArray[5]);

        c = new GUIContent("附加Atb属性ID", "填AtbID，以这个Atb为参考增加/减少血量，填0则不附加血量");
        e.dataIntArray[2] = EditorGUILayout.IntField(c, e.dataIntArray[2]);

        if (e.dataIntArray[2] > 0)
        {
            c = new GUIContent("附加Atb百分比", "根据该Atb的一定百分比改变血量，正数表示增加，负数表示减少，填100表示100%");
            float pct = (float)e.dataIntArray[3] * 0.01f;
            pct = EditorGUILayout.FloatField(c, pct);
            e.dataIntArray[3] = (int)(pct * 100.0f);

            StateItemModifyHp.UseAtbType useAtbType = (StateItemModifyHp.UseAtbType)e.dataIntArray[4];

            int[] values = Enum.GetValues(typeof(StateItemModifyHp.UseAtbType)) as int[];
            string[] texts = new string[values.Length];
            for (int i = 0; i < texts.Length; ++i)
            {
                texts[i] = ((StateItemModifyHp.UseAtbType)values[i]).ToDescription();
            }
            e.dataIntArray[4] = EditorGUILayout.IntPopup("Atb来源", e.dataIntArray[4], texts, values);
        }
    }
    
    static StateEffectEditorRegister()
    {
        mStateEffectMethods[StateItemType.ModifyHp] = ModifyHp;
    }
    public delegate void StateEffectEditorMethod(excel_state_effect e);
    public static Dictionary<StateItemType, StateEffectEditorMethod> mStateEffectMethods = new Dictionary<StateItemType, StateEffectEditorMethod>();
}