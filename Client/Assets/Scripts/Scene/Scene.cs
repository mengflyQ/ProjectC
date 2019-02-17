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

        GameController.mServerStartTime = startGame.ServerStartTime;
        GameController.mClientStartTime = Time.realtimeSinceStartup;
        
        for (int i = 0; i < startGame.Players.Count; ++i)
        {
            ScnPlayerInfo playerInfo = startGame.Players[i];

            excel_cha_class chaClass = excel_cha_class.Find(mScnLists.temp);
            if (chaClass == null)
                continue;

            excel_cha_list chaList = excel_cha_list.Find(chaClass.chaListID);

            GameObject o = ResourceSystem.Load<GameObject>(chaList.path);
            if (o != null)
            {
                GameObject mainPlayer = GameObject.Instantiate(o);
                Player player = mainPlayer.GetComponent<Player>();
                player.gid = playerInfo.GID;
                player.UserID = playerInfo.UserID;
                player.mChaList = chaList;

                mainPlayer.transform.position = new Vector3(82.51f, 7.25f, 34.82f);
                mainPlayer.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);

                mPlayersList.Add(player);
                mCharacterList.Add(player);
                mPlayers.Add(player.gid, player);
                mCharacters.Add(player.gid, player);

                if (GameController.mUserInfo.uid == playerInfo.UserID)
                {
                    player.mEvent += TargetChgEvent;

                    MessageSystem.Instance.MsgDispatch(MessageType.OnSetChaClass, chaClass);

                    GameController.OnPlayerInit(player);
                }
            }

            //ResourceSystem.LoadAsync<GameObject>(chaList.path, (obj) =>
            //{
            //    GameObject o = obj as GameObject;
            //    if (o != null)
            //    {
            //        GameObject mainPlayer = GameObject.Instantiate(o);
            //        Player player = mainPlayer.GetComponent<Player>();
            //        player.gid = playerInfo.GID;
            //        player.UserID = playerInfo.UserID;
            //        player.mChaList = chaList;

            //        mainPlayer.transform.position = new Vector3(82.51f, 7.25f, 34.82f);
            //        mainPlayer.transform.localScale = new Vector3(chaList.scale[0], chaList.scale[1], chaList.scale[2]);

            //        mPlayersList.Add(player);
            //        mCharacterList.Add(player);
            //        mPlayers.Add(player.gid, player);
            //        mCharacters.Add(player.gid, player);

            //        if (GameController.mUserInfo.uid == playerInfo.UserID)
            //        {
            //            player.mEvent += TargetChgEvent;

            //            MessageSystem.Instance.MsgDispatch(MessageType.OnSetChaClass, chaClass);

            //            GameController.OnPlayerInit(player);
            //        }
            //    }
            //});
        }
    }

    void TargetChgEvent(CharacterEventType evtType, Character self)
    {
        if (evtType != CharacterEventType.OnTargetChg)
            return;
        TargetCircle.Instance.SetTarget(self.GetTarget());
    }

    public Player GetPlayer(int uid)
    {
        Player player;
        if (mPlayers.TryGetValue(uid, out player))
        {
            return player;
        }
        return null;
    }

    public Character GetCharacter(int uid)
    {
        Character cha;
        if (mCharacters.TryGetValue(uid, out cha))
        {
            return cha;
        }
        return null;
    }

    public int GetCharacterCount()
    {
        return mCharacterList.Count;
    }

    public Character GetCharacterByIndex(int index)
    {
        return mCharacterList[index];
    }

    void InitializeNet()
    {
        NetWork.RegisterNotify(STC.STC_StartClienGame, OnInitPlayers);
    }

    public excel_scn_list mScnLists = null;

    public List<Character> mCharacterList = new List<Character>();
    public Dictionary<int, Character> mCharacters = new Dictionary<int, Character>();

    public List<Player> mPlayersList = new List<Player>();
    public Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();
}
