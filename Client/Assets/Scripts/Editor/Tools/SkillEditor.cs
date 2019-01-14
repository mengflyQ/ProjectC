using UnityEngine;
using UnityEditor;
using System;
using System.Text;
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
    public SkillExcelDomain childObjectDomain = new SkillExcelDomain();
}

public enum SkillEditorMode
{
    Skill,
    ChildObject
}

public class SkillEditor : EditorWindow
{
    public static SkillEditor mInstance = null;

    int mCurSkillID = 0;
    int mCurSkillStageID = 0;
    int mCurHitID = 0;
    int mCurEventID = 0;
    int mCurChildObjectID = 0;

    int mCurMode = 0;

    List<int> expandSkillIDs = new List<int>();
    List<int> expandSkillStageIDs = new List<int>();
    List<int> expandSkillHitIDs = new List<int>();
    List<int> expandSkillEventIDs = new List<int>();
    List<int> expandChildObjectIDs = new List<int>();

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
        if (excel_child_object.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("child_object");
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

            domainList = page.GetElementsByTagName("childobject");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                SkillExcelDomain domain = new SkillExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.childObjectDomain = domain;
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
        excel_child_object.excelView = null;
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
        string[] modeTexts = new string[] { "技能", "子物体" };
        mCurMode = GUILayout.Toolbar(mCurMode, modeTexts);

        SkillEditorMode mode = (SkillEditorMode)mCurMode;

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
            if (mode == SkillEditorMode.Skill)
                menu.AddItem(new GUIContent("添加技能"), false, SkillMenuEvent, "addSkill");
            else
                menu.AddItem(new GUIContent("添加子物体"), false, SkillMenuEvent, "addChildObj");
            menu.AddItem(new GUIContent("保存表格"), false, SkillMenuEvent, "save");
            menu.ShowAsContext();
        }

        mListViewScrollView = EditorGUILayout.BeginScrollView(mListViewScrollView);
        if (mode == SkillEditorMode.Skill)
        {
            SkillList();
        }
        else if (mode == SkillEditorMode.ChildObject)
        {
            ChildObjectList();
        }
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

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("添加技能段"), false, SkillMenuEvent, "addStage*" + skillExcel.id);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("删除技能"), false, SkillMenuEvent, "delSkill*" + skillExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

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
                    mCurChildObjectID = 0;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                    mCurChildObjectID = 0;
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
                SkillHitList(skillExcel);
                SkillStageList(skillExcel);
                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void ChildObjectList()
    {
        SkillExcelDomainSet domainSet = mSkillDomain[mCurDomainIndex];
        if (domainSet == null)
            return;
        int minID = domainSet.childObjectDomain.minID;
        int maxID = domainSet.childObjectDomain.maxID;

        for (int i = 0; i < excel_child_object.Count; ++i)
        {
            excel_child_object childObjExcel = excel_child_object.GetByIndex(i);
            if (childObjExcel.id < minID || childObjExcel.id > maxID)
                continue;

            bool isExpand = expandChildObjectIDs.Contains(childObjExcel.id);
            bool isSel = mCurChildObjectID == childObjExcel.id;

            EditorGUILayout.BeginHorizontal();
            Rect rcfold = EditorGUILayout.GetControlRect(GUILayout.Width(12.0f));
            Rect rcbtn = EditorGUILayout.GetControlRect();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("添加事件"), false, SkillMenuEvent, "addEvent*" + childObjExcel.id);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("删除子物体"), false, SkillMenuEvent, "delChildObject*" + childObjExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            GUIStyle skillStyle = EditorStyles.foldout;
            bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", skillStyle);
            GUIStyle style = isSel ? mSkillStyleSelected : mSkillStyleNormal;
            if (GUI.Button(rcbtn, "子物体{" + childObjExcel.id + "}::名称{" + childObjExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                    mCurChildObjectID = childObjExcel.id;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                    mCurChildObjectID = 0;
                }
            }
            if (bCurExpand && !isExpand)
            {
                expandChildObjectIDs.Add(childObjExcel.id);
                GUI.FocusControl("");
            }
            else if (!bCurExpand && isExpand)
            {
                expandChildObjectIDs.Remove(childObjExcel.id);
                GUI.FocusControl("");
            }

            if (bCurExpand)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                ChildObjEventList(childObjExcel);
                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void SkillHitList(excel_skill_list skillExcel)
    {
        EditorGUILayout.BeginHorizontal();
        Rect rcfold = EditorGUILayout.GetControlRect(GUILayout.Width(12.0f));
        Rect rcbtn = EditorGUILayout.GetControlRect();
        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
        {
            if (rcbtn.Contains(Event.current.mousePosition))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("添加判定表"), false, SkillMenuEvent, "addHit*" + skillExcel.id);
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        bool isExpand = expandSkillHitIDs.Contains(skillExcel.id);
        GUIStyle skillStyle = EditorStyles.foldout;
        bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", skillStyle);
        GUI.Button(rcbtn, "判定表", mSkillStyleNormal);

        if (bCurExpand && !isExpand)
        {
            expandSkillHitIDs.Add(skillExcel.id);
            GUI.FocusControl("");
        }
        else if (!bCurExpand && isExpand)
        {
            expandSkillHitIDs.Remove(skillExcel.id);
            GUI.FocusControl("");
        }
        if (!bCurExpand)
        {
            return;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.BeginVertical();
        if (skillExcel == null || skillExcel.hits == null)
        {
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return;
        }
        for (int i = 0; i < skillExcel.hits.Length; ++i)
        {
            int hitID = skillExcel.hits[i];
            excel_skill_hit hitExcel = excel_skill_hit.Find(hitID);
            if (hitExcel == null)
                continue;

            rcbtn = EditorGUILayout.GetControlRect();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除判定表"), false, SkillMenuEvent, "delHit*" + skillExcel.id + "*" + hitExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            bool isSel = mCurHitID == hitExcel.id;

            GUIStyle style = isSel ? mSkillStyleSelected : mSkillStyleNormal;
            if (GUI.Button(rcbtn, "◆ 判定表{" + hitExcel.id + "}::名称{" + hitExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = hitExcel.id;
                    mCurEventID = 0;
                    mCurChildObjectID = 0;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                    mCurChildObjectID = 0;
                }
            }
        }
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
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

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("添加技能事件"), false, SkillMenuEvent, "addEvent*" + stageExcel.id);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("删除技能段"), false, SkillMenuEvent, "delStage*" + skillExcel.id + "*" + stageExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

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
                    mCurChildObjectID = 0;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                    mCurChildObjectID = 0;
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

                SkillEventList(stageExcel);

                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    void ChildObjEventList(excel_child_object childObjExcel)
    {
        if (childObjExcel == null || childObjExcel.events == null)
            return;
        for (int i = 0; i < childObjExcel.events.Length; ++i)
        {
            int eventID = childObjExcel.events[i];
            excel_skill_event eventExcel = excel_skill_event.Find(eventID);
            if (eventExcel == null)
                continue;

            bool isSel = mCurEventID == eventExcel.id;

            Rect rcbtn = EditorGUILayout.GetControlRect();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除技能事件"), false, SkillMenuEvent, "delEvent*" + childObjExcel.id + "*" + eventExcel.id);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("上移"), false, SkillMenuEvent, "upEvent*" + childObjExcel.id + "*" + eventExcel.id);
                    menu.AddItem(new GUIContent("下移"), false, SkillMenuEvent, "downEvent*" + childObjExcel.id + "*" + eventExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

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
                    mCurChildObjectID = 0;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                    mCurChildObjectID = 0;
                }
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
            
            Rect rcbtn = EditorGUILayout.GetControlRect();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除技能事件"), false, SkillMenuEvent, "delEvent*" + stageExcel.id + "*" + eventExcel.id);
                    menu.AddSeparator("");
                    menu.AddItem(new GUIContent("上移"), false, SkillMenuEvent, "upEvent*" + stageExcel.id + "*" + eventExcel.id);
                    menu.AddItem(new GUIContent("下移"), false, SkillMenuEvent, "downEvent*" + stageExcel.id + "*" + eventExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

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
                    mCurChildObjectID = 0;
                }
                else
                {
                    mCurSkillID = 0;
                    mCurSkillStageID = 0;
                    mCurHitID = 0;
                    mCurEventID = 0;
                    mCurChildObjectID = 0;
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

    int GetEmptyChildObjectID()
    {
        SkillExcelDomainSet domainSet = mSkillDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        SkillExcelDomain domain = domainSet.childObjectDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_child_object.Count; ++i)
        {
            excel_child_object excel = excel_child_object.GetByIndex(i);
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

    int GetEmptySkillHitID()
    {
        SkillExcelDomainSet domainSet = mSkillDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        SkillExcelDomain domain = domainSet.skillHitDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_skill_hit.Count; ++i)
        {
            excel_skill_hit excel = excel_skill_hit.GetByIndex(i);
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

    void DeleteSkillEvent(int stageID, int skillEventID)
    {
        SkillEditorMode mode = (SkillEditorMode)mCurMode;
        if (mode == SkillEditorMode.Skill)
        {
            excel_skill_stage stageExcel = excel_skill_stage.Find(stageID);
            if (stageExcel == null)
                return;
            excel_skill_event eventExcel = excel_skill_event.Find(skillEventID);
            if (eventExcel == null)
                return;
            expandSkillEventIDs.Remove(skillEventID);
            if (mCurEventID == skillEventID)
                mCurEventID = 0;
            List<int> events = new List<int>(stageExcel.events);
            events.Remove(skillEventID);
            stageExcel.events = events.ToArray();
            excel_skill_event.excelView.Remove(eventExcel);
        }
        else
        {
            excel_child_object childObjExcel = excel_child_object.Find(stageID);
            if (childObjExcel == null)
                return;
            excel_skill_event eventExcel = excel_skill_event.Find(skillEventID);
            if (eventExcel == null)
                return;
            expandSkillEventIDs.Remove(skillEventID);
            if (mCurEventID == skillEventID)
                mCurEventID = 0;
            List<int> events = new List<int>(childObjExcel.events);
            events.Remove(skillEventID);
            childObjExcel.events = events.ToArray();
            excel_skill_event.excelView.Remove(eventExcel);
        }
    }

    void DeleteSkillHit(int skillID, int skillHitID)
    {
        excel_skill_list skillExcel = excel_skill_list.Find(skillID);
        if (skillExcel == null)
            return;
        excel_skill_hit skillHitExcel = excel_skill_hit.Find(skillHitID);
        if (skillHitExcel == null)
            return;
        if (mCurHitID == skillHitID)
            mCurHitID = 0;
        List<int> hits = new List<int>(skillExcel.hits);
        hits.Remove(skillHitID);
        skillExcel.hits = hits.ToArray();
        excel_skill_hit.excelView.Remove(skillHitExcel);
    }

    void DeleteSkillStage(int skillID, int skillStageID)
    {
        excel_skill_list skillExcel = excel_skill_list.Find(skillID);
        if (skillExcel == null)
            return;
        excel_skill_stage skillStageExcel = excel_skill_stage.Find(skillStageID);
        if (skillStageExcel == null)
            return;
        if (mCurSkillStageID == skillStageID)
            mCurSkillStageID = 0;
        if (skillStageExcel.events != null)
        {
            int[] events = skillStageExcel.events.Clone() as int[];
            for (int i = 0; i < events.Length; ++i)
            {
                int eventID = events[i];
                DeleteSkillEvent(skillStageID, eventID);
            }
        }
        
        List<int> stages = new List<int>(skillExcel.stages);
        stages.Remove(skillStageID);
        skillExcel.stages = stages.ToArray();
        excel_skill_stage.excelView.Remove(skillStageExcel);
    }

    void DeleteSkill(int skillID)
    {
        excel_skill_list skillExcel = excel_skill_list.Find(skillID);
        if (skillExcel == null)
            return;
        expandSkillIDs.Remove(skillID);
        expandSkillHitIDs.Remove(skillID);
        if (mCurSkillID == skillID)
            mCurSkillID = 0;
        if (skillExcel.hits != null)
        {
            int[] hits = skillExcel.hits.Clone() as int[];
            for (int i = 0; i < hits.Length; ++i)
            {
                int hitID = hits[i];
                DeleteSkillHit(skillID, hitID);
            }
        }
        if (skillExcel.stages != null)
        {
            int[] stages = skillExcel.stages.Clone() as int[];
            for (int i = 0; i < stages.Length; ++i)
            {
                int stageID = stages[i];
                DeleteSkillStage(skillID, stageID);
            }
        }
        
        excel_skill_list.excelView.Remove(skillExcel);
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
            excel_skill_list.Add(newSkill);
        }
        else if (data0 == "addChildObj")
        {
            excel_child_object newChildObj = new excel_child_object();
            newChildObj.id = GetEmptyChildObjectID();
            newChildObj.name = "newChildObj";
            excel_child_object.Add(newChildObj);
        }
        else if (data0 == "addStage")
        {
            int skillId = 0;
            if (!int.TryParse(sdatas[1], out skillId))
            {
                return;
            }
            if (!expandSkillIDs.Contains(skillId))
                expandSkillIDs.Add(skillId);
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
            if (skillExcel.stages.Length == 1)
            {
                newStage.trait |= (1 << (int)(SkillStageTrait.FirstStage));
            }
            skillExcel.stages[skillExcel.stages.Length - 1] = newStage.id;
            excel_skill_stage.Add(newStage);
        }
        else if (data0 == "addEvent")
        {
            int stageID = 0;
            if (!int.TryParse(sdatas[1], out stageID))
            {
                return;
            }

            SkillEditorMode editorMode = (SkillEditorMode)mCurMode;
            if (editorMode == SkillEditorMode.Skill)
            {
                if (!expandSkillStageIDs.Contains(stageID))
                    expandSkillStageIDs.Add(stageID);
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
                excel_skill_event.Add(eventExcel);
            }
            else if (editorMode == SkillEditorMode.ChildObject)
            {
                if (!expandChildObjectIDs.Contains(stageID))
                    expandChildObjectIDs.Add(stageID);
                excel_child_object childObjExcel = excel_child_object.Find(stageID);
                if (childObjExcel == null)
                    return;
                excel_skill_event eventExcel = new excel_skill_event();
                eventExcel.id = GetEmptySkillEventID();
                eventExcel.name = "newEvent";
                if (childObjExcel.events == null)
                {
                    childObjExcel.events = new int[1];
                }
                else
                {
                    int[] origList = childObjExcel.events.Clone() as int[];
                    childObjExcel.events = new int[childObjExcel.events.Length + 1];
                    origList.CopyTo(childObjExcel.events, 0);
                }
                childObjExcel.events[childObjExcel.events.Length - 1] = eventExcel.id;
                excel_skill_event.Add(eventExcel);
            }
        }
        else if (data0 == "addHit")
        {
            int skillId = 0;
            if (!int.TryParse(sdatas[1], out skillId))
            {
                return;
            }
            if (!expandSkillHitIDs.Contains(skillId))
                expandSkillHitIDs.Add(skillId);
            excel_skill_list skillExcel = excel_skill_list.Find(skillId);
            if (skillExcel == null)
                return;
            excel_skill_hit hitExcel = new excel_skill_hit();
            hitExcel.id = GetEmptySkillHitID();
            hitExcel.name = "newHit";
            if (skillExcel.hits == null)
            {
                skillExcel.hits = new int[1];
            }
            else
            {
                int[] origList = skillExcel.hits.Clone() as int[];
                skillExcel.hits = new int[skillExcel.hits.Length + 1];
                origList.CopyTo(skillExcel.hits, 0);
            }
            skillExcel.hits[skillExcel.hits.Length - 1] = hitExcel.id;
            excel_skill_hit.Add(hitExcel);
        }
        else if (data0 == "delHit")
        {
            int skillId = 0, skillHitId = 0;
            if (!int.TryParse(sdatas[1], out skillId))
            {
                return;
            }
            if (!int.TryParse(sdatas[2], out skillHitId))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("提醒", "是否删除判定表[" + skillHitId + "]", "是", "否"))
            {
                return;
            }
            DeleteSkillHit(skillId, skillHitId);
        }
        else if (data0 == "delEvent")
        {
            int stageId = 0, eventID = 0;
            if (!int.TryParse(sdatas[1], out stageId))
            {
                return;
            }
            if (!int.TryParse(sdatas[2], out eventID))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("提醒", "是否删除技能事件[" + eventID + "]", "是", "否"))
            {
                return;
            }
            DeleteSkillEvent(stageId, eventID);
        }
        else if (data0 == "delStage")
        {
            int skillId = 0, skillStageId = 0;
            if (!int.TryParse(sdatas[1], out skillId))
            {
                return;
            }
            if (!int.TryParse(sdatas[2], out skillStageId))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("提醒", "是否删除技能段[" + skillStageId + "]", "是", "否"))
            {
                return;
            }
            DeleteSkillStage(skillId, skillStageId);
        }
        else if (data0 == "delSkill")
        {
            int skillId = 0;
            if (!int.TryParse(sdatas[1], out skillId))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("提醒", "是否删除技能[" + skillId + "]", "是", "否"))
            {
                return;
            }
            DeleteSkill(skillId);
        }
        else if (data0 == "upEvent")
        {
            int skillStageId = 0, skillEventId = 0;
            if (!int.TryParse(sdatas[1], out skillStageId))
            {
                return;
            }
            if (!int.TryParse(sdatas[2], out skillEventId))
            {
                return;
            }
            SkillEditorMode mode = (SkillEditorMode)mCurMode;
            if (mode == SkillEditorMode.Skill)
            {
                excel_skill_stage stageExcel = excel_skill_stage.Find(skillStageId);
                if (stageExcel == null)
                    return;
                for (int i = 0; i < stageExcel.events.Length; ++i)
                {
                    int eventID = stageExcel.events[i];
                    if (skillEventId == eventID)
                    {
                        if (i == 0)
                            return;
                        int tmp = eventID;
                        stageExcel.events[i] = stageExcel.events[i - 1];
                        stageExcel.events[i - 1] = tmp;
                        return;
                    }
                }
            }
            else if (mode == SkillEditorMode.ChildObject)
            {
                excel_child_object childObjExcel = excel_child_object.Find(skillStageId);
                if (childObjExcel == null)
                    return;
                for (int i = 0; i < childObjExcel.events.Length; ++i)
                {
                    int eventID = childObjExcel.events[i];
                    if (skillEventId == eventID)
                    {
                        if (i == 0)
                            return;
                        int tmp = eventID;
                        childObjExcel.events[i] = childObjExcel.events[i - 1];
                        childObjExcel.events[i - 1] = tmp;
                        return;
                    }
                }
            }
        }
        else if (data0 == "downEvent")
        {
            int skillStageId = 0, skillEventId = 0;
            if (!int.TryParse(sdatas[1], out skillStageId))
            {
                return;
            }
            if (!int.TryParse(sdatas[2], out skillEventId))
            {
                return;
            }
            SkillEditorMode mode = (SkillEditorMode)mCurMode;
            if (mode == SkillEditorMode.Skill)
            {
                excel_skill_stage stageExcel = excel_skill_stage.Find(skillStageId);
                if (stageExcel == null)
                    return;
                for (int i = 0; i < stageExcel.events.Length - 1; ++i)
                {
                    int eventID = stageExcel.events[i];
                    if (skillEventId == eventID)
                    {
                        int tmp = eventID;
                        stageExcel.events[i] = stageExcel.events[i + 1];
                        stageExcel.events[i + 1] = tmp;
                        return;
                    }
                }
            }
            else if (mode == SkillEditorMode.ChildObject)
            {
                excel_child_object childObjExcel = excel_child_object.Find(skillStageId);
                if (childObjExcel == null)
                    return;
                for (int i = 0; i < childObjExcel.events.Length - 1; ++i)
                {
                    int eventID = childObjExcel.events[i];
                    if (skillEventId == eventID)
                    {
                        int tmp = eventID;
                        childObjExcel.events[i] = childObjExcel.events[i + 1];
                        childObjExcel.events[i + 1] = tmp;
                        return;
                    }
                }
            }
        }
        else if (data0 == "save")
        {
            Save();
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
        ShowChildObjectData();
        ShowSkillHitData();
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

        values = Enum.GetValues(typeof(SkillPreOpType)) as int[];
        texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            SkillPreOpType skillPreOpType = (SkillPreOpType)values[i];
            texts[i] = skillPreOpType.ToDescription();
        }
        skillExcel.skillPreOpType = EditorGUILayout.IntPopup("预操作类型", (int)skillExcel.skillPreOpType, texts, values);
        SkillPreOpType opType = (SkillPreOpType)skillExcel.skillPreOpType;
        if (opType == SkillPreOpType.TargetDirLine || opType == SkillPreOpType.TargetDirFan || opType == SkillPreOpType.TargetPos)
        {
            float opRadius = (float)skillExcel.skillPreOpData1 * 0.001f;
            opRadius = EditorGUILayout.FloatField("  最大半径", opRadius);
            skillExcel.skillPreOpData1 = opRadius < 0.0f ? 0 : (int)(opRadius * 1000.0f);
        }
        if (opType == SkillPreOpType.TargetDirLine)
        {
            float opWidth = (float)skillExcel.skillPreOpData2 * 0.001f;
            opWidth = EditorGUILayout.FloatField("  判定宽度", opWidth);
            skillExcel.skillPreOpData2 = opWidth < 0.0f ? 0 : (int)(opWidth * 1000.0f);
        }
        else if (opType == SkillPreOpType.TargetDirFan)
        {
            skillExcel.skillPreOpData2 = EditorGUILayout.IntField("  扇形半角", skillExcel.skillPreOpData2);
            skillExcel.skillPreOpData2 = skillExcel.skillPreOpData2 < 0 ? 0 : skillExcel.skillPreOpData2;
        }
        else if (opType == SkillPreOpType.TargetPos)
        {
            float tRadius = (float)skillExcel.skillPreOpData2 * 0.001f;
            tRadius = EditorGUILayout.FloatField("  目标半径", tRadius);
            skillExcel.skillPreOpData2 = tRadius < 0.0f ? 0 : (int)(tRadius * 1000.0f);
        }
    }

    void ShowChildObjectData()
    {
        if (mCurChildObjectID == 0)
            return;
        GUIContent c = null;

        excel_child_object childObjExcel = excel_child_object.Find(mCurChildObjectID);
        EditorGUILayout.LabelField("子物体ID", string.Format("{0}", childObjExcel.id));
        childObjExcel.name = EditorGUILayout.TextField("子物体名称", childObjExcel.name);

        c = new GUIContent("子物体路径", "Resources/Particles/Prefabs下的预制体路径");
        childObjExcel.path = EditorGUILayout.TextField(c, childObjExcel.path);
        c = new GUIContent("持续时间", "持续帧数，-1为永远存在");
        childObjExcel.duration = EditorGUILayout.IntField(c, childObjExcel.duration);
        c = new GUIContent("粒子大小", "粒子的半径，决定了粒子何时算达到目标");
        childObjExcel.size = EditorGUILayout.FloatField(c, childObjExcel.size);

        int[] values = Enum.GetValues(typeof(ChildObjectInitPosType)) as int[];
        string[] texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            ChildObjectInitPosType t = (ChildObjectInitPosType)values[i];
            texts[i] = t.ToDescription();
        }
        childObjExcel.initPos = EditorGUILayout.IntPopup("初始位置", childObjExcel.initPos, texts, values);

        values = Enum.GetValues(typeof(ChildObjectInitDirType)) as int[];
        texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            ChildObjectInitDirType t = (ChildObjectInitDirType)values[i];
            texts[i] = t.ToDescription();
        }
        childObjExcel.initDir = EditorGUILayout.IntPopup("初始朝向", childObjExcel.initDir, texts, values);

        ChildObjectInitPosType initPosType = (ChildObjectInitPosType)childObjExcel.initPos;
        ChildObjectInitDirType initDirType = (ChildObjectInitDirType)childObjExcel.initDir;
        if (initPosType == ChildObjectInitPosType.SrcHinge || initPosType == ChildObjectInitPosType.TargetHinge
            || initDirType == ChildObjectInitDirType.SrcHingeDir || initDirType == ChildObjectInitDirType.TargetHingeDir)
        {
            childObjExcel.initHinge = EditorGUILayout.TextField("  初始挂点", childObjExcel.initHinge);
        }

        values = Enum.GetValues(typeof(ChildObjectMoveType)) as int[];
        texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            ChildObjectMoveType t = (ChildObjectMoveType)values[i];
            texts[i] = t.ToDescription();
        }
        childObjExcel.moveType = EditorGUILayout.IntPopup("移动类型", childObjExcel.moveType, texts, values);

        childObjExcel.yOffset = EditorGUILayout.FloatField("高度偏移", childObjExcel.yOffset);
        childObjExcel.speed = EditorGUILayout.FloatField("移动速度", childObjExcel.speed);

        values = Enum.GetValues(typeof(ChildObjectTrait)) as int[];
        texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            ChildObjectTrait t = (ChildObjectTrait)values[i];
            texts[i] = t.ToDescription();
        }
        childObjExcel.trait = MaskField("子物体特性", childObjExcel.trait, texts);
    }

    void ShowSkillHitData()
    {
        if (mCurHitID == 0)
            return;
        excel_skill_hit hitExcel = excel_skill_hit.Find(mCurHitID);

        EditorGUILayout.LabelField("判定ID", string.Format("{0}", hitExcel.id));
        hitExcel.name = EditorGUILayout.TextField("判定名称", hitExcel.name);
        int[] values = Enum.GetValues(typeof(SkillHitShape)) as int[];
        string[] texts = new string[values.Length];
        for (int i = 0; i < values.Length; ++i)
        {
            texts[i] = ((SkillHitShape)values[i]).ToDescription();
        }
        hitExcel.hitType = EditorGUILayout.IntPopup("判定形状", hitExcel.hitType, texts, values);
        SkillHitShape shape = (SkillHitShape)hitExcel.hitType;
        if (shape == SkillHitShape.FanSingle || shape == SkillHitShape.FanMultiple)
        {
            float radius = (float)hitExcel.hitData1 * 0.001f;
            radius = EditorGUILayout.FloatField("半径", radius);
            radius = Mathf.Max(radius, 0.0f);
            hitExcel.hitData1 = Mathf.FloorToInt(radius * 1000.0f);
            hitExcel.hitData2 = EditorGUILayout.IntSlider("半角角度", hitExcel.hitData2, 0, 180);
        }
        else if (shape == SkillHitShape.CircleSingle || shape == SkillHitShape.CircleMultiple)
        {
            float radius = (float)hitExcel.hitData1 * 0.001f;
            radius = EditorGUILayout.FloatField("半径", radius);
            radius = Mathf.Max(radius, 0.0f);
            hitExcel.hitData1 = Mathf.FloorToInt(radius * 1000.0f);
        }
        else if (shape == SkillHitShape.RectSingle || shape == SkillHitShape.RectMultiple)
        {
            float width = (float)hitExcel.hitData1 * 0.001f;
            width = EditorGUILayout.FloatField("宽度", width);
            width = Mathf.Max(width, 0.0f);
            hitExcel.hitData1 = Mathf.FloorToInt(width * 1000.0f);

            float length = (float)hitExcel.hitData2 * 0.001f;
            length = EditorGUILayout.FloatField("长度", length);
            length = Mathf.Max(length, 0.0f);
            hitExcel.hitData2 = Mathf.FloorToInt(length * 1000.0f);
        }

        float height = (float)hitExcel.hitData3 * 0.001f;
        height = EditorGUILayout.FloatField("高度", height);
        height = Mathf.Max(height, 0.0f);
        hitExcel.hitData3 = Mathf.FloorToInt(height * 1000.0f);
    }

    void ShowSkillStageData()
    {
        if (mCurSkillStageID == 0)
            return;
        excel_skill_stage stageExcel = excel_skill_stage.Find(mCurSkillStageID);

        EditorGUILayout.LabelField("技能段ID", string.Format("{0}", stageExcel.id));
        stageExcel.name = EditorGUILayout.TextField("技能段名称", stageExcel.name);
        stageExcel.time = EditorGUILayout.IntField("技能段帧数", stageExcel.time);
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
    
    #region Save
    protected enum SkillEditorSaveType
    {
        Skill,
        SkillStage,
        SkillHit,
        SkillEvent,
        ChildObject
    }

    void Save()
    {
        try
        {
            for (int i = 0; i < mSkillDomain.Count; ++i)
            {
                SkillExcelDomainSet domainSet = mSkillDomain[i];

                if (!Save(domainSet.skillDomain, SkillEditorSaveType.Skill))
                {
                    Debug.LogError("技能表保存失败");
                }
                if (!Save(domainSet.skillStageDomain, SkillEditorSaveType.SkillStage))
                {
                    Debug.LogError("技能段保存失败");
                }
                if (!Save(domainSet.skillHitDomain, SkillEditorSaveType.SkillHit))
                {
                    Debug.LogError("技能判定表保存失败");
                }
                if (!Save(domainSet.skillEventDomain, SkillEditorSaveType.SkillEvent))
                {
                    Debug.LogError("技能事件表保存失败");
                }
                if (!Save(domainSet.childObjectDomain, SkillEditorSaveType.ChildObject))
                {
                    Debug.LogError("子物体表保存失败");
                }
            }
            EditorUtility.DisplayDialog("提示", "技能数据保存成功", "确定");
        }
        catch (Exception e)
        {
            Debug.LogError("文件保存失败：\n" + e.Message);
        }
    }

    bool Save(SkillExcelDomain domain, SkillEditorSaveType saveType)
    {
        string path = "Assets/Resources/Excel/skill/" + domain.fileName + ".txt";
        TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        if (asset == null)
            return false;
        string header = string.Empty;
        int enterPos = asset.text.IndexOf('\n');
        if (enterPos > 0)
        {
            header += asset.text.Substring(0, enterPos + 1);
        }
        else
        {
            header += asset.text;
        }

        StringBuilder skillExcelText = new StringBuilder(256);
        if (saveType == SkillEditorSaveType.Skill)
        {
            SaveSkill(domain, ref skillExcelText);
        }
        else if (saveType == SkillEditorSaveType.SkillStage)
        {
            SaveSkillStage(domain, ref skillExcelText);
        }
        else if (saveType == SkillEditorSaveType.SkillHit)
        {
            SaveSkillHit(domain, ref skillExcelText);
        }
        else if (saveType == SkillEditorSaveType.SkillEvent)
        {
            SaveSkillEvent(domain, ref skillExcelText);
        }
        else if (saveType == SkillEditorSaveType.ChildObject)
        {
            SaveChildObject(domain, ref skillExcelText);
        }
        string content = skillExcelText.ToString();

        string absPath = Application.dataPath + "/Resources/Excel/skill/" + domain.fileName + ".txt";
        if (File.Exists(absPath))
            File.Delete(absPath);
        using (StreamWriter sw = new StreamWriter(absPath, false, Encoding.Unicode))
        {
            string text = header + content;
            sw.Write(text);
            sw.Close();
        }
        return true;
    }

    void SaveSkill(SkillExcelDomain domain, ref StringBuilder skillExcelText)
    {
        for (int j = 0; j < excel_skill_list.Count; ++j)
        {
            excel_skill_list excel = excel_skill_list.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            skillExcelText.Append(excel.id).Append("\t");
            skillExcelText.Append(excel.name).Append("\t");
            skillExcelText.Append(IntArrayToString(excel.stages)).Append("\t");
            skillExcelText.Append(IntArrayToString(excel.hits)).Append("\t");
            skillExcelText.Append(excel.trait).Append("\t");
            skillExcelText.Append(excel.maxDistance).Append("\t");
            skillExcelText.Append(excel.targetType).Append("\t");
            skillExcelText.Append(excel.skillPreOpType).Append("\t");
            skillExcelText.Append(excel.skillPreOpData1).Append("\t");
            skillExcelText.Append(excel.skillPreOpData2).Append("\r\n");
        }
    }

    void SaveSkillStage(SkillExcelDomain domain, ref StringBuilder skillExcelText)
    {
        for (int j = 0; j < excel_skill_stage.Count; ++j)
        {
            excel_skill_stage excel = excel_skill_stage.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            skillExcelText.Append(excel.id).Append("\t");
            skillExcelText.Append(excel.name).Append("\t");
            skillExcelText.Append(IntArrayToString(excel.events)).Append("\t");
            skillExcelText.Append(excel.trait).Append("\t");
            skillExcelText.Append(excel.time).Append("\t");
            skillExcelText.Append(excel.nextStageID).Append("\r\n");
        }
    }

    void SaveSkillHit(SkillExcelDomain domain, ref StringBuilder skillExcelText)
    {
        for (int j = 0; j < excel_skill_hit.Count; ++j)
        {
            excel_skill_hit excel = excel_skill_hit.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            skillExcelText.Append(excel.id).Append("\t");
            skillExcelText.Append(excel.name).Append("\t");
            skillExcelText.Append(excel.hitType).Append("\t");
            skillExcelText.Append(excel.hitData1).Append("\t");
            skillExcelText.Append(excel.hitData2).Append("\t");
            skillExcelText.Append(excel.hitData3).Append("\r\n");
        }
    }

    void SaveSkillEvent(SkillExcelDomain domain, ref StringBuilder skillExcelText)
    {
        for (int j = 0; j < excel_skill_event.Count; ++j)
        {
            excel_skill_event excel = excel_skill_event.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            skillExcelText.Append(excel.id).Append("\t");
            skillExcelText.Append(excel.name).Append("\t");
            skillExcelText.Append(excel.triggerType).Append("\t");
            skillExcelText.Append(excel.triggerParam1).Append("\t");
            skillExcelText.Append(excel.triggerParam2).Append("\t");
            skillExcelText.Append(excel.eventType).Append("\t");
            skillExcelText.Append(excel.evnetParam1).Append("\t");
            skillExcelText.Append(excel.evnetParam2).Append("\t");
            skillExcelText.Append(excel.evnetParam3).Append("\t");
            skillExcelText.Append(excel.evnetParam4).Append("\t");
            skillExcelText.Append(excel.evnetParam5).Append("\t");
            skillExcelText.Append(excel.evnetParam6).Append("\t");
            skillExcelText.Append(excel.evnetParam7).Append("\t");
            skillExcelText.Append(excel.evnetParam8).Append("\t");
            skillExcelText.Append(excel.evnetParam9).Append("\t");
            skillExcelText.Append(excel.trait).Append("\r\n");
        }
    }

    void SaveChildObject(SkillExcelDomain domain, ref StringBuilder skillExcelText)
    {
        for (int j = 0; j < excel_child_object.Count; ++j)
        {
            excel_child_object excel = excel_child_object.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            skillExcelText.Append(excel.id).Append("\t");
            skillExcelText.Append(excel.name).Append("\t");
            skillExcelText.Append(excel.path).Append("\t");
            skillExcelText.Append(excel.duration).Append("\t");
            skillExcelText.Append(excel.size).Append("\t");
            skillExcelText.Append(excel.initPos).Append("\t");
            skillExcelText.Append(excel.initHinge).Append("\t");
            skillExcelText.Append(excel.yOffset).Append("\t");
            skillExcelText.Append(excel.initDir).Append("\t");
            skillExcelText.Append(excel.moveType).Append("\t");
            skillExcelText.Append(excel.speed).Append("\t");
            skillExcelText.Append(IntArrayToString(excel.events)).Append("\t");
            skillExcelText.Append(excel.trait).Append("\r\n");
        }
    }

    string IntArrayToString(int[] a)
    {
        if (a == null)
            return string.Empty;
        StringBuilder text = new StringBuilder();
        for (int i = 0; i < a.Length; ++i)
        {
            text.Append(a[i]);
            if (i != a.Length - 1)
                text.Append("*");
        }
        return text.ToString();
    }
    #endregion // Save
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

    public static string ToDescription(this SkillHitShape enumType)
    {
        Type type = typeof(SkillHitShape);
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

    public static string ToDescription(this ChildObjectInitPosType enumType)
    {
        Type type = typeof(ChildObjectInitPosType);
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

    public static string ToDescription(this ChildObjectInitDirType enumType)
    {
        Type type = typeof(ChildObjectInitDirType);
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

    public static string ToDescription(this ChildObjectMoveType enumType)
    {
        Type type = typeof(ChildObjectMoveType);
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

    public static string ToDescription(this ChildObjectTrait enumType)
    {
        Type type = typeof(ChildObjectTrait);
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

    public static string ToDescription(this SkillSelectCharactorType enumType)
    {
        Type type = typeof(SkillSelectCharactorType);
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

    public static string ToDescription(this SkillPreOpType enumType)
    {
        Type type = typeof(SkillPreOpType);
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

    public static string ToDescription(this SkillMoveDataType enumType)
    {
        Type type = typeof(SkillMoveDataType);
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