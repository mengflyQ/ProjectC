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
        Type = CharacterType.Player;
    }

    public void OnReplace()
    {
        if (mSession != null)
        {
            mSession.Close();
        }
    }

    private bool mIsControl = false;
    public bool IsControl
    {
        set
        {
            if (mIsControl == value)
                return;
            mIsControl = value;
        }
        get
        {
            return mIsControl;
        }
    }

    public GameSession mSession;

    public PlayerStatus mStatus;
}
