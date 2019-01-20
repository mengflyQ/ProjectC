using System;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;

public class UIRoot3D : MonoBehaviour
{
    private void Awake()
    {
        MessageSystem.Instance.MsgRegister(MessageType.InitHeadBar, OnInitHeadBar);

        GameObject spawnPoolGO = new GameObject("HeadTextPool");
        spawnPoolGO.transform.position = Vector3.zero;
        spawnPoolGO.transform.rotation = Quaternion.identity;
        spawnPoolGO.transform.localScale = Vector3.one;
        mHeadTextPool = spawnPoolGO.AddComponent<SpawnPool>();
        mHeadTextPool.poolName = "HeadTextPool";
        mHeadTextPool.dontDestroyOnLoad = true;

        headTextTransform = ResourceSystem.Load<Transform>("GUI/UI_HeadText");
        PrefabPool refabPool = new PrefabPool(headTextTransform);
        //默认初始化两个Prefab
        refabPool.preloadAmount = 0;
        //开启限制
        refabPool.limitInstances = true;
        //关闭无限取Prefab
        refabPool.limitFIFO = false;
        //限制池子里最大的Prefab数量
        refabPool.limitAmount = 20;
        //开启自动清理池子
        refabPool.cullDespawned = true;
        //最终保留
        refabPool.cullAbove = 5;
        //多久清理一次
        refabPool.cullDelay = 20;
        //每次清理几个
        refabPool.cullMaxPerPass = 10;
        //初始化内存池
        mHeadTextPool._perPrefabPoolOptions.Add(refabPool);
        mHeadTextPool.CreatePrefabPool(mHeadTextPool._perPrefabPoolOptions[mHeadTextPool.Count]);
    }

    private void OnDestroy()
    {
        mHeadTextPool.DespawnAll();
        Destroy(mHeadTextPool.gameObject);
        MessageSystem.Instance.MsgUnregister(MessageType.InitHeadBar, OnInitHeadBar);
    }

    void OnInitHeadBar(params object[] param)
    {
        Character cha = param[0] as Character;

        ResourceSystem.LoadAsync<GameObject>("GUI/UI_HeadBar", (o) =>
        {
            GameObject go = GameObject.Instantiate(o) as GameObject;
            if (go == null)
                return;
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

            UIHeadBar headBar = go.GetComponent<UIHeadBar>();
            if (headBar == null)
                return;
            headBar.UICamera = uiCamera;
            if (MobaMainCamera.MainCamera != null)
            {
                headBar.WorldCamera = MobaMainCamera.MainCamera;
            }
            headBar.Owner = cha;

            if (cha == null || cha.HingePoints == null)
                return;
            Transform hinge = cha.HingePoints.GetHingeName("HeadBar");
            if (hinge == null)
                return;
            headBar.Hinge = hinge;
            headBar.UIRoot = this;

            cha.headBar = headBar;
        });
    }

    public Transform headTextTransform = null;

    public Camera uiCamera;

    public SpawnPool mHeadTextPool;
}