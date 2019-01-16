using System;
using System.Collections.Generic;
using UnityEngine;

class GameObjectPool : MonoBehaviour
{
    static GameObjectPool m_inst = null;
    public static void Startup()
    {
        if (m_inst == null)
        {
            GameObject go = new GameObject("GoPool");
            m_inst = go.AddComponent<GameObjectPool>();
        }
    }

    static public GameObject Spawn(string name)
    {
        if (m_inst == null)
            return new GameObject(name);

        List<GameObject> pool = m_inst.mPool;
        if (pool.Count > 0)
        {
            GameObject go = pool[0];
            go.SetActive(true);
            go.name = name;
            go.transform.parent = null;
            pool.RemoveAt(0);
            return go;
        }
        return new GameObject(name);
    }

    static public void Despawn(GameObject go)
    {
        if (!Application.isPlaying || go == null) return;

        if (m_inst == null)
            return;

        List<GameObject> pool = m_inst.mPool;
        if (!pool.Contains(go))
        {
            go.transform.parent = m_inst.transform;
            go.SetActive(false);
            pool.Add(go);
        }
    }

    private void OnDestroy()
    {
        if (m_inst == this)
            m_inst = null;
        for (int i = 0; i < mPool.Count; ++i)
        {
            GameObject go = mPool[i];
            GameObject.Destroy(go);
        }
        mPool.Clear();
    }

    List<GameObject> mPool = new List<GameObject>();
}