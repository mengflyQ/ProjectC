using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;

public class SceneSystem
{
    public void ChangeScene(int scnID)
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
        GameApp.Instance.StartCoroutine(LoadingScn(scnList));
    }

    IEnumerator LoadingScn(excel_scn_list scnList)
    {
        AsyncOperation scnLoadRequest = SceneManager.LoadSceneAsync(scnList.name);
        while (!scnLoadRequest.isDone)
        {
            yield return null;
        }

        Scene scn = new Scene();
        scn.mScnLists = scnList;

        mCurrentScene = scn;

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

    public Scene mCurrentScene = null;
}
