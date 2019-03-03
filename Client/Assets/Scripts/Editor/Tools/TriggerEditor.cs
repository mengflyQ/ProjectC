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
}

public class TriggerEditor : EditorWindow
{
    public static TriggerEditor mInstance = null;

    List<int> expandTriggerGroups = new List<int>();

    int mCurTriggerID = 0;

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

            mTriggerDomain.Add(domainSet);
        }
    }

    private void OnDestroy()
    {
        excel_trigger_list.excelView = null;
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
            
            GUIStyle style = isSel ? mTriggerStyleSelected : mTriggerStyleNormal;
            if (GUI.Button(rcbtn, "● 触发器{" + triggerExcel.id + "}::名称{" + triggerExcel.name + "}", style))
            {
                if (!isSel)
                {
                    mCurTriggerID = triggerExcel.id;
                }
                else
                {
                    mCurTriggerID = 0;
                }
            }
        }
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

    void DeleteTrigger(int triggerID)
    {
        excel_trigger_list triggerExcel = excel_trigger_list.Find(triggerID);
        if (triggerExcel == null)
            return;
        expandTriggerGroups.Remove(triggerID);
        if (mCurTriggerID == triggerID)
            mCurTriggerID = 0;

        excel_trigger_list.excelView.Remove(triggerExcel);
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
        else if (data0 == "save")
        {
            Save();
        }
    }
    #endregion

    #region DataView
    void OnDataView(int wndID)
    {
        if (mCurTriggerID == 0)
        {
            EditorGUILayout.LabelField("没有任何数据");
        }
        ShowTriggerData();
    }
    
    void ShowTriggerData()
    {
        if (mCurTriggerID == 0)
            return;
        excel_trigger_list triggerExcel = excel_trigger_list.Find(mCurTriggerID);
        EditorGUILayout.LabelField("触发器ID", string.Format("{0}", triggerExcel.id));
        triggerExcel.name = EditorGUILayout.TextField("触发器名称", triggerExcel.name);

        EditorGUILayout.Separator();


    }
    #endregion

    #region Save
    void Save()
    {
        try
        {
            for (int i = 0; i < mTriggerDomain.Count; ++i)
            {
                TriggerExcelDomainSet domainSet = mTriggerDomain[i];
                Save(domainSet.triggerDomain);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("文件保存失败：\n" + e.Message);
        }
    }

    bool Save(ExcelDomain domain)
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

        StringBuilder triggerExcelText = new StringBuilder(256);

        SaveTrigger(domain, ref triggerExcelText);

        string content = triggerExcelText.ToString();

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

    void SaveTrigger(ExcelDomain domain, ref StringBuilder triggerExcelText)
    {
        for (int j = 0; j < excel_trigger_list.Count; ++j)
        {
            excel_trigger_list excel = excel_trigger_list.GetByIndex(j);
            if (excel == null)
                continue;
            if (excel.id < domain.minID || excel.id > domain.maxID)
                continue;
            triggerExcelText.Append(excel.id).Append("\t");
            triggerExcelText.Append(excel.name).Append("\t");
            triggerExcelText.Append(excel.bindType).Append("\t");
            triggerExcelText.Append(IntArrayToString(excel.bindParams)).Append("\t");
            triggerExcelText.Append(excel.triggerType).Append("\t");
            triggerExcelText.Append(IntArrayToString(excel.triggerParams)).Append("\t");
            triggerExcelText.Append(excel.condition).Append("\t");
            triggerExcelText.Append(IntArrayToString(excel.condParams)).Append("\t");
            triggerExcelText.Append(excel.eventType).Append("\t");
            triggerExcelText.Append(IntArrayToString(excel.eventParams)).Append("\r\n");
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