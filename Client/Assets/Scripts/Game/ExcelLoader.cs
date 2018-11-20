using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using LitJson;

using System.Reflection;

public class ExcelLoader
{
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
							excelField.SetValue(excel, value);
						}
					}
					if (id != 0)
					{
						addMethod.Invoke(vd, new object[] {id, excel} );
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
			if (int.TryParse(value, out rst))
				return rst;
			Debug.LogError("Excel Error: Bad Data Type -- Int");
			return 0;
		}
		if (fieldType == "string")
		{
			return value;
		}
		if (fieldType == "float")
		{
			float rst = 0.0f;
			if (float.TryParse(value, out rst))
				return rst;
			Debug.LogError("Excel Error: Bad Data Type -- Float");
			return 0.0f;
		}
		if (fieldType == "int[]")
		{
			string[] vs = value.Split('*');
			int[] rst = new int[vs.Length];
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
			return rst;
		}
		if (fieldType == "float[]")
		{
			string[] vs = value.Split('*');
			float[] rst = new float[vs.Length];
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