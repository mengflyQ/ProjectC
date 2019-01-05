using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HingeItem
{
    public string name;
    public string path;
    public Vector3 localPosition;
    public Vector3 localRotation;
    public Vector3 localScale;

    [System.NonSerialized]
    public Transform hinge;
}

public class HingePoints : MonoBehaviour
{
    public bool HaveHingePoint(string name)
    {
        if (hingeItems != null)
        {
            for (int i = 0; i < hingeItems.Count; ++i)
            {
                HingeItem hinge = hingeItems[i];
                if (hinge.name == name)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void Initialize()
    {
        for (int i = 0; i < hingeItems.Count; ++i)
        {
            HingeItem item = hingeItems[i];
            if (item.hinge == null)
            {
                Transform parent = transform;
                string path = item.path;
                int endIndex = path.LastIndexOf('/');
                if (endIndex > 0)
                {
                    path = path.Substring(0, endIndex);
                    parent = transform.Find(path);
                }

                GameObject go = GameObjectPool.Spawn(item.name);
                Transform t = go.transform;
                t.parent = parent;
                t.localPosition = item.localPosition;
                t.localEulerAngles = item.localRotation;
                t.localScale = item.localScale;

                item.hinge = t;
            }
        }
    }

    public void Uninitialize()
    {
        for (int i = 0; i < hingeItems.Count; ++i)
        {
            HingeItem item = hingeItems[i];
            if (item.hinge == null)
            {
                continue;
            }
            for (int n = 0; n < item.hinge.childCount; ++n)
            {
                Transform t = item.hinge.GetChild(n);
                t.parent = null;
            }
            GameObjectPool.Despawn(item.hinge.gameObject);
            item.hinge = null;
        }
    }

    public Transform GetHingeName(string name)
    {
        HingeItem h = null;
        if (hingeItems != null)
        {
            for (int i = 0; i < hingeItems.Count; ++i)
            {
                HingeItem hinge = hingeItems[i];
                if (hinge.name == name)
                {
                    h = hinge;
                    break;
                }
            }
        }

        return h.hinge;
    }

    public List<HingeItem> hingeItems;
}