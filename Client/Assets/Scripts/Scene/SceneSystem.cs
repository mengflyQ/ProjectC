using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneSystem
{
    public void ChangeScene(int scnID, Action<Scene> onLoaded = null)
    {
        Debug.Log("Change Scene To " + scnID + " ...");
        if (mCurrentScene != null)
        {
            mCurrentScene.Exit();
            mCurrentScene = null;
        }
        excel_scn_list scnList = excel_scn_list.Find(scnID);
        if (scnList == null)
        {
            Debug.LogError("没有找到ID为" + scnID + "的场景表!");
            return;
        }
        GameApp.Instance.StartCoroutine(LoadingScn(scnList, onLoaded));
    }

    IEnumerator LoadingScn(excel_scn_list scnList, Action<Scene> onLoaded)
    {
        AsyncOperation scnLoadRequest = SceneManager.LoadSceneAsync(scnList.name);
        while (!scnLoadRequest.isDone)
        {
            yield return null;
        }

        Scene scn = new Scene();
        scn.mScnLists = scnList;

        mCurrentScene = scn;

        if (onLoaded != null)
        {
            onLoaded(scn);
        }

        scn.Enter();
    }

    static SceneSystem mInstance = null;
    public static SceneSystem Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new SceneSystem();
            return mInstance;
        }
    }

    public const int loginScnID = 1;
    public const int lobbyScnID = 2;
    public const int roomScnID = 3;

    public Scene mCurrentScene = null;

    public delegate void OnLoadedScene(Scene scn);
    OnLoadedScene mOnLoaded = null;
}
