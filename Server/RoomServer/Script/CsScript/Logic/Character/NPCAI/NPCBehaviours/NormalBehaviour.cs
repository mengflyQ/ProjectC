using System;
using System.Collections.Generic;
using MathLib;
using GameServer.RoomServer;

namespace NPCFramework
{
    public enum SearchTargetType
    {
        Passive,
        ActiveEnemy,
        ActiveFriend
    }

    public enum SearchTargetCondition
    {
        Nearest,
        Farest,
        HateHighest,
        LessHP,
        MostHP,
        Random
    }

    public enum PatrolType
    {
        Static,
        Scope,
        PathLine
    }

    public class NormalBehaviour : BaseBehaviour
    {
        public enum NormalBehaviourPhase
        {
            Patrol,
            PatrolInterval,
        }

        public override void Enter()
        {
            mNPC.SetTarget(null);
        }

        public override void LogicTick()
        {
            DoPatrol();
            mContext.DoSearchTarget();

            Character target = mNPC.GetTarget();
            if (target != null)
            {
                if (!HateSystem.Instance.IsHate(mNPC, target))
                {
                    HateSystem.Instance.AddHate(mNPC, target, 1);
                }
                mContext.mOrigPosition = mNPC.Position;
                mContext.mOrigDirection = mNPC.Direction;
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.IntoFight, mContext);
            }
        }

        void DoPatrol()
        {
            if (mNPC.mRefreshList == null)
                return;
            int npcAIID = mNPC.mRefreshList.npcAI;
            excel_npc_ai npcAI = excel_npc_ai.Find(npcAIID);
            if (npcAI == null)
                return;
            if (NormalBehaviourPhase.PatrolInterval == mPhase)
            {
                mIntervalTime -= Time.DeltaTime;
                if (mIntervalTime > 0.0f)
                    return;
                PatrolType patrolType = (PatrolType)npcAI.patrolType;
                if (patrolType == PatrolType.Scope)
                {
                    if (mNPC.mRefreshList.birthpoint.Length <= 0)
                        return;
                    string birthpoint = mNPC.mRefreshList.birthpoint[0];
                    MarkPoint markPoint = RefreshSystem.Instance.GetMarkPoint(mNPC.mScene.ScnID, birthpoint);
                    if (markPoint == null)
                        return;
                    Vector3 targetPos = markPoint.position;
                    Vector3 dir = new Vector3(Mathf.RandRange(-1.0f, 1.0f), 0.0f, Mathf.RandRange(-1.0f, 1.0f));
                    dir.Normalize();
                    float dist = Mathf.RandRange(0.0f, 1.0f) * npcAI.patrolRadius;
                    targetPos += (dist * dir);

                    Vector3 hitPos = Vector3.zero;
                    if (NavigationSystem.LineCast(mNPC.Position, targetPos, mNPC.mNavLayer, out hitPos))
                    {
                        targetPos = hitPos;
                    }
                    float h = 0.0f;
                    if (NavigationSystem.GetLayerHeight(targetPos, mNPC.mNavLayer, out h))
                    {
                        targetPos.y = h;
                    }
                    mPath = new Vector3[1];
                    mPath[0] = targetPos;
                    mPathIndex = 0;

                    mPhase = NormalBehaviourPhase.Patrol;
                }
            }
            else if (NormalBehaviourPhase.Patrol == mPhase)
            {
                if (npcAI.patrolType == (int)PatrolType.Scope)
                {
                    if (mPath == null || mPathIndex >= mPath.Length)
                    {
                        mIntervalTime = Mathf.RandRange(npcAI.patrolMinInterval, npcAI.patrolMaxInterval);
                        mPhase = NormalBehaviourPhase.PatrolInterval;
                        return;
                    }
                    Vector3 targetPos = mPath[mPathIndex];
                    if (!mNPC.IsSearchMoving())
                    {
                        mNPC.SearchMove(targetPos);
                    }
                    float dist = (targetPos - mNPC.Position).Length();
                    if (dist <= 0.3f)
                    {
                        ++mPathIndex;
                    }
                }
            }
        }

        NormalBehaviourPhase mPhase = NormalBehaviourPhase.PatrolInterval;
        float mIntervalTime;
        Vector3[] mPath = null;
        int mPathIndex = 0;
    }
}