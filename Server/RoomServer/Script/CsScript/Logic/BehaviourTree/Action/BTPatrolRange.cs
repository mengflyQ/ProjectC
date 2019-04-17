using System;
using MathLib;
using GameServer.RoomServer;

public enum BTPatrolPhase
{
    Patrol,
    PatrolInterval,
}

public class BTPatrolRange : BTAction
{
    public BTPatrolRange(Character self)
        : base(self)
    {

    }

    protected override void Enter()
    {
        mPhase = BTPatrolPhase.PatrolInterval;
        mPath = null;
        mPathIndex = 0;
        mIntervalTime = 0.0f;
    }

    public override void Load(LitJson.JsonData json)
    {
        base.Load(json);

        LitJson.JsonData jsonRange = json["Range"];
        Variable.LoadVariable(jsonRange, self, mRange, out mRange);
        Variable.LoadVariable(json["Center"], self, mCenter, out mCenter);
        mPatrolMinInterval = json["MinItv"].AsFloat;
        mPatrolMaxInterval = json["MaxItv"].AsFloat;
    }

    protected override BTStatus Update()
    {
        if (self.Type == CharacterType.Player)
        {
            return BTStatus.Failure;
        }
        if (mRange.type != VariableType.Float)
        {
            return BTStatus.Failure;
        }

        if (BTPatrolPhase.PatrolInterval == mPhase)
        {
            mIntervalTime -= Time.DeltaTime;
            if (mIntervalTime > 0.0f)
                return BTStatus.Running;

            VariableVector3 p = mCenter as VariableVector3;
            Vector3 targetPos = p.value;
            Vector3 dir = new Vector3(Mathf.RandRange(-1.0f, 1.0f), 0.0f, Mathf.RandRange(-1.0f, 1.0f));
            dir.Normalize();
            VariableFloat varR = mRange as VariableFloat;
            float dist = Mathf.RandRange(0.0f, 1.0f) * varR.value;
            targetPos += (dist * dir);

            Vector3 hitPos = Vector3.zero;
            if (NavigationSystem.LineCast(self.Position, targetPos, self.mNavLayer, out hitPos))
            {
                targetPos = hitPos;
            }
            float h = 0.0f;
            if (NavigationSystem.GetLayerHeight(targetPos, self.mNavLayer, out h))
            {
                targetPos.y = h;
            }
            mPath = new Vector3[1];
            mPath[0] = targetPos;
            mPathIndex = 0;

            mPhase = BTPatrolPhase.Patrol;
        }
        else if (BTPatrolPhase.Patrol == mPhase)
        {
            if (mPath == null || mPathIndex >= mPath.Length)
            {
                mIntervalTime = Mathf.RandRange(mPatrolMinInterval, mPatrolMaxInterval);
                mPhase = BTPatrolPhase.PatrolInterval;
                return BTStatus.Running;
            }
            Vector3 targetPos = mPath[mPathIndex];
            if (!self.IsSearchMoving())
            {
                self.SearchMove(targetPos);
            }
            float dist = (targetPos - self.Position).Length();
            if (dist <= 0.3f)
            {
                ++mPathIndex;
            }
        }

        return BTStatus.Running;
    }

    Variable mRange;
    Variable mCenter;
    float mPatrolMinInterval;
    float mPatrolMaxInterval;

    BTPatrolPhase mPhase;
    Vector3[] mPath = null;
    int mPathIndex = 0;
    float mIntervalTime;
}