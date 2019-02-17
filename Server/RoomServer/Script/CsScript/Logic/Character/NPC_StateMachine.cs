using System;
using System.Collections.Generic;
using GameServer.RoomServer;
using MathLib;

public enum SearchTargetType
{
    Passive,
    Active
}

public enum SearchTargetCondition
{
    Nearest,
    HateHighest
}

public enum PatrolType
{
    Static,
    Scope,
    PathLine
}

namespace NPCFramework
{
    public enum BehaviourType
    {
        Normal,

    }

    public enum BehaviourEvent
    {

    }

    //////////////////////////////////////////////////////////////////////

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
                    mPath = new Vector3[1];
                    mPath[0] = targetPos;
                    mPathIndex = 0;

                    mPhase = NormalBehaviourPhase.Patrol;
                }
            }
            else if (NormalBehaviourPhase.Patrol == mPhase)
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

        void DoSearchTarget()
        {
            Scene scn = mNPC.mScene;
            if (scn == null)
                return;

            for (int i = 0; i < scn.GetCharacterCount(); ++i)
            {
                Character c = scn.GetCharacterByIndex(i);


            }
        }

        NormalBehaviourPhase mPhase = NormalBehaviourPhase.PatrolInterval;
        float mIntervalTime;
        Vector3[] mPath = null;
        int mPathIndex = 0;
    }

    //////////////////////////////////////////////////////////////////////

    public class BaseBehaviour
    {
        public virtual void Enter() { }

        public virtual void LogicTick() { }

        public virtual void Exit() { }

        public virtual void DoEvent(BehaviourEvent e) { }

        public NPC mNPC = null;
    }

    public class BehaviourMachine
    {
        public void Initialize()
        {
            Register(BehaviourType.Normal, new NormalBehaviour());

            SetBehaviour(BehaviourType.Normal);
        }

        void Register(BehaviourType type, BaseBehaviour behaviour)
        {
            if (mBehaviours.ContainsKey(type))
            {
                return;
            }
            behaviour.mNPC = mNPC;
            mBehaviours.Add(type, behaviour);
        }

        public void LogicTick()
        {
            if (mCurrentBehaviour != null)
            {
                mCurrentBehaviour.LogicTick();
            }
        }

        public void SetBehaviour(BehaviourType type)
        {
            BaseBehaviour b = null;
            if (!mBehaviours.TryGetValue(type, out b))
            {
                return;
            }
            BaseBehaviour lastBehaviour = mCurrentBehaviour;
            mCurrentBehaviour = b;
            if (lastBehaviour != null)
            {
                lastBehaviour.Exit();
            }
            if (mCurrentBehaviour != null)
            {
                mCurrentBehaviour.Enter();
            }
        }

        Dictionary<BehaviourType, BaseBehaviour> mBehaviours = new Dictionary<BehaviourType, BaseBehaviour>();
        BaseBehaviour mCurrentBehaviour = null;
        public NPC mNPC = null;
    }
}



public partial class NPC : Character
{
    void InitStateMachine()
    {
        mBehaviourMachine.mNPC = this;
        mBehaviourMachine.Initialize();
    }

    void UpdateBehaviour()
    {
        mBehaviourMachine.LogicTick();
    }

    NPCFramework.BehaviourMachine mBehaviourMachine = new NPCFramework.BehaviourMachine();
}