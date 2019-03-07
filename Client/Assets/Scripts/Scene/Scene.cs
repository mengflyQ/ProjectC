using UnityEngine;
using System;
using System.Collections.Generic;
using ProtoBuf;
using ZyGames.Framework.Common.Serialization;

public partial class Scene
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
        if (mScnLists.temp > 0)
        {
            UninitializeNet();
        }
        mTriggers.Clear();
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

    public void DelPlayer(Player player)
    {
        mPlayersList.Remove(player);
        mPlayers.Remove(player.gid);

        mCharacterList.Remove(player);
        mCharacters.Remove(player.gid);
    }

    public void DelNPC(NPC npc)
    {
        mNPCList.Remove(npc);
        mNPCs.Remove(npc.gid);

        mCharacterList.Remove(npc);
        mCharacters.Remove(npc.gid);
    }

    public excel_scn_list mScnLists = null;

    public List<Character> mCharacterList = new List<Character>();
    public Dictionary<int, Character> mCharacters = new Dictionary<int, Character>();

    public List<Player> mPlayersList = new List<Player>();
    public List<NPC> mNPCList = new List<NPC>();
    public Dictionary<int, Player> mPlayers = new Dictionary<int, Player>();
    public Dictionary<int, NPC> mNPCs = new Dictionary<int, NPC>();

    public List<Trigger> mTriggers = new List<Trigger>();
}
