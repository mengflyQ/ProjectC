using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;

[CustomEditor(typeof(ScnMarkManger))]
public class ScnMarkMangerEditor : Editor
{
    private void OnEnable()
    {

    }

    double FloatToDouble(float v)
    {
        double d = Math.Floor((double)v * 1000.0) / 1000.0;
        return d;
    }

    public override void OnInspectorGUI()
    {
        ScnMarkManger scnMarkMgr = target as ScnMarkManger;

        if (GUILayout.Button("生成JSon"))
        {
            JsonData root = new JsonData();

            JsonData markPoints = new JsonData();
            root["MarkPoints"] = markPoints;
            MarkPoint[] pts = scnMarkMgr.GetComponentsInChildren<MarkPoint>();
            for (int i = 0; i < pts.Length; ++i)
            {
                MarkPoint pt = pts[i];
                JsonData markPoint = new JsonData();

                markPoint["name"] = new JsonData(pt.name);

                JsonData position = new JsonData();
                position.Add(FloatToDouble(pt.transform.position.x));
                position.Add(FloatToDouble(pt.transform.position.y));
                position.Add(FloatToDouble(pt.transform.position.z));
                markPoint["Pos"] = position;

                JsonData direction = new JsonData();
                direction.Add(FloatToDouble(pt.transform.forward.x));
                direction.Add(FloatToDouble(pt.transform.forward.y));
                direction.Add(FloatToDouble(pt.transform.forward.z));
                markPoint["Dir"] = direction;

                markPoints.Add(markPoint);
            }

            string jsonText = root.ToJson();
            string path = Application.dataPath;

            int slash1 = path.LastIndexOf('/');
            int slash2 = path.LastIndexOf('\\');
            path = path.Substring(0, Mathf.Max(slash1, slash2));
            path += "/Data";

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "/";
            path += UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "/refreshInfo.json";

            using (FileStream file = new FileStream(path, FileMode.CreateNew))
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(jsonText);
                file.Write(data, 0, data.Length);
                file.Flush();
                file.Close();
            }
            Debug.LogError(jsonText);
        }
    }
}