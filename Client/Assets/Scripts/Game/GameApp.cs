using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameApp : MonoBehaviour
{
	void Awake()
	{
        Instance = this;

		ExcelLoader.Init();

		DontDestroyOnLoad(gameObject);

		excel_scn_list scnList = excel_scn_list.Find(1);
		scnLoadRequest = SceneManager.LoadSceneAsync(scnList.name);

		StartCoroutine(LoadingScn());
	}

    void OnApplicationQuit()
    {
        NavigationSystem.OnExitScene();
    }

	IEnumerator LoadingScn()
	{
		while (!scnLoadRequest.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
		GameController.OnLoadScene();
	}

    public static GameApp Instance
    {
        private set;
        get;
    }

	AsyncOperation scnLoadRequest;
}
