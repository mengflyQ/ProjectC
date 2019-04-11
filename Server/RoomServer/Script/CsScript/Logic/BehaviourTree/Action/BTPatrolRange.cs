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
        mPatrolMinInterval = json["MinItv"].AsFloat;
        mPatrolMaxInterval = json["MaxItv"].AsFloat;
    }

    protected override BTStatus Update()
    {
        if (self.Type == CharacterType.Player)
        {
            return BTStatus.Failure;
        }
        NPC npc = self as NPC;
        if (npc.mRefreshList == null)
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

            if (npc.mRefreshList.birthpoint.Length <= 0)
                return BTStatus.Failure;
            string birthpoint = npc.mRefreshList.birthpoint[0];
            MarkPoint markPoint = RefreshSystem.Instance.GetMarkPoint(npc.mScene.ScnID, birthpoint);
            if (markPoint == null)
                return BTStatus.Failure;
            Vector3 targetPos = markPoint.position;
            Vector3 dir = new Vector3(Mathf.RandRange(-1.0f, 1.0f), 0.0f, Mathf.RandRange(-1.0f, 1.0f));
            dir.Normalize();
            VariableFloat varR = mRange as VariableFloat;
            float dist = Mathf.RandRange(0.0f, 1.0f) * varR.value;
            targetPos += (dist * dir);

            Vector3 hitPos = Vector3.zero;
            if (NavigationSystem.LineCast(npc.Position, targetPos, npc.mNavLayer, out hitPos))
            {
                targetPos = hitPos;
            }
            float h = 0.0f;
            if (NavigationSystem.GetLayerHeight(targetPos, npc.mNavLayer, out h))
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
            if (!npc.IsSearchMoving())
            {
                npc.SearchMove(targetPos);
            }
            float dist = (targetPos - npc.Position).Length();
            if (dist <= 0.3f)
            {
                ++mPathIndex;
            }
        }

        return BTStatus.Running;
    }

    Variable mRange;
    float mPatrolMinInterval;
    float mPatrolMaxInterval;

    BTPatrolPhase mPhase;
    Vector3[] mPath = null;
    int mPathIndex = 0;
    float mIntervalTime;
}