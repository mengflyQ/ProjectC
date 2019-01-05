using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HingePoints))]
public class HingePointsEditor : Editor
{
    string hingeName = string.Empty;
    Transform hingeTransform = null;

    private void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        HingePoints hp = target as HingePoints;

        EditorGUILayout.BeginVertical("Box");
        hingeName = EditorGUILayout.TextField("挂点名", hingeName);
        hingeTransform = (Transform)EditorGUILayout.ObjectField("挂点", hingeTransform, typeof(Transform), true);
        if (GUILayout.Button("添加挂点"))
        {
            if (hp.HaveHingePoint(hingeName))
            {
                EditorUtility.DisplayDialog("Error", "名为" + hingeName + "的挂点已经存在！", "确定");
                return;
            }
            if (string.IsNullOrEmpty(hingeName) || hingeTransform == null)
                return;
            if (hp.hingeItems == null)
                hp.hingeItems = new List<HingeItem>();
            Transform t = hingeTransform;
            string path = t.name;
            bool isChild = false;
            while (t != null)
            {
                if (t.parent == hp.transform)
                {
                    isChild = true;
                    break;
                }
                path = t.parent.name + "/" + path;
                t = t.parent;
            }
            if (!isChild)
                return;

            HingeItem item = new HingeItem();
            item.name = hingeName;
            item.path = path;
            item.localPosition = hingeTransform.localPosition;
            item.localRotation = hingeTransform.localEulerAngles;
            item.localScale = hingeTransform.localScale;
            hp.hingeItems.Add(item);

            if (Application.isPlaying)
            {
                GameObject.Destroy(hingeTransform.gameObject);
            }
            else
            {
                GameObject.DestroyImmediate(hingeTransform.gameObject);
            }
            hingeTransform = null;
            hingeName = string.Empty;
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        if (hp.hingeItems == null)
            return;
        for (int i = 0; i < hp.hingeItems.Count; ++i)
        {
            HingeItem item = hp.hingeItems[i];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(item.name, item.path);

            bool created = (item.hinge != null);

            if (created)
            {
                if (GUILayout.Button("保存"))
                {
                    Transform t = item.hinge;
                    string path = t.name;
                    bool isChild = false;
                    while (t != null)
                    {
                        if (t.parent == hp.transform)
                        {
                            isChild = true;
                            break;
                        }
                        path = t.parent.name + "/" + path;
                        t = t.parent;
                    }
                    if (!isChild)
                        return;
                    item.path = path;
                    item.localPosition = item.hinge.localPosition;
                    item.localRotation = item.hinge.localEulerAngles;
                    item.localScale = item.hinge.localScale;

                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(item.hinge.gameObject);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(item.hinge.gameObject);
                    }
                    item.hinge = null;
                }
            }
            else
            {
                if (GUILayout.Button("编辑"))
                {
                    Transform parent = hp.transform;
                    string path = item.path;
                    int endIndex = path.LastIndexOf('/');
                    string goName = item.name;
                    if (endIndex > 0)
                    {
                        goName = path.Substring(endIndex + 1);
                        path = path.Substring(0, endIndex);
                        parent = hp.transform.Find(path);
                    }

                    GameObject go = new GameObject(goName);
                    Transform t = go.transform;
                    t.parent = parent;
                    t.localPosition = item.localPosition;
                    t.localEulerAngles = item.localRotation;
                    t.localScale = item.localScale;

                    item.hinge = t;

                    Selection.activeGameObject = go;
                }
            }
            if (GUILayout.Button("删除"))
            {
                hp.hingeItems.Remove(item);
                if (Application.isPlaying)
                {
                    GameObject.Destroy(item.hinge.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(item.hinge.gameObject);
                }
                item.hinge = null;
                return;
            }

            EditorGUILayout.EndHorizontal();
        }
    }
}