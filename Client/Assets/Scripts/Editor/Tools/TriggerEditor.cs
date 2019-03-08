using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TriggerExcelDomainSet
{
    public string domainName = string.Empty;
    public ExcelDomain triggerDomain = new ExcelDomain();
    public ExcelDomain conditionDomain = new ExcelDomain();
    public ExcelDomain eventDomain = new ExcelDomain();
}

public class TriggerEditor : EditorWindow
{
    public static TriggerEditor mInstance = null;

    List<int> expandTriggerGroups = new List<int>();
    List<int> expandConditionGroups = new List<int>();
    List<int> expandEventGroups = new List<int>();

    int mCurTriggerID = 0;
    int mCurTriggerCondID = 0;
    int mCurTriggerEventID = 0;

    Vector2 mListViewScrollView = Vector2.zero;

    public List<TriggerExcelDomainSet> mTriggerDomain = new List<TriggerExcelDomainSet>();
    int mCurDomainIndex = 0;

    GUIStyle mTriggerStyleNormal;
    GUIStyle mTriggerStyleSelected;

    [MenuItem("Tools/触发器编辑器")]
    static void CreateTriggerEditor()
    {
        if (mInstance != null)
        {
            mInstance.Close();
            mInstance = null;
            return;
        }
        mInstance = EditorWindow.GetWindow<TriggerEditor>(true, "触发器编辑器");
        mInstance.Show();

        if (excel_trigger_list.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("trigger_list");
        }
        if (excel_trigger_condition.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("trigger_condition");
        }
        if (excel_trigger_event.excelView == null)
        {
            ExcelLoader.LoadSingleExcel("trigger_event");
        }
    }

    private void OnEnable()
    {
        Texture2D selectedBackGround = new Texture2D(2, 2, TextureFormat.RGB24, false);
        selectedBackGround.SetPixels(new Color[] { Color.blue, Color.blue, Color.blue, Color.blue });
        selectedBackGround.Apply();

        mTriggerStyleNormal = new GUIStyle("Label");
        mTriggerStyleSelected = new GUIStyle("Label");
        mTriggerStyleSelected.normal.background = selectedBackGround;

        string xmlPath = Application.dataPath;
        xmlPath = xmlPath.Substring(0, xmlPath.LastIndexOf("Assets"));
        xmlPath += "Tools/ExcelDomains/trigger_excel_domain.xml";

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
            TriggerExcelDomainSet domainSet = new TriggerExcelDomainSet();
            domainSet.domainName = page.GetAttribute("name");

            domainList = page.GetElementsByTagName("trigger");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                ExcelDomain domain = new ExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.triggerDomain = domain;
            }
            domainList = page.GetElementsByTagName("condition");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                ExcelDomain domain = new ExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.conditionDomain = domain;
            }
            domainList = page.GetElementsByTagName("event");
            if (domainList.Count > 0)
            {
                XmlElement domainXml = domainList[0] as XmlElement;
                ExcelDomain domain = new ExcelDomain();
                domain.fileName = domainXml.GetAttribute("filename");
                int.TryParse(domainXml.GetAttribute("min"), out domain.minID);
                int.TryParse(domainXml.GetAttribute("max"), out domain.maxID);
                domainSet.eventDomain = domain;
            }

            mTriggerDomain.Add(domainSet);
        }
    }

    private void OnDestroy()
    {
        excel_trigger_list.excelView = null;
        excel_trigger_condition.excelView = null;
        excel_trigger_event.excelView = null;
    }

    private void OnGUI()
    {
        Rect wndRc = mInstance.position;

        BeginWindows();
        GUILayout.Window(1, new Rect(0.0f, 0.0f, wndRc.width / 4.0f, wndRc.height), OnListView, "列表");
        GUILayout.Window(2, new Rect(wndRc.width / 4.0f, 0.0f, wndRc.width * 3.0f / 4.0f, wndRc.height * 3.0f / 4.0f), OnDataView, "技能列表");
        EndWindows();
    }

    #region ListView
    void OnListView(int wndID)
    {
        List<string> domainName = new List<string>();
        for (int i = 0; i < mTriggerDomain.Count; ++i)
        {
            TriggerExcelDomainSet domainSet = mTriggerDomain[i];
            if (domainSet == null)
                continue;
            domainName.Add(domainSet.domainName);
        }
        mCurDomainIndex = GUILayout.Toolbar(mCurDomainIndex, domainName.ToArray());

        if (GUILayout.Button("...", EditorStyles.boldLabel, GUILayout.Width(32.0f)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("添加技能"), false, TriggerMenuEvent, "addTrigger");
            menu.AddItem(new GUIContent("保存表格"), false, TriggerMenuEvent, "save");
            menu.ShowAsContext();
        }

        mListViewScrollView = EditorGUILayout.BeginScrollView(mListViewScrollView);
        TriggerList();
        EditorGUILayout.EndScrollView();
    }

    void TriggerList()
    {
        TriggerExcelDomainSet domainSet = mTriggerDomain[mCurDomainIndex];
        if (domainSet == null)
            return;
        int minID = domainSet.triggerDomain.minID;
        int maxID = domainSet.triggerDomain.maxID;

        for (int i = 0; i < excel_trigger_list.Count; ++i)
        {
            excel_trigger_list triggerExcel = excel_trigger_list.GetByIndex(i);
            if (triggerExcel.id < minID || triggerExcel.id > maxID)
                continue;
            bool isExpand = expandTriggerGroups.Contains(triggerExcel.id);
            bool isSel = mCurTriggerID == triggerExcel.id;

            EditorGUILayout.BeginHorizontal();
            Rect rcfold = EditorGUILayout.GetControlRect(GUILayout.Width(12.0f));
            Rect rcbtn = EditorGUILayout.GetControlRect();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除技能"), false, TriggerMenuEvent, "delTrigger*" + triggerExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            GUIStyle skillStyle = EditorStyles.foldout;
            bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", skillStyle);
            GUIStyle style = isSel ? mTriggerStyleSelected : mTriggerStyleNormal;
            if (GUI.Button(rcbtn, "触发器{" + triggerExcel.id + "}::名称{" + triggerExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurTriggerID = triggerExcel.id;
                    mCurTriggerCondID = 0;
                    mCurTriggerEventID = 0;
                }
                else
                {
                    mCurTriggerID = 0;
                    mCurTriggerCondID = 0;
                    mCurTriggerEventID = 0;
                }
            }
            if (bCurExpand && !isExpand)
            {
                expandTriggerGroups.Add(triggerExcel.id);
                GUI.FocusControl("");
            }
            else if (!bCurExpand && isExpand)
            {
                expandTriggerGroups.Remove(triggerExcel.id);
                GUI.FocusControl("");
            }

            if (bCurExpand)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.BeginVertical();
                TriggerConditionList(triggerExcel);
                TriggerEventList(triggerExcel);
                EditorGUILayout.Space();

                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    public void TriggerConditionList(excel_trigger_list triggerExcel)
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
                menu.AddItem(new GUIContent("添加条件"), false, TriggerMenuEvent, "addCond*" + triggerExcel.id);
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        bool isExpand = expandConditionGroups.Contains(triggerExcel.id);
        GUIStyle skillStyle = EditorStyles.foldout;
        bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", skillStyle);
        GUI.Button(rcbtn, "条件", mTriggerStyleNormal);

        if (bCurExpand && !isExpand)
        {
            expandConditionGroups.Add(triggerExcel.id);
            GUI.FocusControl("");
        }
        else if (!bCurExpand && isExpand)
        {
            expandConditionGroups.Remove(triggerExcel.id);
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
        if (triggerExcel == null || triggerExcel.conditions == null)
        {
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return;
        }

        for (int i = 0; i < triggerExcel.conditions.Length; ++i)
        {
            int condID = triggerExcel.conditions[i];
            excel_trigger_condition condExcel = excel_trigger_condition.Find(condID);
            if (condExcel == null)
                continue;

            rcbtn = EditorGUILayout.GetControlRect();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除条件"), false, TriggerMenuEvent, "delCond*" + triggerExcel.id + "*" + condExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            bool isSel = mCurTriggerCondID == condExcel.id;

            GUIStyle style = isSel ? mTriggerStyleSelected : mTriggerStyleNormal;
            if (GUI.Button(rcbtn, "◆ 条件表{" + condExcel.id + "}::类型{" + ((TriggerConditionType)condExcel.condition).ToString() + "}", style))
            {
                if (!isSel)
                {
                    mCurTriggerID = 0;
                    mCurTriggerCondID = condExcel.id;
                    mCurTriggerEventID = 0;
                }
                else
                {
                    mCurTriggerID = 0;
                    mCurTriggerCondID = 0;
                    mCurTriggerEventID = 0;
                }
            }
        }

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    public void TriggerEventList(excel_trigger_list triggerExcel)
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
                menu.AddItem(new GUIContent("添加事件"), false, TriggerMenuEvent, "addEvent*" + triggerExcel.id);
                menu.ShowAsContext();
                Event.current.Use();
            }
        }

        bool isExpand = expandEventGroups.Contains(triggerExcel.id);
        GUIStyle triggerStyle = EditorStyles.foldout;
        bool bCurExpand = GUI.Toggle(rcfold, isExpand, "", triggerStyle);
        GUI.Button(rcbtn, "事件", mTriggerStyleNormal);

        if (bCurExpand && !isExpand)
        {
            expandEventGroups.Add(triggerExcel.id);
            GUI.FocusControl("");
        }
        else if (!bCurExpand && isExpand)
        {
            expandEventGroups.Remove(triggerExcel.id);
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
        if (triggerExcel == null || triggerExcel.events == null)
        {
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            return;
        }

        for (int i = 0; i < triggerExcel.events.Length; ++i)
        {
            int eventID = triggerExcel.events[i];
            excel_trigger_event eventExcel = excel_trigger_event.Find(eventID);
            if (eventExcel == null)
                continue;

            rcbtn = EditorGUILayout.GetControlRect();
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                if (rcbtn.Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("删除事件"), false, TriggerMenuEvent, "delEvent*" + triggerExcel.id + "*" + eventExcel.id);
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            bool isSel = mCurTriggerEventID == eventExcel.id;

            GUIStyle style = isSel ? mTriggerStyleSelected : mTriggerStyleNormal;
            if (GUI.Button(rcbtn, "◆ 事件表{" + eventExcel.id + "}::类型{" + ((TriggerEventType)eventExcel.eventType).ToString() + "}", style))
            {
                if (!isSel)
                {
                    mCurTriggerID = 0;
                    mCurTriggerCondID = 0;
                    mCurTriggerEventID = eventExcel.id;
                }
                else
                {
                    mCurTriggerID = 0;
                    mCurTriggerCondID = 0;
                    mCurTriggerEventID = 0;
                }
            }
        }

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
    }

    int GetEmptyTriggerID()
    {
        TriggerExcelDomainSet domainSet = mTriggerDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        ExcelDomain domain = domainSet.triggerDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_trigger_list.Count; ++i)
        {
            excel_trigger_list excel = excel_trigger_list.GetByIndex(i);
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

    int GetEmptyConditionID()
    {
        TriggerExcelDomainSet domainSet = mTriggerDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        ExcelDomain domain = domainSet.conditionDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_trigger_condition.Count; ++i)
        {
            excel_trigger_condition excel = excel_trigger_condition.GetByIndex(i);
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

    int GetEmptyEventID()
    {
        TriggerExcelDomainSet domainSet = mTriggerDomain[mCurDomainIndex];
        if (domainSet == null)
            return -1;
        ExcelDomain domain = domainSet.eventDomain;
        if (domain == null)
            return -1;
        int id = domain.minID;
        for (int i = 0; i < excel_trigger_event.Count; ++i)
        {
            excel_trigger_event excel = excel_trigger_event.GetByIndex(i);
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

    void DeleteTrigger(int triggerID)
    {
        excel_trigger_list triggerExcel = excel_trigger_list.Find(triggerID);
        if (triggerExcel == null)
            return;
        expandTriggerGroups.Remove(triggerID);
        if (mCurTriggerID == triggerID)
            mCurTriggerID = 0;

        if (triggerExcel.conditions != null)
        {
            for (int i = 0; i < triggerExcel.conditions.Length; ++i)
            {
                int condID = triggerExcel.conditions[i];
                excel_trigger_condition condExcel = excel_trigger_condition.Find(condID);
                if (condExcel == null)
                    continue;
                DeleteTriggerCondition(triggerExcel.id, condExcel.id);
            }
        }
        if (triggerExcel.events != null)
        {
            for (int i = 0; i < triggerExcel.events.Length; ++i)
            {
                int eventID = triggerExcel.events[i];
                excel_trigger_event eventExcel = excel_trigger_event.Find(eventID);
                if (eventExcel == null)
                    continue;
                DeleteTriggerEvent(triggerExcel.id, eventExcel.id);
            }
        }

        excel_trigger_list.excelView.Remove(triggerExcel);
    }

    void DeleteTriggerCondition(int triggerID, int condID)
    {
        excel_trigger_list triggerExcel = excel_trigger_list.Find(triggerID);
        if (triggerExcel == null)
            return;
        excel_trigger_condition condExcel = excel_trigger_condition.Find(condID);
        if (condExcel == null)
            return;
        expandConditionGroups.Remove(triggerID);
        if (mCurTriggerCondID == condID)
            mCurTriggerCondID = 0;

        List<int> conditions = new List<int>(triggerExcel.conditions);
        conditions.Remove(condID);
        triggerExcel.conditions = conditions.ToArray();

        excel_trigger_condition.excelView.Remove(condExcel);
    }

    void DeleteTriggerEvent(int triggerID, int eventID)
    {
        excel_trigger_list triggerExcel = excel_trigger_list.Find(triggerID);
        if (triggerExcel == null)
            return;
        excel_trigger_event eventExcel = excel_trigger_event.Find(eventID);
        if (eventExcel == null)
            return;
        expandEventGroups.Remove(triggerID);
        if (mCurTriggerEventID == eventID)
            mCurTriggerEventID = 0;

        List<int> events = new List<int>(triggerExcel.events);
        events.Remove(eventID);
        triggerExcel.conditions = events.ToArray();

        excel_trigger_event.excelView.Remove(eventExcel);
    }

    void TriggerMenuEvent(object obj)
    {
        string strData = obj as string;
        if (string.IsNullOrEmpty(strData))
            return;
        string[] sdatas = strData.Split('*');
        if (sdatas == null || sdatas.Length < 1)
            return;
        string data0 = sdatas[0];
        if (data0 == "addTrigger")
        {
            excel_trigger_list newTrigger = new excel_trigger_list();
            newTrigger.id = GetEmptyTriggerID();
            newTrigger.name = "newTrigger";
            excel_trigger_list.Add(newTrigger);
        }
        else if (data0 == "delTrigger")
        {
            int triggerId = 0;
            if (!int.TryParse(sdatas[1], out triggerId))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("提醒", "是否删除触发器[" + triggerId + "]", "是", "否"))
            {
                return;
            }
            DeleteTrigger(triggerId);
        }
        else if (data0 == "addCond")
        {
            int triggerId = 0;
            if (!int.TryParse(sdatas[1], out triggerId))
            {
                return;
            }
            if (!expandConditionGroups.Contains(triggerId))
                expandConditionGroups.Add(triggerId);
            excel_trigger_list triggerExcel = excel_trigger_list.Find(triggerId);
            if (triggerExcel == null)
                return;
            excel_trigger_condition condExcel = new excel_trigger_condition();
            condExcel.id = GetEmptyConditionID();
            if (triggerExcel.conditions == null)
            {
                triggerExcel.conditions = new int[1];
            }
            else
            {
                int[] origList = triggerExcel.conditions.Clone() as int[];
                triggerExcel.conditions = new int[triggerExcel.conditions.Length + 1];
                origList.CopyTo(triggerExcel.conditions, 0);
            }
            triggerExcel.conditions[triggerExcel.conditions.Length - 1] = condExcel.id;
            if (triggerExcel.conditions.Length == 1)
            {
                triggerExcel.firstCondition = condExcel.id;
            }
            excel_trigger_condition.Add(condExcel);
        }
        else if (data0 == "delCond")
        {
            int triggerId = 0;
            if (!int.TryParse(sdatas[1], out triggerId))
            {
                return;
            }
            int conditionId = 0;
            if (!int.TryParse(sdatas[2], out conditionId))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("提醒", "是否删除触发条件[" + conditionId + "]", "是", "否"))
            {
                return;
            }
            DeleteTriggerCondition(triggerId, conditionId);
        }
        else if (data0 == "addEvent")
        {
            int triggerId = 0;
            if (!int.TryParse(sdatas[1], out triggerId))
            {
                return;
            }
            if (!expandEventGroups.Contains(triggerId))
                expandEventGroups.Add(triggerId);
            excel_trigger_list triggerExcel = excel_trigger_list.Find(triggerId);
            if (triggerExcel == null)
                return;
            excel_trigger_event eventExcel = new excel_trigger_event();
            eventExcel.id = GetEmptyEventID();
            if (triggerExcel.events == null)
            {
                triggerExcel.events = new int[1];
            }
            else
            {
                int[] origList = triggerExcel.events.Clone() as int[];
                triggerExcel.events = new int[triggerExcel.events.Length + 1];
                origList.CopyTo(triggerExcel.events, 0);
            }
            triggerExcel.events[triggerExcel.events.Length - 1] = eventExcel.id;
            excel_trigger_event.Add(eventExcel);
        }
        else if (data0 == "delEvent")
        {
            int triggerId = 0;
            if (!int.TryParse(sdatas[1], out triggerId))
            {
                return;
            }
            int eventId = 0;
            if (!int.TryParse(sdatas[2], out eventId))
            {
                return;
            }
            if (!EditorUtility.DisplayDialog("提醒", "是否删除触发事件[" + eventId + "]", "是", "否"))
            {
                return;
            }
            DeleteTriggerEvent(triggerId, eventId);
        }
        else if (data0 == "save")
        {
            Save();
        }
    }
    #endregion

    #region DataView
    void OnDataView(int wndID)
    {
        if (mCurTriggerID == 0 && mCurTriggerCondID == 0 && mCurTriggerEventID == 0)
        {
            EditorGUILayout.LabelField("没有任何数据");
        }
        ShowTriggerData();
        ShowConditionData();
        ShowEventData();
    }
    
    void ShowTriggerData()
    {
        if (mCurTriggerID == 0)
            return;
        excel_trigger_list triggerExcel = excel_trigger_list.Find(mCurTriggerID);
        EditorGUILayout.LabelField("触发器ID", string.Format("{0}", triggerExcel.id));
        triggerExcel.name = EditorGUILayout.TextField("触发器名称", triggerExcel.name);

        EditorGUILayout.Separator();

        int[] valuse = Enum.GetValues(typeof(TriggerBindType)) as int[];
        string[] texts = new string[valuse.Length];
        for (int i = 0; i < valuse.Length; ++i)
        {
            texts[i] = ((TriggerBindType)valuse[i]).ToDescription();
        }
        triggerExcel.bindType = EditorGUILayout.IntPopup("触发对象类型", triggerExcel.bindType, texts, valuse);

        EditorGUILayout.Separator();

        valuse = Enum.GetValues(typeof(TriggerType)) as int[];
        texts = new string[valuse.Length];
        for (int i = 0; i < valuse.Length; ++i)
        {
            texts[i] = ((TriggerType)valuse[i]).ToDescription();
        }
        triggerExcel.triggerType = EditorGUILayout.IntPopup("触发时机", triggerExcel.triggerType, texts, valuse);

        EditorGUILayout.Separator();

        TriggerTypeEditorRegister.TriggerTypeEditorMethod method = null;
        if (TriggerTypeEditorRegister.mTriggerTypeMethods.TryGetValue((TriggerType)triggerExcel.triggerType, out method))
        {
            method(triggerExcel);
        }

        triggerExcel.firstCondition = EditorGUILayout.IntField("首条件", triggerExcel.firstCondition);

        excel_trigger_condition firstCondition = excel_trigger_condition.Find(triggerExcel.firstCondition);

        if (firstCondition != null)
        {
            TriggerConditionType condType = (TriggerConditionType)firstCondition.condition;
            TriggerConditionEditorRegister.TriggerConditionEditorMethod condMethod = null;
            if (TriggerConditionEditorRegister.mTriggerConditionMethods.TryGetValue(condType, out condMethod))
            {
                condMethod(firstCondition, this);
            }
        }

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("触发事件");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(16.0f);
        EditorGUILayout.BeginVertical();
        if (triggerExcel.events == null)
        {
            triggerExcel.events = new int[0];
        }

        for (int i = 0; i < triggerExcel.events.Length; ++i)
        {
            int eventID = triggerExcel.events[i];

            EditorGUILayout.LabelField("事件 " + i, string.Format("事件{{0}}", eventID));
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    void CheckBindParamCount(excel_trigger_list triggerExcel, int count)
    {
        if (triggerExcel.bindParams == null || triggerExcel.bindParams.Length != count)
        {
            triggerExcel.bindParams = new int[count];
        }
    }

    void TriggerBindParams(excel_trigger_list triggerExcel, TriggerBindType bindType)
    {
        switch (bindType)
        {
            case TriggerBindType.Scene:
                {

                }
                break;
            case TriggerBindType.Player:
                {

                }
                break;
            case TriggerBindType.NPC:
                {

                }
                break;
        }
    }

    void ShowConditionData()
    {
        if (mCurTriggerCondID == 0)
            return;
        excel_trigger_condition condExcel = excel_trigger_condition.Find(mCurTriggerCondID);
        EditorGUILayout.LabelField("条件ID", string.Format("{0}", condExcel.id));

        int[] valuse = Enum.GetValues(typeof(TriggerConditionType)) as int[];
        string[] texts = new string[valuse.Length];
        for (int i = 0; i < valuse.Length; ++i)
        {
            texts[i] = ((TriggerConditionType)valuse[i]).ToDescription();
        }
        condExcel.condition = EditorGUILayout.IntPopup("触发条件", condExcel.condition, texts, valuse);

        TriggerConditionType condType = (TriggerConditionType)condExcel.condition;
        TriggerConditionEditorRegister.TriggerConditionEditorMethod condMethod = null;
        if (TriggerConditionEditorRegister.mTriggerConditionMethods.TryGetValue(condType, out condMethod))
        {
            condMethod(condExcel, this);
        }
    }

    void ShowEventData()
    {
        if (mCurTriggerEventID == 0)
            return;
        excel_trigger_event eventExcel = excel_trigger_event.Find(mCurTriggerEventID);
        EditorGUILayout.LabelField("事件ID", string.Format("{0}", eventExcel.id));

        int[] valuse = Enum.GetValues(typeof(TriggerEventType)) as int[];
        string[] texts = new string[valuse.Length];
        for (int i = 0; i < valuse.Length; ++i)
        {
            texts[i] = ((TriggerEventType)valuse[i]).ToDescription();
        }
        eventExcel.eventType = EditorGUILayout.IntPopup("事件类型", eventExcel.eventType, texts, valuse);

        TriggerEventType eventType = (TriggerEventType)eventExcel.eventType;
        TriggerEventEditorRegister.TriggerEventEditorMethod method = null;
        if (TriggerEventEditorRegister.mTriggerEventMethods.TryGetValue(eventType, out method))
        {
            method(eventExcel);
        }
    }

    #endregion

    #region Save

    protected enum TriggerEditorSaveType
    {
        Trigger,
        Condition,
        Event,
    }

    void Save()
    {
        try
        {
            for (int i = 0; i < mTriggerDomain.Count; ++i)
            {
                TriggerExcelDomainSet domainSet = mTriggerDomain[i];


                if (!Save(domainSet.triggerDomain, TriggerEditorSaveType.Trigger))
                {
                    Debug.LogError("触发器表保存失败");
                }
                if (!Save(domainSet.conditionDomain, TriggerEditorSaveType.Condition))
                {
                    Debug.LogError("触发器条件表保存失败");
                }
                if (!Save(domainSet.eventDomain, TriggerEditorSaveType.Event))
                {
                    Debug.LogError("触发器事件表保存失败");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("文件保存失败：\n" + e.Message);
        }
    }

    bool Save(ExcelDomain domain, TriggerEditorSaveType saveType)
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

        StringBuilder excelText = new StringBuilder(256);

        if (saveType == TriggerEditorSaveType.Trigger)
        {
            SaveTrigger(domain, ref excelText);
        }
        else if (saveType == TriggerEditorSaveType.Condition)
        {
            SaveTriggerCondition(domain, ref excelText);
        }
        else if (saveType == TriggerEditorSaveType.Event)
        {
            SaveTriggerEvent(domain, ref excelText);
        }

        string content = excelText.ToString();

        string absPath = Application.dataPath + "/Resources/Excel/trigger/" + domain.fileName + ".txt";
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

    void SaveTrigger(ExcelDomain domain, ref StringBuilder excelText)
    {
        for (int j = 0; j < excel_trigger_list.Count; ++j)
        {
            excel_trigger_list excel = excel_trigger_list.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            excelText.Append(excel.id).Append("\t");
            excelText.Append(excel.name).Append("\t");
            excelText.Append(excel.bindType).Append("\t");
            excelText.Append(IntArrayToString(excel.bindParams)).Append("\t");
            excelText.Append(excel.triggerType).Append("\t");
            excelText.Append(IntArrayToString(excel.triggerParams)).Append("\t");
            excelText.Append(excel.firstCondition).Append("\t");
            excelText.Append(IntArrayToString(excel.conditions)).Append("\t");
            excelText.Append(IntArrayToString(excel.events)).Append("\t");
            excelText.Append(excel.trait).Append("\r\n");
        }
    }

    void SaveTriggerCondition(ExcelDomain domain, ref StringBuilder excelText)
    {
        for (int j = 0; j < excel_trigger_condition.Count; ++j)
        {
            excel_trigger_condition excel = excel_trigger_condition.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            excelText.Append(excel.id).Append("\t");
            excelText.Append(excel.condition).Append("\t");
            excelText.Append(IntArrayToString(excel.condParams)).Append("\r\n");
        }
    }

    void SaveTriggerEvent(ExcelDomain domain, ref StringBuilder excelText)
    {
        for (int j = 0; j < excel_trigger_event.Count; ++j)
        {
            excel_trigger_event excel = excel_trigger_event.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            excelText.Append(excel.id).Append("\t");
            excelText.Append(excel.eventType).Append("\t");
            excelText.Append(IntArrayToString(excel.eventParams)).Append("\r\n");
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
    #endregion
}

public static partial class EnumExtension
{
    public static string ToDescription(this TriggerBindType enumType)
    {
        Type type = typeof(TriggerBindType);
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

    public static string ToDescription(this TriggerType enumType)
    {
        Type type = typeof(TriggerType);
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

    public static string ToDescription(this TriggerConditionType enumType)
    {
        Type type = typeof(TriggerConditionType);
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

    public static string ToDescription(this TriggerEventType enumType)
    {
        Type type = typeof(TriggerEventType);
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