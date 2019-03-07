using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public static class TriggerTypeEditorRegister
{
    static TriggerTypeEditorRegister()
    {
    }

    public delegate void TriggerTypeEditorMethod(excel_trigger_list e);
    public static Dictionary<TriggerType, TriggerTypeEditorMethod> mTriggerTypeMethods = new Dictionary<TriggerType, TriggerTypeEditorMethod>();
}