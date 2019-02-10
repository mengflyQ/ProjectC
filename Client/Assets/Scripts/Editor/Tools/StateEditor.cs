using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StateEditor : EditorWindow
{
    public static StateEditor mInstance = null;

    List<int> expandStateGroups = new List<int>();
    List<int> expandStateEffects = new List<int>();

    int mCurStateGroup = 0;
    int mCurStateEffect = 0;

    Vector2 mListViewScrollView = Vector2.zero;

    GUIStyle mStateStyleNormal;
    GUIStyle mStateStyleSelected;

    [MenuItem("Tools/状态编辑器")]
    static void CreateStateEditor()
    {
        if (mInstance != null)
        {
            mInstance.Close();
            mInstance = null;
            return;
        }
        mInstance = EditorWindow.GetWindow<StateEditor>(true, "状态编辑器");
        mInstance.Show();

        if (excel_state_group.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("state_group");
        }
        if (excel_state_effect.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("state_effect");
        }
    }

    private void OnEnable()
    {
        Texture2D selectedBackGround = new Texture2D(2, 2, TextureFormat.RGB24, false);
        selectedBackGround.SetPixels(new Color[] { Color.blue, Color.blue, Color.blue, Color.blue });
        selectedBackGround.Apply();

        mStateStyleNormal = new GUIStyle("Label");
        mStateStyleSelected = new GUIStyle("Label");
        mStateStyleSelected.normal.background = selectedBackGround;


    }

    private void OnDestroy()
    {
        excel_state_group.excelView = null;
        excel_state_effect.excelView = null;
    }

    private void OnGUI()
    {
        Rect wndRc = mInstance.position;

        BeginWindows();
        GUILayout.Window(1, new Rect(0.0f, 0.0f, wndRc.width / 4.0f, wndRc.height), OnListView, "状态组列表");
        GUILayout.Window(2, new Rect(wndRc.width / 4.0f, 0.0f, wndRc.width * 3.0f / 4.0f, wndRc.height * 3.0f / 4.0f), OnDataView, "状态效果列表");
        EndWindows();
    }

    void OnListView(int wndID)
    {
        if (GUILayout.Button("...", EditorStyles.boldLabel, GUILayout.Width(32.0f)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("添加状态"), false, StateMenuEvent, "addState");
            menu.AddItem(new GUIContent("保存表格"), false, StateMenuEvent, "save");
            menu.ShowAsContext();
        }

        mListViewScrollView = EditorGUILayout.BeginScrollView(mListViewScrollView);

        StateList();

        EditorGUILayout.EndScrollView();
    }

    void StateList()
    {
        for (int i = 0; i < excel_state_group.Count; ++i)
        {
            excel_state_group stateExcel = excel_state_group.GetByIndex(i);

            bool isExpand = expandStateGroups.Contains(stateExcel.id);
            bool isSel = mCurStateGroup == stateExcel.id;

            EditorGUILayout.BeginHorizontal();
            Rect rcfold = EditorGUILayout.GetControlRect(GUILayout.Width(12.0f));
            Rect rcbtn = EditorGUILayout.GetControlRect();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("添加状态效果"), false, StateMenuEvent, "addEffect*" + stateExcel.id);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("删除状态"), false, StateMenuEvent, "delState*" + stateExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }
            GUIStyle skillStyle = EditorStyles.foldout;
            bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", skillStyle);
            GUIStyle style = isSel ? mStateStyleSelected : mStateStyleNormal;
            if (GUI.Button(rcbtn, "状态{" + stateExcel.id + "}::名称{" + stateExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurStateGroup = stateExcel.id;
                    mCurStateEffect = 0;
                }
                else
                {
                    mCurStateGroup = 0;
                    mCurStateEffect = 0;
                }
            }
            if (bCurExpand && !isExpand)
            {
                expandStateGroups.Add(stateExcel.id);
                GUI.FocusControl("");
            }
            else if (!bCurExpand && isExpand)
            {
                expandStateGroups.Remove(stateExcel.id);
                GUI.FocusControl("");
            }

            if (bCurExpand)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                StateEffectList(stateExcel);
                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void StateEffectList(excel_state_group stateExcel)
    {
        if (stateExcel == null || stateExcel.stateEffectIDs == null)
            return;
        for (int i = 0; i < stateExcel.stateEffectIDs.Length; ++i)
        {
            int effectID = stateExcel.stateEffectIDs[i];
            excel_state_effect effectExcel = excel_state_effect.Find(effectID);
            if (effectExcel == null)
                continue;

            bool isExpand = expandStateEffects.Contains(effectExcel.id);
            bool isSel = mCurStateEffect == effectExcel.id;

            EditorGUILayout.BeginHorizontal();
            Rect rcfold = EditorGUILayout.GetControlRect(GUILayout.Width(12.0f));
            Rect rcbtn = EditorGUILayout.GetControlRect();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除技能段"), false, StateMenuEvent, "delEffect*" + stateExcel.id + "*" + effectExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            GUIStyle skillStyle = EditorStyles.foldout;
            GUIStyle style = isSel ? mStateStyleSelected : mStateStyleNormal;
            if (GUI.Button(rcbtn, "● 状态效果{" + effectExcel.id + "}::名称{" + effectExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurStateGroup = 0;
                    mCurStateEffect = effectExcel.id;
                }
                else
                {
                    mCurStateGroup = 0;
                    mCurStateEffect = 0;
                }
            }
        }
    }

    int GetEmptyStateID()
    {
        int id = 1;
        for (int i = 0; i < excel_state_group.Count; ++i)
        {
            excel_state_group excel = excel_state_group.GetByIndex(i);
            if (excel.id < 1 || excel.id > 9999999)
                continue;
            if (id == excel.id)
            {
                ++id;
            }
            else
            {
                return id;
            }
        }
        return id;
    }

    int GetEmptyStateEffectID()
    {
        int id = 1;
        for (int i = 0; i < excel_state_effect.Count; ++i)
        {
            excel_state_effect excel = excel_state_effect.GetByIndex(i);
            if (excel.id < 1 || excel.id > 9999999)
                continue;
            if (id == excel.id)
            {
                ++id;
            }
            else
            {
                return id;
            }
        }
        return id;
    }

    void StateMenuEvent(object obj)
    {
        string strData = obj as string;
        if (string.IsNullOrEmpty(strData))
            return;
        string[] sdatas = strData.Split('*');
        if (sdatas == null || sdatas.Length < 1)
            return;
        string data0 = sdatas[0];
        if (data0 == "addState")
        {
            excel_state_group newState = new excel_state_group();
            newState.id = GetEmptyStateID();
            newState.name = "newState";
            excel_state_group.Add(newState);
        }
        else if (data0 == "delState")
        {
        }
        else if (data0 == "addEffect")
        {
            int stateID = 0;
            if (!int.TryParse(sdatas[1], out stateID))
            {
                return;
            }
            if (!expandStateGroups.Contains(stateID))
                expandStateGroups.Add(stateID);
            excel_state_group stateExcel = excel_state_group.Find(stateID);
            if (stateExcel == null)
                return;
            excel_state_effect newEffect = new excel_state_effect();
            newEffect.id = GetEmptyStateEffectID();
            newEffect.name = "newStage";
            if (stateExcel.stateEffectIDs == null)
            {
                stateExcel.stateEffectIDs = new int[1];
            }
            else
            {
                int[] origList = stateExcel.stateEffectIDs.Clone() as int[];
                stateExcel.stateEffectIDs = new int[stateExcel.stateEffectIDs.Length + 1];
                origList.CopyTo(stateExcel.stateEffectIDs, 0);
            }
            stateExcel.stateEffectIDs[stateExcel.stateEffectIDs.Length - 1] = newEffect.id;
            excel_state_effect.Add(newEffect);
        }
        else if (data0 == "delEffect")
        {
        }
        else if (data0 == "save")
        {

        }
    }

    void OnDataView(int wndID)
    {
        if (mCurStateGroup == 0 && mCurStateEffect == 0)
        {
            EditorGUILayout.LabelField("没有任何数据");
        }

        ShowStateData();
        ShowEffectData();
    }

    void ShowStateData()
    {
        if (mCurStateGroup == 0)
            return;
        GUIContent c = null;

        excel_state_group stateExcel = excel_state_group.Find(mCurStateGroup);
        EditorGUILayout.LabelField("状态ID", string.Format("{0}", stateExcel.id));
        stateExcel.name = EditorGUILayout.TextField("状态名称", stateExcel.name);

        c = new GUIContent("持续时间", "单位毫秒，填-1为永久存在");
        stateExcel.duration = EditorGUILayout.IntField(c, stateExcel.duration);

        int[] values = Enum.GetValues(typeof(StateType)) as int[];
        string[] texts = Enum.GetNames(typeof(StateType));

        stateExcel.type = EditorGUILayout.IntPopup("状态类型", stateExcel.type, texts, values);

        c = new GUIContent("状态图标", "目录下的路径");
        stateExcel.icon = EditorGUILayout.TextField(c, stateExcel.icon);

        EditorGUILayout.Separator();

        c = new GUIContent("互斥组ID", "同ID的状态为互斥状态，填0则都不进行互斥判断");
        stateExcel.mutexID = EditorGUILayout.IntField(c, stateExcel.mutexID);

        values = Enum.GetValues(typeof(StateMutexScope)) as int[];
        GUIContent[] contents = new GUIContent[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            StateMutexScope v = (StateMutexScope)values[i];
            contents[i] = new GUIContent(v.ToDescription());
        }

        c = new GUIContent("互斥检测范围", "在该范围内的状态才参与互斥判断");
        stateExcel.mutexScope = EditorGUILayout.IntPopup(c, stateExcel.mutexScope, contents, values);

        c = new GUIContent("互斥优先级", "当该状态与已有状态互斥时，保留优先级高的状态");
        stateExcel.mutexPriority = EditorGUILayout.IntField(c, stateExcel.mutexPriority);

        c = new GUIContent("互斥同级跳转ID", "当该状态与已有状态互斥，并且优先级相同，则可以跳转到其他状态，一般用作状态升级，填0则不跳转只替换");
        stateExcel.mutexNextID = EditorGUILayout.IntField(c, stateExcel.mutexNextID);
    }

    void ShowEffectData()
    {
        if (mCurStateEffect == 0)
            return;
        excel_state_effect effectExcel = excel_state_effect.Find(mCurStateEffect);
        EditorGUILayout.LabelField("效果ID", string.Format("{0}", effectExcel.id));
        effectExcel.name = EditorGUILayout.TextField("效果名称", effectExcel.name);

        StateItemType stateItemType = (StateItemType)effectExcel.type;

        int[] values = new int[(int)StateItemType.Count];
        string[] texts = new string[(int)StateItemType.Count];
        for (int i = 0; i < (int)StateItemType.Count; ++i)
        {
            StateItemType v = (StateItemType)i;
            values[i] = i;
            texts[i] = string.Format("{0:D2}. {1}", i, v.ToDescription());
        }
        effectExcel.type = EditorGUILayout.IntPopup("效果类型", effectExcel.type, texts, values);

        StateEffectEditorRegister.StateEffectEditorMethod method = null;
        if (StateEffectEditorRegister.mStateEffectMethods.TryGetValue(stateItemType, out method))
        {
            method(effectExcel);
        }
    }
}

public static class StateEnumExtension
{
    public static string ToDescription(this StateMutexScope enumType)
    {
        Type type = typeof(StateMutexScope);
        try
        {
            FieldInfo info = type.GetField(enumType.ToString());
            if (info == null)
                return "Unkown";
            EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
            if (descAttribute != null)
            {
                return descAttribute.Description;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        return type.ToString();
    }

    public static string ToDescription(this StateItemType enumType)
    {
        Type type = typeof(StateItemType);
        try
        {
            FieldInfo info = type.GetField(enumType.ToString());
            if (info == null)
                return "Unkown";
            EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
            if (descAttribute != null)
            {
                return descAttribute.Description;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
        return type.ToString();
    }

    public static string ToDescription(this StateItemModifyHp.UseAtbType enumType)
    {
        Type type = typeof(StateItemModifyHp.UseAtbType);
        try
        {
            FieldInfo info = type.GetField(enumType.ToString());
            if (info == null)
                return "Unkown";
            EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
            if (descAttribute != null)
            {
                return descAttribute.Description;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        return type.ToString();
    }
}