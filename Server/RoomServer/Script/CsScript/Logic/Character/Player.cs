using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;


public enum PlayerStatus
{
    Online,
    Offline
}

public class Player : Character
{
    public Player() : base()
    {

    }

    public void OnReplace()
    {
        if (mSession != null)
        {
            mSession.Close();
        }
    }

    public GameSession mSession;

    public PlayerStatus mStatus;
}
