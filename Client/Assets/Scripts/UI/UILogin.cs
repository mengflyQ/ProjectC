using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UILogin : MonoBehaviour
{
    void Awake()
    {
        
    }

    public void OnClickLogin()
    {
        NetWriter.SetUrl("127.0.0.1:9001");
        Net.Instance.Send((int)ActionType.Login, LoginCallback, null);
    }

    private void LoginCallback(ActionResult actionResult)
    {
        StartCoroutine(LoadingScn());
    }

    IEnumerator LoadingScn()
    {
        excel_scn_list scnList = excel_scn_list.Find(2);
        AsyncOperation scnLoadRequest = SceneManager.LoadSceneAsync(scnList.name);
        while (!scnLoadRequest.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        GameController.OnLoadScene();
    }

    public string UserName
    {
        get
        {
            return userNameInput.text;
        }
    }

    public InputField userNameInput;
    public Button loginBtn;
}
