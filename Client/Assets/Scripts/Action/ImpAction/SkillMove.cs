using System;
using UnityEngine;
using System.Collections.Generic;

public enum SkillMoveDataType
{
    [EnumDescription("朝目标位置匀速直线运动1")]
    MoveType1,
    [EnumDescription("朝目标位置匀速直线运动2")]
    MoveType2,
}

public class SkillMoveBaseData
{
    public SkillMoveDataType mMoveDataType;
}

public class SkillMoveData1 : SkillMoveBaseData
{
    public Vector3 mDestPos;
    public Vector3 mSrcPos;
    public float mTime;
}

public class SkillMoveData2 : SkillMoveBaseData
{
    public Vector3 mSrcPos;
    public Vector3 mDir;
    public float mSpeed;
    public float mTime;
}

public class SkillMove : IAction
{
    public SkillMove() : base(ChaActionType.SkillMove)
    {

    }

    public void Init1(Character owner, Vector3 destPos, float time)
    {
        mOwner = owner;
        SkillMoveData1 moveData1 = new SkillMoveData1();
        mMoveData = moveData1;

        moveData1.mMoveDataType = SkillMoveDataType.MoveType1;
        moveData1.mSrcPos = owner.Position;
        moveData1.mDestPos = destPos;
        moveData1.mTime = time;
    }

    public void Init2(Character owner, Vector3 dir, float speed, float time)
    {
        mOwner = owner;
        SkillMoveData2 moveData2 = new SkillMoveData2();
        mMoveData = moveData2;

        moveData2.mMoveDataType = SkillMoveDataType.MoveType2;
        moveData2.mSrcPos = owner.Position;
        moveData2.mDir = dir;
        moveData2.mDir.y = 0.0f;
        moveData2.mDir.Normalize();
        moveData2.mSpeed = speed;
        moveData2.mTime = time;
    }

    public override void Enter()
    {
        mStartTime = Time.realtimeSinceStartup;
    }

    public override bool LogicTick()
    {
        if (mMoveData == null)
            return false;
        float time = Time.realtimeSinceStartup - mStartTime;
        if (mMoveData.mMoveDataType == SkillMoveDataType.MoveType1)
        {
            SkillMoveData1 moveData = mMoveData as SkillMoveData1;

            float t = time / moveData.mTime;
            Vector3 dir = moveData.mDestPos - moveData.mSrcPos;
            Vector3 curPos = moveData.mSrcPos + dir * t;
            Vector3 curDir = dir.normalized;
            mOwner.Position = curPos;
            mOwner.Direction = curDir;

            if (time >= moveData.mTime)
                return false;
        }
        if (mMoveData.mMoveDataType == SkillMoveDataType.MoveType2)
        {
            SkillMoveData2 moveData = mMoveData as SkillMoveData2;

            Vector3 curPos = moveData.mSrcPos + moveData.mTime * moveData.mSpeed * moveData.mDir;
            mOwner.Position = curPos;
            mOwner.Direction = moveData.mDir;

            if (time >= moveData.mTime)
                return false;
        }

        Vector3 pos = mOwner.Position;
        pos.y += 10.0f;
        float h = 0.0f;
        if (NavigationSystem.GetLayerHeight(pos, mOwner.NavLayer, out h))
        {
            pos.y = h;
            mOwner.Position = pos;
        }

        return true;
    }

    public override void Exit()
    {
        if (mMoveData.mMoveDataType == SkillMoveDataType.MoveType1)
        {
            SkillMoveData1 moveData = mMoveData as SkillMoveData1;

            mOwner.Position = moveData.mDestPos;
        }

        Vector3 pos = mOwner.Position;
        pos.y += 10.0f;
        float h = 0.0f;
        if (NavigationSystem.GetLayerHeight(pos, mOwner.NavLayer, out h))
        {
            pos.y = h;
            mOwner.Position = pos;
        }
    }

    SkillMoveBaseData mMoveData = null;
    Character mOwner;
    float mStartTime;
}