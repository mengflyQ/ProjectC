using System;
using MathLib;
using GameServer.RoomServer;

public class BTWait : BTAction
{
    public BTWait(Character self)
        : base(self)
    {

    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        Variable.LoadVariable(json["Random"], self, mRandomWaitTime, out mRandomWaitTime);
        VariableBool rwt = mRandomWaitTime as VariableBool;
        if (rwt.value)
        {
            Variable.LoadVariable(json["Min"], self, mMinWaitTime, out mMinWaitTime);
            Variable.LoadVariable(json["Max"], self, mMaxWaitTime, out mMaxWaitTime);
        }
        else
        {
            Variable.LoadVariable(json["Time"], self, mWaitTime, out mWaitTime);
        }
    }

    protected override void Enter()
    {
        base.Enter();

        VariableBool rwt = mRandomWaitTime as VariableBool;
        if (rwt.value)
        {
            VariableFloat min = mMinWaitTime as VariableFloat;
            VariableFloat max = mMaxWaitTime as VariableFloat;

            mTotalWaitTime = Mathf.RandRange(min.value, max.value);
        }
        else
        {
            VariableFloat w = mWaitTime as VariableFloat;

            mTotalWaitTime = w.value;
        }
        mStartTime = Time.ElapsedSeconds;
    }

    protected override BTStatus Update()
    {
        float time = Time.ElapsedSeconds - mStartTime;

        if (time < mTotalWaitTime)
            return BTStatus.Success;
        return BTStatus.Running;
    }

    Variable mWaitTime;
    Variable mRandomWaitTime;
    Variable mMinWaitTime;
    Variable mMaxWaitTime;

    float mStartTime;
    float mTotalWaitTime;
}