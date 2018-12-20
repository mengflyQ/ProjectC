﻿using System;
using System.Collections.Generic;
using ZyGames.Framework.Common.Serialization;
using GameServer.RoomServer;
using MathLib;

public class Scene
{
    public Scene(int scnID, int maxPlayerCount)
    {
        PlayerMaxCount = maxPlayerCount;

        IsVanish = false;

        ScnID = scnID;
        mScnList = excel_scn_list.Find(scnID);

        ScnUID = SceneManager.Instance.GenSceneID();
    }

    public void AddPlayer(Player player)
    {
        player.mScene = this;
        player.Initialize();

        mPlayersList.Add(player);
        mCharactersList.Add(player);

        mPlayers.Add(player.uid, player);
        mCharacters.Add(player.uid, player);
    }

    public void DelPlayer(Player player)
    {
        mPlayersList.Remove(player);
        mPlayers.Remove(player.uid);

        mCharactersList.Remove(player);
        mCharacters.Remove(player.uid);
    }

    public Player FindPlayer(int uid)
    {
        Player player = null;
        if (mPlayers.TryGetValue(uid, out player))
        {
            return player;
        }
        return null;
    }

    public Character FindCharacter(int uid)
    {
        Character cha = null;
        if (mCharacters.TryGetValue(uid, out cha))
        {
            return cha;
        }
        return null;
    }

    public int GetPlayerCount()
    {
        return mPlayersList.Count;
    }

    public Player GetPlayerByIndex(int index)
    {
        if (index < 0 || index >= mPlayersList.Count)
            return null;
        return mPlayersList[index];
    }

    public uint GetCellIndex(Vector3 pos)
    {
        int indexX = (int)(pos.x / mCellSize);
        int indexZ = (int)(pos.z / mCellSize);

        return EncodeCellIndex(indexX, indexZ);
    }

    public uint EncodeCellIndex(int x, int z)
	{
		return ((uint)(x + 30000) << 16) | ((uint)(z + 30000) & 0xFFFF);
	}

    public void DecodeCellIndex(uint index, out int x, out int z)
    {
        x = (int)(index >> 16) - 30000;
        z = (int)(index & 0xFFFF) - 30000;
    }

    public void UpdateVisible(Character cha)
    {
        uint index = GetCellIndex(cha.Position);
        if (index != cha.mVisibleIndex)
        {
            UpdateCellIndex(cha.mVisibleIndex, index, cha);
            cha.mVisibleIndex = index;
        }

        for (int i = cha.mVisibleObjs.Count - 1; i >= 0; --i)
        {
            Character visibleCha = cha.mVisibleObjs[i];
            if (SkillUtility.GetDistance(cha, visibleCha, DistanceCalcType.Center) > mScnList.viewDist)
            {
                int lastIndex = cha.mVisibleObjs.Count - 1;
                Character tmp = visibleCha;
                cha.mVisibleObjs[i] = cha.mVisibleObjs[lastIndex];
                cha.mVisibleObjs[lastIndex] = tmp;

                cha.mVisibleObjs.RemoveAt(lastIndex);

                cha.OnExitView(tmp);

                tmp.mVisibleObjs.Remove(cha);
                tmp.OnExitView(cha);
            }
        }

        int minIndexX, minIndexZ;
        int maxIindexX, maxIndexZ;
        Vector3 minPos = cha.Position - mScnList.viewDist;
        Vector3 maxPos = cha.Position + mScnList.viewDist;
        uint minIndex = GetCellIndex(minPos);
        uint maxIndex = GetCellIndex(maxPos);
        DecodeCellIndex(minIndex, out minIndexX, out minIndexZ);
        DecodeCellIndex(maxIndex, out maxIindexX, out maxIndexZ);

        for (int x = minIndexX; x <= maxIindexX; ++x)
        {
            for (int z = minIndexZ; z <= maxIndexZ; ++z)
            {
                uint idx = EncodeCellIndex(x, z);

                List<Character> visibles;
                if (!mVisibleObjs.TryGetValue(idx, out visibles))
                {
                    continue;
                }
                for (int i = 0; i < visibles.Count; ++i)
                {
                    Character c = visibles[i];
                    if (SkillUtility.GetDistance(cha, c, DistanceCalcType.Center) <= mScnList.viewDist)
                    {
                        if (cha.mVisibleObjs.Contains(c) || c == cha)
                            continue;
                        cha.mVisibleObjs.Add(c);
                        c.mVisibleObjs.Add(cha);

                        cha.OnEnterView(c);
                        c.OnEnterView(cha);
                    }
                }
            }
        }
    }

    void UpdateCellIndex(uint oldIndex, uint newIndex, Character cha)
    {
        List<Character> lsVisibles = null;
        if (mVisibleObjs.TryGetValue(oldIndex, out lsVisibles))
        {
            lsVisibles.Remove(cha);
            if (lsVisibles.Count == 0)
            {
                mVisibleObjs.Remove(oldIndex);
            }
        }
        if (!mVisibleObjs.TryGetValue(newIndex, out lsVisibles))
        {
            lsVisibles = new List<Character>();
            mVisibleObjs.Add(newIndex, lsVisibles);
        }
        lsVisibles.Add(cha);
    }

    // 当所有角色加载场景完毕，通知客户端开始游戏;
    public void StartClientGame()
    {
        NotifyStartGame startGame = new NotifyStartGame();
        startGame.Players = new List<ScnPlayerInfo>();

        for (int i = 0; i < mPlayersList.Count; ++i)
        {
            Player player = mPlayersList[i];

            ScnPlayerInfo playerInfo = new ScnPlayerInfo();
            playerInfo.Name = player.mNickName;
            playerInfo.UserID = player.uid;
            startGame.Players.Add(playerInfo);
        }

        for (int i = 0; i < mPlayersList.Count; ++i)
        {
            Player player = mPlayersList[i];

            NetWork.NotifyMessage<NotifyStartGame>(player.uid, STC.STC_StartClienGame, startGame);
        }
    }

    public void Tick()
    {
        if (IsVanish)
            return;
        for (int i = 0; i < mCharactersList.Count; ++i)
        {
            Character cha = mCharactersList[i];
            if (cha == null)
            {
                Console.WriteLine("Error: Character List Contains Empty Character!");
                continue;
            }
            cha.Update();
        }
    }

    public bool IsVanish
    {
        private set;
        get;
    }

    public int ScnUID
    {
        private set;
        get;
    }

    public int ScnID
    {
        set;
        get;
    }

    public int PlayerMaxCount
    {
        private set;
        get;
    }

    Dictionary<int, Character> mCharacters = new Dictionary<int, Character>();
    Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();

    List<Character> mCharactersList = new List<Character>();
    List<Player> mPlayersList = new List<Player>();

    excel_scn_list mScnList = null;
    float mCellSize = 5.0f;

    Dictionary<uint, List<Character>> mVisibleObjs = new Dictionary<uint, List<Character>>();
}