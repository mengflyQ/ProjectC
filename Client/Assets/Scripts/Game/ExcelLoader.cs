using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using LitJson;
using System.Xml;

using System.Reflection;

public class ExcelLoader
{
    public static void LoadSingleExcel(string className)
    {
        TextAsset asset = Resources.Load<TextAsset>("Excel/config/" + className);
        JsonData data = JsonMapper.ToObject(asset.text);

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

            asset = Resources.Load<TextAsset>("Excel/" + filename);
            string[] excel_lines = GetLines(asset);
            for (int l = 1; l < excel_lines.Length; ++l)
            {
                string excel_line = excel_lines[l];
                string[] excel_line_data = excel_line.Split('\t');
                if (excel_line_data.Length != fieldData.Count)
                {
                    Debug.LogError("Excel Error: Excel Data Number Is Not Equal To Config Data Number!");
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
                    object value = GetFieldValueByType(fieldType, strValue);
                    if (value != null)
                    {
                        if (fieldName == "id")
                        {
                            id = (int)value;
                        }
                        if (excelField == null)
                        {
                            return;
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

	public static void Init()
	{
		TextAsset asset = Resources.Load<TextAsset>("Excel/excel_index");
		string[] lines = GetLines(asset);

		for (int i = 1; i < lines.Length; ++i)
		{
			string line = lines[i];
			string[] datas = line.Split('\t');
			string className = datas[1];
			asset = Resources.Load<TextAsset>("Excel/config/" + className);
			JsonData data = JsonMapper.ToObject(asset.text);

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

				asset = Resources.Load<TextAsset>("Excel/" + filename);
                if (asset == null)
                {
                    Debug.LogError("无法加载表格: " + "Excel/" + filename);
                    continue;
                }
				string[] excel_lines = GetLines(asset);
				for (int l = 1; l < excel_lines.Length; ++l)
				{
					string excel_line = excel_lines[l];
					string[] excel_line_data = excel_line.Split('\t');
					if (excel_line_data.Length != fieldData.Count)
					{
						Debug.LogError("Excel Error: Excel Data Number Is Not Equal To Config Data Number!");
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
						object value = GetFieldValueByType(fieldType, strValue);
						if (value != null)
						{
							if (fieldName == "id")
							{
                                id = (int)value;
							}
                            if (excelField == null)
                            {
                                return;
                            }
							excelField.SetValue(excel, value);
						}
					}
					if (id != 0)
					{
						addMethod.Invoke(vd, new object[] { excel} );
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
                Debug.LogError("Excel Error: Bad Data Type -- Int");
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
                Debug.LogError("Excel Error: Bad Data Type -- Float");
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
                        Debug.LogError("Excel Error: Bad Data Type -- Int[]");
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
                        Debug.LogError("Excel Error: Bad Data Type -- float[]");
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
		Debug.LogError("Excel Error: Bad Data Type -- Unknown -- " + fieldType);
		return null;
	}

	static string[] GetLines(TextAsset asset)
	{
		int index = 0;
		int count = 0;
		List<string> list = new List<string>();
		for (int i = 0; i < asset.bytes.Length; ++i)
		{
			byte b = asset.bytes[i];
			if (b == '\r' || b == '\n' && count > 0)
			{
				string s = Encoding.UTF8.GetString(asset.bytes, index, count);
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

    public static void Add(T excel)
    {
        excelView.Add(excel);
        excelView.Sort(CompareExcel);
    }
}