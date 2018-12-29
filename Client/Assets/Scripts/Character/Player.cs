﻿using UnityEngine;

public class Player : Character
{
    protected override void Initialize()
    {
        Type = CharacterType.Player;
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
}