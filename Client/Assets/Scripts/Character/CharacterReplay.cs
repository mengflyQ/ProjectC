using UnityEngine;
using System;
using System.Collections.Generic;

public class CharactorReplay
{
    public CharactorReplay(Character cha)
    {
        mCha = cha;
    }

    public void AddMoveData(ReqPlayerMove moveData)
    {
        if (mHeadIndex == mCurrentIndex)
        {
            Debug.LogError("超出最大可接受移动数据");
            return;
        }
        mMoveDatas[mHeadIndex - 1] = moveData;
        mHeadIndex = (mHeadIndex) % MAX_CMD_LEN + 1;
    }

    public void LogicTick()
    {
        int nextIndex = (mCurrentIndex + 1) % MAX_CMD_LEN;
        if (nextIndex == mHeadIndex)
        {
            return;
        }

        ReqPlayerMove moveData = mMoveDatas[mCurrentIndex];
        if (moveData == null)
        {
            Debug.LogError("同步队列出错");
            mCurrentIndex = nextIndex;
            return;
        }
        float span = Time.realtimeSinceStartup - mLastTimeStamp;
        if (span >= moveData.timespan)
        {
            mCha.MoveSpeed = moveData.speed;
            mCha.Direction = moveData.direction.ToVector3();
            mCha.Position = moveData.position.ToVector3();
            mMoveDatas[mCurrentIndex] = null;
            mCurrentIndex = nextIndex;
            mLastTimeStamp = moveData.timestamp;
        }
    }

    Character mCha;
    ReqPlayerMove[] mMoveDatas = new ReqPlayerMove[MAX_CMD_LEN];
    int mHeadIndex = 1;
    int mCurrentIndex = 0;
    float mLastTimeStamp = 0.0f;

    private const int MAX_CMD_LEN = 50;
}