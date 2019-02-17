using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using LitJson;

using System.Reflection;

public class ExcelLoader
{
    public static void Init()
    {
        byte[] excelIndexDatas = null;
        using (FileStream fsRead = new FileStream(@"../Data/Excel/excel_index.txt", FileMode.Open))
        {
            excelIndexDatas = new byte[fsRead.Length];
            fsRead.Read(excelIndexDatas, 0, excelIndexDatas.Length);
            fsRead.Close();
        }
        string excelIndexStr = System.Text.Encoding.Unicode.GetString(excelIndexDatas);
        // string[] lines = GetLines(excelIndexDatas);
        string[] lines = excelIndexStr.Split(new string[]{"\r\n"}, StringSplitOptions.None);

        for (int i = 1; i < lines.Length; ++i)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;
            string[] datas = line.Split('\t');
            string className = datas[1];
            string loadPort = datas[2];
            if (!loadPort.Contains("s"))
                continue;
            string configText = string.Empty;
            using (FileStream fsRead = new FileStream(@"../Data/Excel/config/" + className + ".json", FileMode.Open))
            {
                byte[] configDatas = new byte[fsRead.Length];
                fsRead.Read(configDatas, 0, configDatas.Length);
                configText = System.Text.Encoding.UTF8.GetString(configDatas);
                fsRead.Close();
            }
            JsonData data = JsonMapper.ToObject(configText);

            Type excel_type = Type.GetType("excel_" + className);
            FieldInfo excelViewField = excel_type.BaseType.GetField("excelView");
            Type viewType = excelViewField.FieldType;
            object vd = System.Activator.CreateInstance(viewType);
            MethodInfo addMethod = viewType.GetMethod("Add");
            MethodInfo initMethod = excel_type.BaseType.GetMethod("Initialize");

            JsonData filesData = data["files"];
            JsonData fieldData = data["field"];
            for (int j = 0; j < filesData.Count; ++j)
            {
                JsonData fileData = filesData[j];
                string filename = fileData.ToString();

                string excelStr = null;
                using (FileStream fsRead = new FileStream(@"../Data/Excel/" + filename + ".txt", FileMode.Open))
                {
                    byte[] excelDatas = new byte[fsRead.Length];
                    fsRead.Read(excelDatas, 0, excelDatas.Length);
                    fsRead.Close();
                    excelStr = System.Text.Encoding.Unicode.GetString(excelDatas);
                }
                string[] excel_lines = excelStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                //GetLines(excelDatas);
                for (int l = 1; l < excel_lines.Length; ++l)
                {
                    string excel_line = excel_lines[l];
                    if (string.IsNullOrWhiteSpace(excel_line) || string.IsNullOrEmpty(excel_line))
                        continue;
                    string[] excel_line_data = excel_line.Split('\t');
                    if (excel_line_data.Length != fieldData.Count)
                    {
                        Debug.LogError("Excel Error: Excel Data Number Is Not Equal To Config Data Number! File: " + filename);
                        continue;
                    }

                    object excel = System.Activator.CreateInstance(excel_type);
                    int id = 0;

                    for (int m = 0; m < fieldData.Count; ++m)
                    {
                        JsonData fieldDef = fieldData[m];
                        string fieldName = fieldDef["name"].ToString();
                        string fieldType = fieldDef["type"].ToString();
                        FieldInfo excelField = excel_type.GetField(fieldName);
                        string strValue = excel_line_data[m];
                        if (string.IsNullOrWhiteSpace(strValue))
                            continue;
                        object value = GetFieldValueByType(fieldType, strValue);
                        if (value != null)
                        {
                            if (fieldName == "id")
                            {
                                id = (int)value;
                            }
                            excelField.SetValue(excel, value);
                        }
                    }
                    if (id != 0)
                    {
                        addMethod.Invoke(vd, new object[] { excel });
                    }
                }
                excelViewField.SetValue(null, vd);
                initMethod.Invoke(vd, new object[] { });
            }
        }
    }

    static object GetFieldValueByType(string fieldType, string value)
    {
        if (fieldType == "int")
        {
            int rst = 0;
            if (!string.IsNullOrEmpty(value))
            {
                if (int.TryParse(value, out rst))
                    return rst;
                Console.WriteLine("Excel Error: Bad Data Type -- Int");
            }
            return 0;
        }
        if (fieldType == "string")
        {
            return value;
        }
        if (fieldType == "string[]")
        {
            string[] rst = null;
            if (!string.IsNullOrEmpty(value))
            {
                rst = value.Split('*');
            }
            else
            {
                rst = new string[0];
            }
            return rst;
        }
        if (fieldType == "float")
        {
            float rst = 0.0f;
            if (!string.IsNullOrEmpty(value))
            {
                if (float.TryParse(value, out rst))
                    return rst;
                Console.WriteLine("Excel Error: Bad Data Type -- Float");
            }
            return 0.0f;
        }
        if (fieldType == "int[]")
        {
            int[] rst = null;
            if (!string.IsNullOrEmpty(value))
            {
                string[] vs = value.Split('*');
                rst = new int[vs.Length];
                for (int i = 0; i < vs.Length; ++i)
                {
                    string v = vs[i];
                    int r = 0;
                    if (!int.TryParse(v, out r))
                    {
                        Console.WriteLine("Excel Error: Bad Data Type -- Int[]");
                        r = 0;
                    }
                    rst[i] = r;
                }
            }
            else
            {
                rst = new int[0];
            }
            return rst;
        }
        if (fieldType == "float[]")
        {
            float[] rst = null;
            if (!string.IsNullOrEmpty(value))
            {
                string[] vs = value.Split('*');
                rst = new float[vs.Length];
                for (int i = 0; i < vs.Length; ++i)
                {
                    string v = vs[i];
                    float r = 0.0f;
                    if (!float.TryParse(v, out r))
                    {
                        Console.WriteLine("Excel Error: Bad Data Type -- float[]");
                        r = 0.0f;
                    }
                    rst[i] = r;
                }
            }
            else
            {
                rst = new float[0];
            }
            return rst;
        }
        Console.WriteLine("Excel Error: Bad Data Type -- Unknown -- " + fieldType);
        return null;
    }

    static string[] GetLines(byte[] bytes)
    {
        int index = 0;
        int count = 0;
        List<string> list = new List<string>();
        for (int i = 0; i < bytes.Length; ++i)
        {
            byte b = bytes[i];
            if (b == '\r' || b == '\n' && count > 0)
            {
                string s = Encoding.Unicode.GetString(bytes, index, count);
                list.Add(s);
                count = 0;
                index = i + 1;
                continue;
            }
            ++count;
        }
        return list.ToArray();
    }

    public static List<excel_refresh> LoadRefreshExcel(int scnID)
    {
        var scnRefreshMap = RefreshSystem.Instance.mScnRefreshDatas;
        List<excel_refresh> refreshDatas = null;
        if (scnRefreshMap.TryGetValue(scnID, out refreshDatas))
        {
            return refreshDatas;
        }
        refreshDatas = new List<excel_refresh>();
        scnRefreshMap.Add(scnID, refreshDatas);

        excel_scn_list scnList = excel_scn_list.Find(scnID);
        if (scnList == null || string.IsNullOrEmpty(scnList.refreshPath))
            return null;
        if (mRefrshFieldData == null)
        {
            string configText = string.Empty;
            using (FileStream fsRead = new FileStream(@"../Data/SvrExcel/config/refresh.json", FileMode.Open))
            {
                byte[] configDatas = new byte[fsRead.Length];
                fsRead.Read(configDatas, 0, configDatas.Length);
                configText = System.Text.Encoding.UTF8.GetString(configDatas);
                fsRead.Close();
            }
            JsonData data = JsonMapper.ToObject(configText);
            mRefrshFieldData = data["field"];
        }

        Type excel_type = typeof(excel_refresh);
        FieldInfo excelViewField = excel_type.BaseType.GetField("excelView");
        Type viewType = excelViewField.FieldType;
        object vd = System.Activator.CreateInstance(viewType);
        MethodInfo addMethod = viewType.GetMethod("Add");

        MethodInfo initMethod = excel_type.BaseType.GetMethod("Initialize");

        string excelStr = null;
        using (FileStream fsRead = new FileStream(@"../Data/SvrExcel/" + scnList.refreshPath + ".txt", FileMode.Open))
        {
            byte[] excelDatas = new byte[fsRead.Length];
            fsRead.Read(excelDatas, 0, excelDatas.Length);
            fsRead.Close();
            excelStr = System.Text.Encoding.Unicode.GetString(excelDatas);
        }
        string[] excel_lines = excelStr.Split(new string[] { "\r\n" }, StringSplitOptions.None);
        //GetLines(excelDatas);
        for (int l = 1; l < excel_lines.Length; ++l)
        {
            string excel_line = excel_lines[l];
            if (string.IsNullOrWhiteSpace(excel_line) || string.IsNullOrEmpty(excel_line))
                continue;
            string[] excel_line_data = excel_line.Split('\t');
            if (excel_line_data.Length != mRefrshFieldData.Count)
            {
                Console.WriteLine("Excel Error: Excel Data Number Is Not Equal To Config Data Number! File: " + scnList.refreshPath);
                continue;
            }

            object excel = System.Activator.CreateInstance(excel_type);
            int id = 0;

            for (int m = 0; m < mRefrshFieldData.Count; ++m)
            {
                JsonData fieldDef = mRefrshFieldData[m];
                string fieldName = fieldDef["name"].ToString();
                string fieldType = fieldDef["type"].ToString();
                FieldInfo excelField = excel_type.GetField(fieldName);
                string strValue = excel_line_data[m];
                if (string.IsNullOrWhiteSpace(strValue))
                    continue;
                object value = GetFieldValueByType(fieldType, strValue);
                if (value != null)
                {
                    if (fieldName == "id")
                    {
                        id = (int)value + scnID * 1000;
                    }
                    excelField.SetValue(excel, value);
                }
            }
            if (id != 0)
            {
                addMethod.Invoke(vd, new object[] { excel });
            }

            excel_refresh er = excel as excel_refresh;
            refreshDatas.Add(er);
        }
        excelViewField.SetValue(null, vd);
        initMethod.Invoke(vd, new object[] { });

        return refreshDatas;
    }

    private static JsonData mRefrshFieldData = null;
}

public class ExcelSimple
{
    public int id;
}

public class ExcelBase<T> : ExcelSimple where T : ExcelSimple
{
    public static List<T> excelView = null;

    public static int Count
    {
        get
        {
            return excelView.Count;
        }
    }

    public static void Initialize()
    {
        excelView.Sort(CompareExcel);
    }

    static int CompareExcel(T t1, T t2)
    {
        return t1.id.CompareTo(t2.id);
    }

    static T BinarySearchExcel(int low, int high, int id)
    {
        T highScene = excelView[high];
        T lowScene = excelView[low];

        int mid = (low + high) / 2;

        if (lowScene.id <= highScene.id)
        {
            T midScene = excelView[mid];
            if (midScene.id == id)
                return midScene;
            else if (midScene.id > id)
                return BinarySearchExcel(low, mid - 1, id);
            else
                return BinarySearchExcel(mid + 1, high, id);
        }
        return null;
    }

    public static T Find(int id)
    {
        if (excelView == null || excelView.Count <= 0)
            return null;

        return BinarySearchExcel(0, excelView.Count - 1, id);
    }

    public static T GetByIndex(int index)
    {
        if (index >= 0 && index < excelView.Count)
        {
            return excelView[index];
        }
        return null;
    }
}