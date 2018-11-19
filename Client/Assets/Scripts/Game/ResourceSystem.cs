using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ResourceSystem
{
    public delegate void OnLoadedAsset(UnityEngine.Object o);

    static Dictionary<string, AssetLoadRequest> mRequests = new Dictionary<string, AssetLoadRequest>();

    public static void LoadAsync<T>(string path, OnLoadedAsset onLoaded) where T : UnityEngine.Object
    {
        if (mRequests.ContainsKey(path))
        {
            AssetLoadRequest r = mRequests[path];
            r.callback += onLoaded;
            return;
        }

        ResourceRequest request = Resources.LoadAsync<T>(path);

        AssetLoadRequest alr = new AssetLoadRequest();
        alr.path = path;
        alr.callback = onLoaded;
        alr.request = request;

        GameApp.Instance.StartCoroutine(OnLoading(alr));
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
