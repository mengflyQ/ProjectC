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

    static SkillEventEditorRegister()
    {
        mSkillEventMethods[SkillEventType.Hit]          = Hit;
    }
    public delegate void SkillEventEditorMethod(excel_skill_event e);
    public static Dictionary<SkillEventType, SkillEventEditorMethod> mSkillEventMethods = new Dictionary<SkillEventType, SkillEventEditorMethod>();
}