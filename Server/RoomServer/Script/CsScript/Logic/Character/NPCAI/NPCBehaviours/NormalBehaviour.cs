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
            DoSearchTarget();
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

        void DoSearchTarget()
        {
            Scene scn = mNPC.mScene;
            if (scn == null)
                return;

            SearchTargetType searchTargetType = (SearchTargetType)mNPC.GetFlagMemory(FlagMemory.SearchTargetType);
            SearchTargetCondition searchTargetCondition = (SearchTargetCondition)mNPC.GetFlagMemory(FlagMemory.SearchTargetCondition);
            excel_npc_ai npcAI = excel_npc_ai.Find(mNPC.mRefreshList.npcAI);
            if (npcAI == null)
            {
                return;
            }

            List<Character> selected = new List<Character>();

            float minDist = float.MaxValue;
            Character minDistCha = null;
            for (int i = 0; i < scn.GetCharacterCount(); ++i)
            {
                Character c = scn.GetCharacterByIndex(i);

                if (searchTargetType == SearchTargetType.Passive)
                {
                    continue;
                }
                if (searchTargetType == SearchTargetType.ActiveEnemy
                    && !CampSystem.IsEnemy(c, mNPC))
                {
                    continue;
                }
                if (searchTargetType == SearchTargetType.ActiveFriend
                    && !CampSystem.IsFriend(c, mNPC))
                {
                    continue;
                }

                Vector3 d = c.Position - mNPC.Position;
                d.y = 0.0f;
                float dist = d.Length();
                if (minDist > dist)
                {
                    minDist = dist;
                    minDistCha = c;
                }

                if (dist > npcAI.searchTargetDist)
                    continue;
                selected.Add(c);
            }

            Character target = SearchTarget(searchTargetCondition, selected, minDistCha);

            mNPC.SetTarget(target);

            if (target != null)
            {
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.UseSkill, mContext);
            }
        }

        Character SearchTarget(SearchTargetCondition condition, List<Character> characters, Character nearest)
        {
            if (SearchTargetCondition.Random == condition)
            {
                int index = Mathf.RandRange(0, characters.Count - 1);
                return characters[index];
            }

            float maxFloatValue = float.MinValue;
            int maxIntValue = int.MinValue;
            int minIntValue = int.MaxValue;
            Character target = nearest;
            for (int i = 0; i < characters.Count; ++i)
            {
                Character c = characters[i];
                switch (condition)
                {
                    case SearchTargetCondition.Farest:
                        {
                            Vector3 d = c.Position - mNPC.Position;
                            d.y = 0.0f;
                            float dist = d.Length();
                            if (dist > maxFloatValue)
                            {
                                maxFloatValue = dist;
                                target = c;
                            }
                        }
                        break;
                    case SearchTargetCondition.HateHighest:
                        {
                            int hate = HateSystem.Instance.GetHate(c, mNPC);
                            if (hate == 0)
                            {
                                break;
                            }
                            if (maxIntValue < hate)
                            {
                                maxIntValue = hate;
                                target = c;
                            }
                        }
                        break;
                    case SearchTargetCondition.LessHP:
                        {
                            int hp = c.HP;
                            if (hp == 0)
                            {
                                break;
                            }
                            if (minIntValue > hp)
                            {
                                minIntValue = hp;
                                target = c;
                            }
                        }
                        break;
                    case SearchTargetCondition.MostHP:
                        {
                            int hp = c.HP;
                            if (hp == 0)
                            {
                                break;
                            }
                            if (maxIntValue < hp)
                            {
                                minIntValue = hp;
                                target = c;
                            }
                        }
                        break;
                }
            }
            return target;
        }

        NormalBehaviourPhase mPhase = NormalBehaviourPhase.PatrolInterval;
        float mIntervalTime;
        Vector3[] mPath = null;
        int mPathIndex = 0;
    }
}