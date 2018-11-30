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

        MessageSystem.Instance.MsgRegister(MessageType.SetPlayerInfo, OnSetPlayerInfo);

        NetWork.RegisterNotify(STC.STC_RoomInfo, NotifyRoomInfo);
        NetWork.RegisterNotify(STC.STC_MatchFailed, NotifyMatchFailed);
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

    void NotifyRoomInfo(byte[] data)
    {
        for (int i = 0; i < texPlayerNames.Length; ++i)
        {
            Text uiText = texPlayerNames[i];
            uiText.text = string.Empty;
        }

        NotifyRoomInfo roomInfo = ProtoBufUtils.Deserialize<NotifyRoomInfo>(data);

        for (int i = 0; i < roomInfo.Players.Count; ++i)
        {
            RoomPlayerInfo roomPlayerInfo = roomInfo.Players[i];
            if (roomPlayerInfo == null)
                continue;
            texPlayerNames[i].text = roomPlayerInfo.Name;
        }

        mainMenuRoot.SetActive(false);
        matchRoot.SetActive(true);
    }

    void NotifyMatchFailed(byte[] data)
    {
        mainMenuRoot.SetActive(true);
        matchRoot.SetActive(false);
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
}
