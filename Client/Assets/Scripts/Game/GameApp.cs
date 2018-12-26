using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameApp : MonoBehaviour
{
	void Awake()
	{
        if (Instance != null)
        {
            Debug.LogError("重复创建GameApp！");
            GameObject.Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        UserName = "";
        Platform = Application.platform.ToString();
        DeviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;
        DeviceModel = SystemInfo.deviceModel;
        DeviceTypeStr = SystemInfo.deviceType.ToString();

        Debug.Log("SystemInfo.deviceModel: " + SystemInfo.deviceModel);
        Debug.Log("SystemInfo.deviceName: " + SystemInfo.deviceName);
        Debug.Log("SystemInfo.deviceType: " + SystemInfo.deviceType.ToString());
        Debug.Log("SystemInfo.graphicsDeviceID: " + SystemInfo.graphicsDeviceID);
        Debug.Log("SystemInfo.graphicsDeviceName: " + SystemInfo.graphicsDeviceName);
        Debug.Log("SystemInfo.graphicsDeviceType: " + SystemInfo.graphicsDeviceType.ToString());
        Debug.Log("SystemInfo.graphicsDeviceVendor: " + SystemInfo.graphicsDeviceVendor);
        Debug.Log("SystemInfo.graphicsDeviceVendorID: " + SystemInfo.graphicsDeviceVendorID);
        Debug.Log("SystemInfo.graphicsDeviceVersion: " + SystemInfo.graphicsDeviceVersion);
        ScreenWidth = Screen.width;
        ScreenHeight = Screen.height;

        // Net.Instance.HeadFormater = new CustomHeadFormater();
		ExcelLoader.Init();
        SkillEventRegister.Initialize();

        if (directGame)
        {
            SceneSystem.Instance.ChangeScene(SceneSystem.roomScnID);
            return;
        }
        SceneSystem.Instance.ChangeScene(SceneSystem.loginScnID);
	}

    void FixedUpdate()
    {
        GameController.LogicTick();
    }

    void OnApplicationQuit()
    {
        NavigationSystem.OnExitScene();
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

    public string DeviceModel
    {
        get;
        private set;
    }

    public string DeviceTypeStr
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

    public bool directGame = false;
}
