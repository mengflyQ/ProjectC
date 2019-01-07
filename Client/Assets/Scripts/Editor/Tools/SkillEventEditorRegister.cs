using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public static class SkillEventEditorRegister
{
    static void Hit(excel_skill_event e)
    {
        e.evnetParam1 = EditorGUILayout.IntField("判定ID", e.evnetParam1);
    }

    static void PlayAnimation(excel_skill_event e)
    {
        GUIContent c = new GUIContent("动画ID", "anim_list的ID");
        e.evnetParam1 = EditorGUILayout.IntField(c, e.evnetParam1);

        string[] traitTxts = new string[]
        {
            "循环播放", "逆向播放", "只播上身动画", "高优先级动画"
        };
        e.evnetParam2 = MaskField("播动画特性", e.evnetParam2, traitTxts);
        e.evnetParam3 = EditorGUILayout.IntField("起始帧", e.evnetParam3);

        bool chgSpeed = e.evnetParam4 > 0;
        chgSpeed = EditorGUILayout.ToggleLeft("改变速度", chgSpeed);
        e.evnetParam4 = chgSpeed ? 1 : 0;

        if (chgSpeed)
        {
            c = new GUIContent("动画速度", "1.0是正常速度");
            float speed = (float)e.evnetParam5 * 0.001f;
            speed = EditorGUILayout.FloatField(c, speed);
            e.evnetParam5 = (int)(speed * 1000.0f);
        }
    }

    static void CreateChildObject(excel_skill_event e)
    {
        GUIContent c = new GUIContent("子物体ID", "child_object_list的ID");
        e.evnetParam1 = EditorGUILayout.IntField(c, e.evnetParam1);

        bool multi = (e.evnetParam2 > 0);
        multi = EditorGUILayout.Toggle("发射多个子物体", multi);
        if (multi)
        {
            e.evnetParam3 = EditorGUILayout.IntField("子物体个数", e.evnetParam3);
            e.evnetParam4 = EditorGUILayout.IntField("间隔角度", e.evnetParam4);
        }

        Vector3 offset = new Vector3(
            (float)e.evnetParam5 * 0.001f,
            (float)e.evnetParam6 * 0.001f,
            (float)e.evnetParam7 * 0.001f
        );
        offset = EditorGUILayout.Vector3Field("初始偏移位置", offset);

        e.evnetParam5 = (int)(offset.x * 1000.0f);
        e.evnetParam6 = (int)(offset.y * 1000.0f);
        e.evnetParam7 = (int)(offset.z * 1000.0f);
    }

    static void ResetTargePos(excel_skill_event e)
    {
        int[] values = Enum.GetValues(typeof(SkillSelectCharactorType)) as int[];
        string[] texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            SkillSelectCharactorType t = (SkillSelectCharactorType)values[i];
            texts[i] = t.ToDescription();
        }
        e.evnetParam1 = EditorGUILayout.IntPopup("目标", e.evnetParam1, texts, values);
        
        texts = new string[] { "技能施放者到目标的方向", "目标正前方" };
        values = new int[] { 0, 1 };
        e.evnetParam4 = EditorGUILayout.IntPopup("偏移朝向", e.evnetParam4, texts, values);
        e.evnetParam2 = EditorGUILayout.IntSlider("偏移朝向附加角度", e.evnetParam2, 0, 180);
        e.evnetParam3 = EditorGUILayout.IntSlider("偏移朝向附加角度随机范围", e.evnetParam3, 0, 180);

        float dist = (float)e.evnetParam5 * 0.001f;
        dist = EditorGUILayout.FloatField("偏移距离", dist);
        e.evnetParam5 = (int)(dist * 1000.0f);

        texts = new string[]
        {
            "无",
            "边界检测",
            "是否在导航上"
        };
        values = new int[]
        {
            (int)TargetPosTestType.None,
            (int)TargetPosTestType.LineTest,
            (int)TargetPosTestType.TargetInNav
        };
        e.evnetParam6 = EditorGUILayout.IntPopup("导航检测", e.evnetParam6, texts, values);
    }

        static int MaskField(string label, int mask, string[] texts)
    {
        EditorGUILayout.BeginHorizontal("Box");
        GUILayout.Space(12.0f);
        EditorGUILayout.BeginVertical();
        int m = 0;
        for (int i = 0; i < texts.Length; ++i)
        {
            string text = texts[i];
            bool b = (mask & (1 << i)) != 0;
            b = EditorGUILayout.ToggleLeft("  " + text, b);
            if (b)
            {
                m |= (1 << i);
            }
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        return m;
    }

    static SkillEventEditorRegister()
    {
        mSkillEventMethods[SkillEventType.Hit]                      = Hit;
        mSkillEventMethods[SkillEventType.PlayAnimation]            = PlayAnimation;
        mSkillEventMethods[SkillEventType.CreateChildObject]        = CreateChildObject;
        mSkillEventMethods[SkillEventType.ResetTargePos]            = ResetTargePos;
    }
    public delegate void SkillEventEditorMethod(excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventEditorMethod> mSkillEventMethods = new Dictionary<SkillEventType, SkillEventEditorMethod>();
}