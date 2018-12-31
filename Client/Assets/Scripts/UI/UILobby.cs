using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoBuf;
using ZyGames.Framework.Common.Serialization;

[Serializable]
public class UIPlayerInfo
{
    public Text playerName;
    public Text playerLevel;
}

public class UILobby : MonoBehaviour
{
    void Awake()
    {
        Instance = this;

        playerIDs = new int[texPlayerNames.Length];
        ResetRoomGUI();

        MessageSystem.Instance.MsgRegister(MessageType.SetPlayerInfo, OnSetPlayerInfo);

        NetWork.RegisterNotify(STC.STC_RoomInfo, NotifyRoomInfo);
        NetWork.RegisterNotify(STC.STC_RoomAddPlayer, NotifyRoomAddPlayer);
        NetWork.RegisterNotify(STC.STC_MatchFailed, NotifyMatchOver);
        NetWork.RegisterNotify(STC.STC_MatchReady, NotifyMatchReady);
        NetWork.RegisterNotify(STC.STC_SceneReady, NotifyScnReady);
    }

    void FixedUpdate()
    {
        if (endTime <= 0.0f)
            return;
        restTime = endTime - Time.realtimeSinceStartup;
        restTimeText.text = string.Format("{0}s", (int)restTime);
    }

    void ResetRoomGUI()
    {
        for (int i = 0; i < playerIDs.Length; ++i)
        {
            playerIDs[i] = 0;
            playerLocks[i].SetActive(false);
            Text uiText = texPlayerNames[i];
            uiText.text = string.Empty;
        }
        readyBtn.interactable = true;

        mainMenuRoot.SetActive(true);
        matchRoot.SetActive(false);

        restTime = 30.0f;
        endTime = 0.0f;
        playerCount = 0;
    }

    void OnSetPlayerInfo(params object[] objs)
    {
        string nickName = (string)objs[0];
        int level = (int)objs[1];

        playerInfo.playerName.text = nickName;
        playerInfo.playerLevel.text = string.Format("等级：{0}", level);
    }

    public void BeginMatch()
    {
        ReqMatch req = new ReqMatch();
        req.UserID = GameController.mUserInfo.uid;
        NetWork.SendPacket<ReqMatch>(CTS.CTS_Match, req, null);
    }

    public void ReadyMatch()
    {
        EmptyMsg req = new EmptyMsg();
        NetWork.SendPacket<EmptyMsg>(CTS.CTS_MatchReady, req, null);
    }

    void NotifyRoomInfo(byte[] data)
    {
        NotifyRoomInfo roomInfo = ProtoBufUtils.Deserialize<NotifyRoomInfo>(data);

        for (int i = 0; i < roomInfo.Players.Count; ++i)
        {
            RoomPlayerInfo roomPlayerInfo = roomInfo.Players[i];
            if (roomPlayerInfo == null)
                continue;
            texPlayerNames[i].text = roomPlayerInfo.Name;
            playerIDs[i] = roomPlayerInfo.UserID;
            ++playerCount;
        }
        restTime = roomInfo.RestTime;
        endTime = Time.realtimeSinceStartup + roomInfo.RestTime;

        mainMenuRoot.SetActive(false);
        matchRoot.SetActive(true);
    }

    void NotifyRoomAddPlayer(byte[] data)
    {
        NotifyRoomAddPlayer addPlayerInfo = ProtoBufUtils.Deserialize<NotifyRoomAddPlayer>(data);

        texPlayerNames[playerCount].text = addPlayerInfo.NewPlayer.Name;
        playerIDs[playerCount] = addPlayerInfo.NewPlayer.UserID;
        ++playerCount;
    }

    void NotifyMatchOver(byte[] data)
    {
        ResetRoomGUI();
    }

    void NotifyMatchReady(byte[] data)
    {
        ReqMatch msg = ProtoBufUtils.Deserialize<ReqMatch>(data);

        if (msg.UserID == GameController.mUserInfo.uid)
        {
            readyBtn.interactable = false;
        }

        for (int i = 0; i < playerIDs.Length; ++i)
        {
            int uid = playerIDs[i];
            if (msg.UserID != uid)
                continue;
            playerLocks[i].SetActive(true);
        }
    }

    void NotifyScnReady(byte[] data)
    {
        NotifySceneReady roomReady = ProtoBufUtils.Deserialize<NotifySceneReady>(data);

        GameController.mScnUID = roomReady.SceneID;
        NetWork.SetUrl(ServerManager.RoomServerUrl);
        SceneSystem.Instance.ChangeScene(SceneSystem.roomScnID);

        ReqEnterScene reqEnterScn = GetReqEnterScn();
        reqEnterScn.UserID = GameController.mUserInfo.uid;
        reqEnterScn.NickName = GameController.mUserInfo.nickName;
        reqEnterScn.ClassID = 1;
        NetWork.SendPacket<ReqEnterScene>(CTS.CTS_EnterScn, reqEnterScn, null);
    }

    private static ReqEnterScene GetReqEnterScn()
    {
        return new ReqEnterScene();
    }

    public UILobby Instance
    {
        private set;
        get;
    }

    public UIPlayerInfo playerInfo;
    public Button btnMatch;
    public GameObject mainMenuRoot;
    public GameObject matchRoot;
    public Button[] btnPlayers;
    public Text[] texPlayerNames;
    public GameObject[] playerLocks;
    private int[] playerIDs;
    private int playerCount;
    public Button readyBtn;
    public Text restTimeText;
    private float restTime;
    private float endTime;
}
