using UnityEngine;
using System;
using System.Collections.Generic;

public class MainPlayerRecord
{
    public MainPlayerRecord(Player player)
    {
        mMainPlayer = player;
    }

    public void LogicTick()
    {
        UpdateDirty();

        if (mDirty)
        {
            ReqPlayerMove req = new ReqPlayerMove();
            req.speed = mMainPlayer.MoveSpeed;
            req.direction = Vector3Packat.FromVector3(mMainPlayer.Direction);
            req.position = Vector3Packat.FromVector3(mMainPlayer.Position);
            req.timestamp = Time.realtimeSinceStartup;
            req.timespan = Time.realtimeSinceStartup - mLastTimeStamp;
            NetWork.SendPacket<ReqPlayerMove>(CTS.CTS_PlayerMove, req, null);

            mLastTimeStamp = Time.realtimeSinceStartup;

            ClearDirty();
        }
    }

    void ClearDirty()
    {
        mLastDirection = mMainPlayer.Direction;
        mLastSpeed = mMainPlayer.MoveSpeed;
        mDirty = false;
    }

    void UpdateDirty()
    {
        if (mMainPlayer.Direction != mLastDirection)
        {
            mDirty = true;
        }
        if (mMainPlayer.MoveSpeed != mLastSpeed)
        {
            mDirty = true;
        }
    }

    Player mMainPlayer = null;

    bool mDirty = false;

    Vector3 mLastDirection = Vector3.zero;
    float mLastSpeed = 0.0f;
    float mLastTimeStamp = 0.0f;
}