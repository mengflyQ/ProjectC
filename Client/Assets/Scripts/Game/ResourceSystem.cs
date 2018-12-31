using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResourceSystem
{
    public delegate void OnLoadedAsset(UnityEngine.Object o);

    static Dictionary<string, AssetLoadRequest> mRequests = new Dictionary<string, AssetLoadRequest>();
    static List<AssetLoadRequest> mRequestList = new List<AssetLoadRequest>();

    public static void LoadAsync<T>(string path, OnLoadedAsset onLoaded) where T : UnityEngine.Object
    {
        if (mRequests.ContainsKey(path))
        {
            AssetLoadRequest r = mRequests[path];
            r.callback += onLoaded;
            return;
        }

        ResourceRequest request = Resources.LoadAsync<T>(path);

        if (request.isDone)
        {
            if (onLoaded != null)
            {
                onLoaded(request.asset);
            }
            return;
        }

        AssetLoadRequest alr = new AssetLoadRequest();
        alr.path = path;
        alr.callback = onLoaded;
        alr.request = request;

        mRequests.Add(path, alr);
        mRequestList.Add(alr);
        GameApp.Instance.StartCoroutine(OnLoading(alr));
    }

    public static void LogicTick()
    {
        //for (int i = mRequestList.Count - 1; i >= 0; --i)
        //{
        //    AssetLoadRequest request = mRequestList[i];
        //    if (!request.request.isDone)
        //    {
        //        continue;
        //    }
        //    if (request.callback != null)
        //    {
        //        request.callback(request.request.asset);
        //    }
        //    else
        //    {
        //        UnityEngine.Object.Destroy(request.request.asset);
        //    }
        //    int lastCount = mRequestList.Count - 1;
        //    mRequestList[i] = mRequestList[lastCount];
        //    mRequestList[lastCount] = request;
        //    mRequestList.RemoveAt(lastCount);
        //    mRequests.Remove(request.path);
        //}
    }

    static IEnumerator OnLoading(AssetLoadRequest request)
    {
        while (!request.request.isDone)
        {
            yield return null;
        }
        if (request.callback != null)
        {
            request.callback(request.request.asset);
        }
        else
        {
            UnityEngine.Object.Destroy(request.request.asset);
        }
        mRequests.Remove(request.path);
        mRequestList.Remove(request);
    }

    class AssetLoadRequest
    {
        public string path;
        public OnLoadedAsset callback;
        public ResourceRequest request;
    }

    public static void RemoveRequest(string path)
    {
        if (mRequests.ContainsKey(path))
        {
            AssetLoadRequest r = mRequests[path];
            r.callback = null;
            mRequests.Remove(path);
        }
    }

    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }
}
