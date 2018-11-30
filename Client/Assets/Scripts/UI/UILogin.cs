using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using ProtoBuf;
using ZyGames.Framework.Common.Serialization;

public class UILogin : MonoBehaviour
{
    void Awake()
    {
        
    }

    public void OnClickLogin()
    {
        GameApp.Instance.UserName = UserName;

        NetWriter.SetUrl(ServerManager.LoginServerUrl);

        LoginData data = new LoginData();
        data.UserName = GameApp.Instance.UserName;
        data.DeviceUniqueIdentifier = GameApp.Instance.DeviceUniqueIdentifier;
        data.DeviceModel = GameApp.Instance.DeviceModel;
        data.DeviceTypeStr = GameApp.Instance.DeviceTypeStr;
        data.Platform = GameApp.Instance.Platform;
        data.ScreenWidth = GameApp.Instance.ScreenWidth;
        data.ScreenHeight = GameApp.Instance.ScreenHeight;
        NetWork.SendPacket<LoginData>(CTS.CTS_Login, data, LoginCallback);
    }

    public void OnClickReigst()
    {
        NetWriter.SetUrl(ServerManager.LoginServerUrl);
        RegistData data = new RegistData();
        data.NickName = registInput.text;
        NetWork.SendPacket<RegistData>(CTS.CTS_Regist, data, LoginCallback);
    }

    private void LoginCallback(byte[] responseData)
    {
        if (responseData == null)
        {
            return;
        }

        LoginResponse response = null;
        bool regist = (responseData.Length == 4);
        if (regist)
        {
            GameController.mUserInfo.uid = BitConverter.ToInt32(responseData, 0);
            loginFrame.SetActive(false);
            registFrame.SetActive(true);
            return;
        }
        else
        {
            response = ProtoBufUtils.Deserialize<LoginResponse>(responseData);
            GameController.mUserInfo.uid = response.UserID;
            loginFrame.SetActive(true);
            registFrame.SetActive(false);
        }

        if (response == null)
        {
            Debug.LogError("网络解析错误，未解析LoginResponse");
            return;
        }

        Net.Instance.CloseSocket();

        SceneSystem.Instance.ChangeScene(SceneSystem.lobbyScnID, (scn) => {
            NetWriter.SetUrl(ServerManager.LobbyServerUrl);
            NetWork.SendPacket<LoginResponse>(CTS.CTS_EnterLobby, response, EnterLobbyCallback);
        });
    }

    private void EnterLobbyCallback(byte[] result)
    {
        if (result == null)
            return;
        EnterLobbyResponse enterLobby = ProtoBufUtils.Deserialize<EnterLobbyResponse>(result);
        MessageSystem.Instance.MsgDispatch(MessageType.SetPlayerInfo, enterLobby.NickName, enterLobby.Level);
    }

    public string UserName
    {
        get
        {
            return userNameInput.text;
        }
    }

    public GameObject loginFrame;
    public GameObject registFrame;
    public InputField userNameInput;
    public InputField registInput;
}
