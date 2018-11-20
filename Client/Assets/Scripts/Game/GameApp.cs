using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameApp : MonoBehaviour
{
	void Awake()
	{
        Instance = this;

        UserName = "";
        Password = "";
        Platform = Application.platform.ToString();
        DeviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

		ExcelLoader.Init();

		DontDestroyOnLoad(gameObject);

		StartCoroutine(LoadingScn());
	}

    void OnApplicationQuit()
    {
        NavigationSystem.OnExitScene();
    }

	IEnumerator LoadingScn()
	{
        excel_scn_list scnList = excel_scn_list.Find(loginScnID);
        AsyncOperation scnLoadRequest = SceneManager.LoadSceneAsync(scnList.name);
		while (!scnLoadRequest.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
	}

    public static GameApp Instance
    {
        private set;
        get;
    }

    public string Platform
    {
        get;
        private set;
    }

    public string DeviceUniqueIdentifier
    {
        get;
        private set;
    }

    public int ScreenWidth
    {
        get;
        private set;
    }

    public int ScreenHeight
    {
        get;
        private set;
    }

    public string UserName
    {
        get;
        set;
    }

    public string Password
    {
        get;
        set;
    }

    private const int loginScnID = 1;
}
