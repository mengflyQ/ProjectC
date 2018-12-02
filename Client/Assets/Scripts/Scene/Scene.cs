using UnityEngine;
using System;
using System.Collections.Generic;
using ProtoBuf;
using ZyGames.Framework.Common.Serialization;

public class Scene
{
    public void Enter()
    {
        if (mScnLists.temp > 0)
        {
            GameController.OnLoadScene();

            ReqLoadedScn reqLoadedScn = new ReqLoadedScn();
            reqLoadedScn.ScnUID = GameController.mScnUID;
            reqLoadedScn.ScnID = mScnLists.id;
            NetWork.SendPacket<ReqLoadedScn>(CTS.CTS_LoadedScn, reqLoadedScn, null);

            InitializeNet();
        }
    }

    public void Exit()
    {

    }

    void OnInitPlayers(byte[] data)
    {
        NotifyStartGame startGame = ProtoBufUtils.Deserialize<NotifyStartGame>(data);
        excel_scn_list scnList = SceneSystem.Instance.mCurrentScene.mScnLists;

        Debug.LogError("---------------- " + startGame.Players.Count);
        for (int i = 0; i < startGame.Players.Count; ++i)
        {
            ScnPlayerInfo playerInfo = startGame.Players[i];

            if (GameController.mUserInfo.uid == playerInfo.UserID)
                continue;

            excel_cha_list chaList = excel_cha_list.Find(scnList.temp);

            GameObject o = ResourceSystem.Load<GameObject>(chaList.path);
            if (o != null)
            {
                GameObject mainPlayer = GameObject.Instantiate(o);
                Player player = mainPlayer.GetComponent<Player>();
                player.mChaList = chaList;

                mainPlayer.transform.position = new Vector3(82.51f, 7.25f, 34.82f);
                mainPlayer.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);
            }
        }
    }

    void InitializeNet()
    {
        NetWork.RegisterNotify(STC.STC_StartClienGame, OnInitPlayers);
    }

    public excel_scn_list mScnLists = null;
    public List<Character> mPlayers = new List<Character>();
}
