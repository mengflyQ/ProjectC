using UnityEngine;
using UnityEditor;
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

public class SkillExcelDomain
{
    public string fileName;
    public int minID;
    public int maxID;
}

public class SkillExcelDomainSet
{
    public string domainName = string.Empty;
    public SkillExcelDomain skillDomain = new SkillExcelDomain();
    public SkillExcelDomain skillStageDomain = new SkillExcelDomain();
    public SkillExcelDomain skillHitDomain = new SkillExcelDomain();
    public SkillExcelDomain skillEventDomain = new SkillExcelDomain();
}

public class SkillEditor : EditorWindow
{
    public static SkillEditor mInstance = null;

    int mCurSkillID = 0;
    int mCurSkillStageID = 0;
    int mCurHitID = 0;
    int mCurEventID = 0;

    List<int> expandSkillIDs = new List<int>();
    List<int> expandSkillStageIDs = new List<int>();
    List<int> expandSkillHitIDs = new List<int>();
    List<int> expandSkillEventIDs = new List<int>();

    public List<SkillExcelDomainSet> mSkillDomain = new List<SkillExcelDomainSet>();
    int mCurDomainIndex = 0;
    Vector2 mListViewScrollView = Vector2.zero;

    GUIStyle mSkillStyleNormal;
    GUIStyle mSkillStyleSelected;

   [MenuItem("Tools/技能编辑器")]
    static void CreateSkillEditor()
    {
        if (mInstance != null)
        {
            mInstance.Close();
            mInstance = null;
            return;
        }
        mInstance = EditorWindow.GetWindow<SkillEditor>(true, "技能编辑器");
        mInstance.Show();

        if (excel_skill_list.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("skill_list");
        }
        if (excel_skill_stage.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("skill_stage");
        }
        if (excel_skill_hit.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("skill_hit");
        }
        if (excel_skill_event.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("skill_event");
        }
    }

    private void OnEnable()
    {
        Texture2D selectedBackGround = new Texture2D(2, 2, TextureFormat.RGB24, false);
        selectedBackGround.SetPixels(new Color[] { Color.blue, Color.blue, Color.blue, Color.blue });
        selectedBackGround.Apply();

        mSkillStyleNormal = new GUIStyle("Label");
        mSkillStyleSelected = new GUIStyle("Label");
        mSkillStyleSelected.normal.background = selectedBackGround;

        string xmlPath = Application.dataPath;
        xmlPath = xmlPath.Substring(0, xmlPath.LastIndexOf("Assets"));
        xmlPath += "Tools/SkillEditor/skill_excel_domain.xml";
        string txtXml = string.Empty;
        using (FileStream fsRead = new FileStream(xmlPath, FileMode.Open))
        {
            int fsLen = (int)fsRead.Length;
            byte[] heByte = new byte[fsLen];
            fsRead.Read(heByte, 0, heByte.Length);
            txtXml = System.Text.Encoding.UTF8.GetString(heByte);
            fsRead.Close();
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(txtXml);

        XmlNodeList pages = xmlDoc.GetElementsByTagName("page");
        for (int i = 0; i < pages.Count; ++i)
        {
            XmlElement page = pages[i] as XmlElement;

            XmlNodeList domainList = null;
            SkillExcelDomainSet domainSet = new SkillExcelDomainSet();
            domainSet.domainName = page.GetAttribute("name");

            domainList = page.GetElementsByTagName("skill");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                SkillExcelDomain domain = new SkillExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.skillDomain = domain;
            }

            domainList = page.GetElementsByTagName("skillstage");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                SkillExcelDomain domain = new SkillExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.skillStageDomain = domain;
            }

            domainList = page.GetElementsByTagName("skillhit");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                SkillExcelDomain domain = new SkillExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.skillHitDomain = domain;
            }

            domainList = page.GetElementsByTagName("skillevent");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                SkillExcelDomain domain = new SkillExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.skillEventDomain = domain;
            }

            mSkillDomain.Add(domainSet);
        }
    }

    private void OnDestroy()
    {
        excel_skill_list.excelView = null;
        excel_skill_stage.excelView = null;
        excel_skill_hit.excelView = null;
        excel_skill_event.excelView = null;
    }

    private void OnGUI()
    {
        Rect wndRc = mInstance.position;

        BeginWindows();
        GUILayout.Window(1, new Rect(0.0f, 0.0f, wndRc.width / 4.0f, wndRc.height), OnListView, "技能列表");
        GUILayout.Window(2, new Rect(wndRc.width / 4.0f, 0.0f, wndRc.width * 3.0f / 4.0f, wndRc.height * 3.0f / 4.0f), OnDataView, "技能列表");
        EndWindows();
    }

    #region ListView
    void OnListView(int wndID)
    {
        List<string> domainName = new List<string>();
        for (int i = 0; i < mSkillDomain.Count; ++i)
        {
            SkillExcelDomainSet domainSet = mSkillDomain[i];
            if (domainSet == null)
                continue;
            domainName.Add(domainSet.domainName);
        }
        mCurDomainIndex = GUILayout.Toolbar(mCurDomainIndex, domainName.ToArray());
        
        if (GUILayout.Button("...", EditorStyles.boldLabel, GUILayout.Width(32.0f)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("添加技能"), false, SkillMenuEvent, "addSkill");
            menu.AddItem(new GUIContent("保存表格"), false, SkillMenuEvent, "save");
            menu.ShowAsContext();
        }

        mListViewScrollView = EditorGUILayout.BeginScrollView(mListViewScrollView);
        SkillList();
        EditorGUILayout.EndScrollView();
    }

    void SkillList()
    {
        SkillExcelDomainSet domainSet = mSkillDomain[mCurDomainIndex];
        if (domainSet == null)
            return;
        int minID = domainSet.skillDomain.minID;
        int maxID = domainSet.skillDomain.maxID;

        for (int i = 0; i < excel_skill_list.Count; ++i)
        {
            excel_skill_list skillExcel = excel_skill_list.GetByIndex(i);
            if (skillExcel.id < minID || skillExcel.id > maxID)
                continue;

            bool isExpand = expandSkillIDs.Contains(skillExcel.id);
            bool isSel = mCurSkillID == skillExcel.id;

            EditorGUILayout.BeginHorizontal();
            Rect rcfold = EditorGUILayout.GetControlRect(GUILayout.Width(12.0f));
            Rect rcbtn= EditorGUILayout.GetControlRect();
            EditorGUILayout.EndHorizontal();

            GUIStyle skillStyle = EditorStyles.foldout;
            bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", skillStyle);
            GUIStyle style = isSel ? mSkillStyleSelected : mSkillStyleNormal;
            if (GUI.Button(rcbtn, "技能{" + skillExcel.id + "}::名称{" + skillExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurSkillID = skillExcel.id;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                }
            }
            if (bCurExpand && !isExpand)
            {
                expandSkillIDs.Add(skillExcel.id);
                GUI.FocusControl("");
            }
            else if (!bCurExpand && isExpand)
            {
                expandSkillIDs.Remove(skillExcel.id);
                GUI.FocusControl("");
            }

            if (bCurExpand)
            {                
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                if (GUILayout.Button("...", EditorStyles.boldLabel, GUILayout.Width(32.0f)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("添加技能段"), false, SkillMenuEvent, "addStage*" + skillExcel.id);
                    menu.ShowAsContext();
                }
                SkillStageList(skillExcel);
                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void SkillStageList(excel_skill_list skillExcel)
    {
        if (skillExcel == null || skillExcel.stages == null)
            return;
        for (int i = 0; i < skillExcel.stages.Length; ++i)
        {
            int stageID = skillExcel.stages[i];
            excel_skill_stage stageExcel = excel_skill_stage.Find(stageID);
            if (stageExcel == null)
                continue;

            bool isExpand = expandSkillStageIDs.Contains(stageExcel.id);
            bool isSel = mCurSkillStageID == stageExcel.id;

            EditorGUILayout.BeginHorizontal();
            Rect rcfold = EditorGUILayout.GetControlRect(GUILayout.Width(12.0f));
            Rect rcbtn = EditorGUILayout.GetControlRect();
            EditorGUILayout.EndHorizontal();

            GUIStyle skillStyle = EditorStyles.foldout;
            bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", skillStyle);
            GUIStyle style = isSel ? mSkillStyleSelected : mSkillStyleNormal;
            if (GUI.Button(rcbtn, "技能段{" + stageExcel.id + "}::名称{" + stageExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = stageExcel.id;
                    mCurHitID = 0;
                    mCurEventID = 0;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                }
            }
            if (bCurExpand && !isExpand)
            {
                expandSkillStageIDs.Add(stageExcel.id);
                GUI.FocusControl("");
            }
            else if (!bCurExpand && isExpand)
            {
                expandSkillStageIDs.Remove(stageExcel.id);
                GUI.FocusControl("");
            }

            if (bCurExpand)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();

                if (GUILayout.Button("...", EditorStyles.boldLabel, GUILayout.Width(32.0f)))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("添加技能事件"), false, SkillMenuEvent, "addEvent*" + stageExcel.id);
                    menu.ShowAsContext();
                }
                SkillEventList(stageExcel);

                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void SkillEventList(excel_skill_stage stageExcel)
    {
        if (stageExcel == null || stageExcel.events == null)
            return;
        for (int i = 0; i < stageExcel.events.Length; ++i)
        {
            int eventID = stageExcel.events[i];
            excel_skill_event eventExcel = excel_skill_event.Find(eventID);
            if (eventExcel == null)
                continue;
            
            bool isSel = mCurEventID == eventExcel.id;

            EditorGUILayout.BeginHorizontal();
            Rect rcbtn = EditorGUILayout.GetControlRect();
            EditorGUILayout.EndHorizontal();

            GUIStyle skillStyle = EditorStyles.foldout;
            GUIStyle style = isSel ? mSkillStyleSelected : mSkillStyleNormal;
            if (GUI.Button(rcbtn, "● 事件{" + eventExcel.id + "}::名称{" + eventExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = eventExcel.id;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                }
            }
        }
    }

    int GetEmptySkillID()
    {
        SkillExcelDomainSet domainSet = mSkillDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        SkillExcelDomain domain = domainSet.skillDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_skill_list.Count; ++i)
        {
            excel_skill_list excel = excel_skill_list.GetByIndex(i);
            if (excel.id < domain.minID || excel.id > domain.maxID)
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

    int GetEmptySkillStageID()
    {
        SkillExcelDomainSet domainSet = mSkillDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        SkillExcelDomain domain = domainSet.skillStageDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_skill_stage.Count; ++i)
        {
            excel_skill_stage excel = excel_skill_stage.GetByIndex(i);
            if (excel.id < domain.minID || excel.id > domain.maxID)
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

    int GetEmptySkillEventID()
    {
        SkillExcelDomainSet domainSet = mSkillDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        SkillExcelDomain domain = domainSet.skillEventDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_skill_event.Count; ++i)
        {
            excel_skill_event excel = excel_skill_event.GetByIndex(i);
            if (excel.id < domain.minID || excel.id > domain.maxID)
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

    void SkillMenuEvent(object obj)
    {
        string strData = obj as string;
        if (string.IsNullOrEmpty(strData))
            return;
        string[] sdatas = strData.Split('*');
        if (sdatas == null || sdatas.Length < 1)
            return;
        string data0 = sdatas[0];
        if (data0 == "addSkill")
        {
            excel_skill_list newSkill = new excel_skill_list();
            newSkill.id = GetEmptySkillID();
            newSkill.name = "newSkill";
            excel_skill_list.excelView.Add(newSkill);
        }
        else if (data0 == "addStage")
        {
            int skillId = 0;
            if (!int.TryParse(sdatas[1], out skillId))
            {
                return;
            }
            excel_skill_list skillExcel = excel_skill_list.Find(skillId);
            if (skillExcel == null)
                return;
            excel_skill_stage newStage = new excel_skill_stage();
            newStage.id = GetEmptySkillStageID();
            newStage.name = "newStage";
            if (skillExcel.stages == null)
            {
                skillExcel.stages = new int[1];
            }
            else
            {
                int[] origList = skillExcel.stages.Clone() as int[];
                skillExcel.stages = new int[skillExcel.stages.Length + 1];
                origList.CopyTo(skillExcel.stages, 0);
            }
            skillExcel.stages[skillExcel.stages.Length - 1] = newStage.id;
            excel_skill_stage.excelView.Add(newStage);
        }
        else if (data0 == "addEvent")
        {
            int stageID = 0;
            if (!int.TryParse(sdatas[1], out stageID))
            {
                return;
            }
            excel_skill_stage stageExcel = excel_skill_stage.Find(stageID);
            if (stageExcel == null)
                return;
            excel_skill_event eventExcel = new excel_skill_event();
            eventExcel.id = GetEmptySkillEventID();
            eventExcel.name = "newEvent";
            if (stageExcel.events == null)
            {
                stageExcel.events = new int[1];
            }
            else
            {
                int[] origList = stageExcel.events.Clone() as int[];
                stageExcel.events = new int[stageExcel.events.Length + 1];
                origList.CopyTo(stageExcel.events, 0);
            }
            stageExcel.events[stageExcel.events.Length - 1] = eventExcel.id;
            excel_skill_event.excelView.Add(eventExcel);
        }
    }
    #endregion // ListView

    #region DataView
    void OnDataView(int wndID)
    {
        if (mCurSkillID == 0 && mCurSkillStageID == 0 && mCurHitID == 0 && mCurEventID == 0)
        {
            EditorGUILayout.LabelField("没有任何数据");
        }
        ShowSkillData();
        ShowSkillStageData();
        ShowSkillEventData();
    }

    void ShowSkillData()
    {
        if (mCurSkillID == 0)
            return;
        excel_skill_list skillExcel = excel_skill_list.Find(mCurSkillID);
        EditorGUILayout.LabelField("技能ID", string.Format("{0}", skillExcel.id));
        skillExcel.name = EditorGUILayout.TextField("技能名称", skillExcel.name);
        skillExcel.maxDistance = EditorGUILayout.FloatField("技能最大距离", skillExcel.maxDistance);

        int[] values = new int[(int)SkillTargetType.Count];
        string[] texts = new string[(int)SkillTargetType.Count];
        for (int i = 0; i < (int)SkillTargetType.Count; ++i)
        {
            values[i] = i;
            texts[i] = ((SkillTargetType)i).ToDescription();
        }
        skillExcel.targetType = EditorGUILayout.IntPopup("目标类型", (int)skillExcel.targetType, texts, values);
    }

    void ShowSkillStageData()
    {
        if (mCurSkillStageID == 0)
            return;
        excel_skill_stage stageExcel = excel_skill_stage.Find(mCurSkillStageID);

        EditorGUILayout.LabelField("技能段ID", string.Format("{0}", stageExcel.id));
        stageExcel.name = EditorGUILayout.TextField("技能段名称", stageExcel.name);
        stageExcel.time = EditorGUILayout.FloatField("技能段时间", stageExcel.time);
        stageExcel.nextStageID = EditorGUILayout.IntField("后续技能段", stageExcel.nextStageID);

        string[] texts = new string[(int)SkillStageTrait.Count];
        for (int i = 0; i < (int)SkillStageTrait.Count; ++i)
        {
            texts[i] = ((SkillStageTrait)i).ToDescription();
        }
        stageExcel.trait = MaskField("技能段特性", stageExcel.trait, texts);
    }

    void ShowSkillEventData()
    {
        if (mCurEventID == 0)
            return;
        excel_skill_event eventExcel = excel_skill_event.Find(mCurEventID);

        EditorGUILayout.LabelField("技能事件ID", string.Format("{0}", eventExcel.id));
        eventExcel.name = EditorGUILayout.TextField("技能事件名称", eventExcel.name);

        EditorGUILayout.Separator();

        int[] values = new int[(int)SkillEventTriggerType.Count];
        string[] texts = new string[(int)SkillEventTriggerType.Count];
        for (int i = 0; i < (int)SkillEventTriggerType.Count; ++i)
        {
            values[i] = i;
            texts[i] = ((SkillEventTriggerType)i).ToDescription();
        }
        eventExcel.triggerType = EditorGUILayout.IntPopup("触发类型", eventExcel.triggerType, texts, values);
        ShowSkillTriggerType(eventExcel, (SkillEventTriggerType)eventExcel.triggerType);

        EditorGUILayout.Separator();
        
        values = Enum.GetValues(typeof(SkillEventType)) as int[];
        texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            texts[i] = ((SkillEventType)values[i]).ToDescription();
        }
        eventExcel.eventType = EditorGUILayout.IntPopup("事件类型", eventExcel.eventType, texts, values);

        SkillEventEditorRegister.SkillEventEditorMethod method = null;
        if (SkillEventEditorRegister.mSkillEventMethods.TryGetValue((SkillEventType)eventExcel.eventType, out method))
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(12.0f);
            EditorGUILayout.BeginVertical();

            method(eventExcel);

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Separator();

        texts = new string[(int)SkillEventTrait.Count];
        for (int i = 0; i < (int)SkillEventTrait.Count; ++i)
        {
            texts[i] = ((SkillEventTrait)i).ToDescription();
        }
        eventExcel.trait = MaskField("技能事件特性", eventExcel.trait, texts);
    }

    void ShowSkillTriggerType(excel_skill_event eventInfo, SkillEventTriggerType type)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(12.0f);
        EditorGUILayout.BeginVertical();

        if (type == SkillEventTriggerType.Frame)
        {
            eventInfo.triggerParam1 = EditorGUILayout.IntField("执行帧", eventInfo.triggerParam1);
        }
        else if (type == SkillEventTriggerType.Hit)
        {
            eventInfo.triggerParam1 = EditorGUILayout.IntField("判定ID", eventInfo.triggerParam1);
        }
        else if (type == SkillEventTriggerType.Loop)
        {
            eventInfo.triggerParam1 = EditorGUILayout.IntField("开始帧", eventInfo.triggerParam1);
            eventInfo.triggerParam2 = EditorGUILayout.IntField("间隔帧", eventInfo.triggerParam2);
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    int MaskField(string label, int mask, string[] texts)
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
    #endregion
}

public static class EnumExtension
{
    public static string ToDescription(this SkillTargetType enumType)
    {
        Type type = typeof(SkillTargetType);
        FieldInfo info = type.GetField(enumType.ToString());
        if (info == null)
            return "Unkown";
        EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
        if (descAttribute != null)
        {
            return descAttribute.Description;
        }
        return type.ToString();
    }

    public static string ToDescription(this SkillStageTrait enumType)
    {
        Type type = typeof(SkillStageTrait);
        FieldInfo info = type.GetField(enumType.ToString());
        if (info == null)
            return "Unkown";
        EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
        if (descAttribute != null)
        {
            return descAttribute.Description;
        }
        return type.ToString();
    }

    public static string ToDescription(this SkillEventTrait enumType)
    {
        Type type = typeof(SkillEventTrait);
        FieldInfo info = type.GetField(enumType.ToString());
        if (info == null)
            return "Unkown";
        EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
        if (descAttribute != null)
        {
            return descAttribute.Description;
        }
        return type.ToString();
    }

    public static string ToDescription(this SkillEventTriggerType enumType)
    {
        Type type = typeof(SkillEventTriggerType);
        FieldInfo info = type.GetField(enumType.ToString());
        if (info == null)
            return "Unkown";
        EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
        if (descAttribute != null)
        {
            return descAttribute.Description;
        }
        return type.ToString();
    }

    public static string ToDescription(this SkillEventType enumType)
    {
        Type type = typeof(SkillEventType);
        FieldInfo info = type.GetField(enumType.ToString());
        if (info == null)
            return "Unkown";
        EnumDescriptionAttribute descAttribute = info.GetCustomAttributes(typeof(EnumDescriptionAttribute), true)[0] as EnumDescriptionAttribute;
        if (descAttribute != null)
        {
            return descAttribute.Description;
        }
        return type.ToString();
    }
}