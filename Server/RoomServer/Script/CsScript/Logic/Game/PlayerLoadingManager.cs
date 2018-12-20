using System;
using System.Collections.Generic;
using ZyGames.Framework.Game.Contract;
using GameServer.Model;
using GameServer.RoomServer;

// 玩家加载场景期间，暂时把玩家数据存于此;
public class PlayerLoadingManager : BaseSystem
{
    public Player AddPlayer(int uid, GameSession session)
    {
        SessionUser user = new SessionUser();
        user.UserId = uid;

        Player oldPlayer = null;
        if (mPlayersMap.TryGetValue(uid, out oldPlayer))
        {
            oldPlayer.OnReplace();
            mPlayers.Remove(oldPlayer);
            mPlayersMap.Remove(uid);
        }

        session.Bind(user);

        Player player = new Player();
        player.uid = uid;
        player.mSession = session;
        player.mStatus = PlayerStatus.Online;

        mPlayers.Add(player);
        mPlayersMap[uid] = player;

        return player;
    }

    public Player FindPlayer(int uid)
    {
        Player player;
        if (!mPlayersMap.TryGetValue(uid, out player))
        {
            return null;
        }
        return player;
    }

    public void RemovePlayer(Player player)
    {
        mPlayers.Remove(player);
        mPlayersMap.Remove(player.uid);
    }

    static PlayerLoadingManager mInstance = null;
    public static PlayerLoadingManager Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new PlayerLoadingManager();
            }                   
            return mInstance;
        }
    }

    List<Player> mPlayers = new List<Player>();
    Dictionary<int, Player> mPlayersMap = new Dictionary<int, Player>();
}
