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
                    if (string.IsNullOrWhiteSpace(excel_line))
                        continue;
                    string[] excel_line_data = excel_line.Split('\t');
                    if (excel_line_data.Length != fieldData.Count)
                    {
                        Console.WriteLine("Excel Error: Excel Data Number Is Not Equal To Config Data Number!");
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
                        addMethod.Invoke(vd, new object[] { id, excel });
                    }
                    excelViewField.SetValue(null, vd);
                }
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
}

public class ExcelBase<T>
{
    public static Dictionary<int, T> excelView = null;

    public static T Find(int id)
    {
        T excel = default(T);
        if (excelView.TryGetValue(id, out excel))
        {
            return excel;
        }
        return default(T);
    }
}