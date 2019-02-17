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
            float time = Time.realtimeSinceStartup - GameController.mClientStartTime;

            ReqPlayerMove req = new ReqPlayerMove();
            req.gid = mMainPlayer.gid;
            req.speed = mMainPlayer.MoveSpeed;
            req.direction = Vector3Packat.FromVector3(mMainPlayer.Direction);
            req.position = Vector3Packat.FromVector3(mMainPlayer.Position);
            req.control = mMainPlayer.IsControl;
            req.timestamp = time;
            req.timespan = time - mLastTimeStamp;
            NetWork.SendPacket<ReqPlayerMove>(CTS.CTS_PlayerMove, req, null);

            mLastTimeStamp = time;

            ClearDirty();
        }
    }

    void ClearDirty()
    {
        mLastDirection = mMainPlayer.Direction;
        mLastSpeed = mMainPlayer.MoveSpeed;
        mLastControl = mMainPlayer.IsControl;
        mDirty = false;
    }

    void UpdateDirty()
    {
        if (mMainPlayer.IsControl)
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
        if (mMainPlayer.IsControl != mLastControl)
        {
            mDirty = true;
        }
    }

    Player mMainPlayer = null;

    bool mDirty = false;

    Vector3 mLastDirection = Vector3.zero;
    float mLastSpeed = 0.0f;
    float mLastTimeStamp = 0.0f;
    bool mLastControl = false;
}