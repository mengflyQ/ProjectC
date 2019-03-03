using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;


public enum PlayerStatus
{
    Online,
    Offline
}

public partial class Player : Character
{
    public Player() : base()
    {
        Type = CharacterType.Player;
    }

    public override void Initialize()
    {
        base.Initialize();
        InitPlayerAtb();
    }

    public override void Destroy()
    {
        base.Destroy();
        mScene.DelPlayer(this);
    }

    public void OnReplace()
    {
        if (mSession != null)
        {
            mSession.Close();
        }
    }

    public override void SetCannotFlag(CannotFlag flag, OptType type, bool cannot)
    {
        if (flag == CannotFlag.CannotControl && cannot)
        {
            IsControl = false;
        }
        base.SetCannotFlag(flag, type, cannot);
    }

    private bool mIsControl = false;
    public bool IsControl
    {
        set
        {
            if (IsCannotFlag(CannotFlag.CannotControl))
                return;
            if (mIsControl == value)
                return;
            mIsControl = value;
            if (mIsControl && mCurSkill != null)
            {
                mCurSkill.OnMove();
            }
        }
        get
        {
            return mIsControl;
        }
    }

    public int UserID
    {
        set;
        get;
    }

    public excel_cha_class mChaClass = null;

    public GameSession mSession;

    public PlayerStatus mStatus;
}
