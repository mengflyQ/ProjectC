﻿using System;
using System.Collections.Generic;
using GameServer.Model;
using ZyGames.Framework.Game.Contract;
using MathLib;
using GameServer.RoomServer;

public partial class Character
{
    protected void InitMove()
    {
        mStartTime = Time.ElapsedSeconds;
    }

    protected void UpdateMove()
    {
        if (mPosDirty)
        {
            UpdateVisibleInfo();
            mPosDirty = false;
        }
        if (!IsCannotFlag(CannotFlag.CannotMove) && Speed > 0.0f)
        {
            Position += Time.DeltaTime * mDirection;
            float h;
            if (NavigationSystem.GetLayerHeight(Position, mNavLayer, out h))
            {
                Vector3 pos = Position;
                pos.y = h;
                Position = pos;
            }
            else
            {
                mNavLayer = NavigationSystem.GetLayer(Position);
            }
        }

        UpdateSearchMove();
    }

    void UpdateSearchMove()
    {
        if (mPath == null || mPath.Length == 0)
            return;
        Vector3 nextPos = mPath[mCurrentNodeIndex];
        Vector3 dir = nextPos - Position;
        if (dir.Length() <= 0.1f)
        {
            ++mCurrentNodeIndex;
            if (mCurrentNodeIndex >= mPath.Length)
            {
                Direction = dir;
                StopSearchMove();
            }
            return;
        }
        Vector3 targetPos = mPath[mPath.Length - 1];
        if (Vector3.Distance(Position, targetPos) <= mDestRadius)
        {
            Direction = dir;
            StopSearchMove();
            return;
        }
        Direction = dir;
        Speed = 3.0f;
    }

    public void StopMove(bool sync = true)
    {
        Speed = 0.0f;
        if (IsSearchMoving())
        {
            StopSearchMove(sync);
        }
    }

    public bool IsSearchMoving()
    {
        return mPath != null;
    }

    public void StopSearchMove(bool sync = true)
    {
        Speed = 0.0f;
        mPath = null;
        mCurrentNodeIndex = 0;
    }

    public void SearchMove(Vector3 pos, float destRadius = 0.3f, bool sync = true)
    {
        if (mNavLayer == 0)
        {
            mNavLayer = NavigationSystem.GetLayer(pos);
        }
        Vector3[] path;
        if (!NavigationSystem.Nav_CalcLayerPath(Position, pos, mNavLayer, out path))
        {
            return;
        }
        mPath = path;
        mCurrentNodeIndex = 0;
        Speed = 0.0f;
        mDestRadius = destRadius;
    }

    public void LineMove(Vector3 pos, float destRadius = 0.3f, bool sync = true)
    {
        Vector3 destPos;
        if (!NavigationSystem.LineCast(Position, pos, mNavLayer, out destPos))
        {
            return;
        }
        mPath = new Vector3[1] { destPos };
        mCurrentNodeIndex = 0;
        Speed = 0.0f;
        mDestRadius = destRadius;
    }

    protected void UpdateVisibleInfo()
    {
        mScene.UpdateVisible(this);
    }

    public void OnEnterView(Character cha)
    {

    }

    public void OnExitView(Character cha)
    {

    }

    public uint mVisibleIndex = 0;
    public List<Character> mVisibleObjs = new List<Character>();
    public float mStartTime;

    protected uint mNavLayer = 0;
    
    // Search Path
    private Vector3[] mPath;
    private int mCurrentNodeIndex = 0;
    private float mDestRadius = 0.1f;
}
