using System;
using System.Collections.Generic;
using MathLib;
using GameServer.RoomServer;

namespace NPCFramework
{
    public enum SkillAICondition
    {
        OnOffFight = 1,
        OnHitDown = 2,
        OnDead = 3,
        OnEnterFight = 4,
        OnBorn = 5,

        OnTargetState = 7,
        OnSelfState = 8,
        OnTargetHP = 9,
        OnSelfHP = 10,

        Default = 11
    }

    public enum SkillAISelectSkillType
    {
        SeqRand,
        OneOfSeqRand,
    }

    public struct RandSkillData
    {
        public int mRandVal;
        public int mSkillID;
    }

    public class BehaviourContext
    {
        public BehaviourContext(NPC c)
        {
            mSelf = c;
        }

        int TableRandom(List<RandSkillData> v)
        {
            int rand = Mathf.RandRange(0, 100);
            int sum = 0;
            for (int i = 0; i < v.Count; ++i)
            {
                RandSkillData rsd = v[i];
                sum += rsd.mRandVal;
                if (sum >= rand)
                {
                    return rsd.mSkillID;
                }
            }
            return -1;
        }

        public bool RandSkill(excel_skill_ai skillAI, out List<int> output)
        {
            output = new List<int>();
            if (skillAI == null)
                return false;
            int skillCount = skillAI.selectSkillIDs.Length;
            if (skillCount == 0 || skillCount % 2 != 0)
            {
                Debug.LogError("SkillAI({0}): CallSkill Number Is {1}", skillAI.id, skillCount);
                return false;
            }
            switch ((SkillAISelectSkillType)skillAI.selectType)
            {
                case SkillAISelectSkillType.SeqRand:
                    {
                        for (int i = 0; i < skillCount; i += 2)
                        {
                            int nRandVal = skillAI.selectSkillIDs[i];
                            int nSkillID = skillAI.selectSkillIDs[i + 1];

                            if (nRandVal >= 100 || (int)Mathf.RandRange(0.0f, 100.0f) <= nRandVal)
                            {
                                output.Add(nSkillID);
                            }
                        }
                    }
                    break;
                case SkillAISelectSkillType.OneOfSeqRand:
                    {
                        List<RandSkillData> randGroup = new List<RandSkillData>();
                        int curRand = 0;
                        for (int i = 0; i < skillCount; i += 2)
                        {
                            int nRandVal = skillAI.selectSkillIDs[i];
                            int nSkillID = skillAI.selectSkillIDs[i + 1];

                            if (curRand >= 100)
                            {
                                int skillID = TableRandom(randGroup);
                                if (skillID > 0)
                                {
                                    output.Add(skillID);
                                }
                                randGroup.Clear();
                                curRand = 0;
                            }
                            curRand += nRandVal;

                            RandSkillData rsd;
                            rsd.mRandVal = nRandVal;
                            rsd.mSkillID = nSkillID;
                            randGroup.Add(rsd);
                        }
                        int id = TableRandom(randGroup);
                        if (id > 0)
                        {
                            output.Add(id);
                        }
                        randGroup.Clear();
                        curRand = 0;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        public excel_skill_ai CalcSkillAI()
        {
            if (mSelf.mSkillAIs == null)
                return null;

            float minDist = float.MaxValue;
            int condID = -1;
            excel_skill_ai skillAI = null;
            for (int i = 0; i < mSelf.mSkillAIs.Length; ++i)
            {
                excel_skill_ai ai = mSelf.mSkillAIs[i];
                if (ai == null)
                    continue;
                if (condID > 0 && ai.conditionType != condID)
                {
                    break;
                }
                bool rst = CheckSkillAI(ai, ref minDist);
                if (rst)
                {
                    skillAI = ai;
                    condID = ai.conditionType;
                }
            }
            return skillAI;
        }

        bool CheckSkillAI(excel_skill_ai ai, ref float dist)
        {
            switch ((SkillAICondition)ai.conditionType)
            {
                case SkillAICondition.OnSelfState:
                    {
                        if (ai.conditionDatas.Length < 1)
                        {
                            Debug.LogError("怪物AI表ID为{0}的数据中，条件数据数量为空;", ai.id);
                            return false;
                        }
                        bool findState = false;
                        StateMgr stateMgr = mSelf.mStateMgr;
                        for (int i = 0; i < ai.conditionDatas.Length; ++i)
                        {
                            int data = ai.conditionDatas[i];
                            if (stateMgr.HasState(data))
                            {
                                findState = true;
                                break;
                            }
                        }
                        if (!findState)
                            return false;
                    }
                    break;
                case SkillAICondition.OnTargetState:
                    {
                        if (ai.conditionDatas.Length < 1)
                        {
                            Debug.LogError("怪物AI表ID为{0}的数据中，条件数据数量为空;", ai.id);
                            return false;
                        }
                        Character t = mSelf.GetTarget();
                        if (t == null)
                            return false;

                        bool findState = false;
                        StateMgr stateMgr = t.mStateMgr;
                        for (int i = 0; i < ai.conditionDatas.Length; ++i)
                        {
                            int data = ai.conditionDatas[i];
                            if (stateMgr.HasState(data))
                            {
                                findState = true;
                                break;
                            }
                        }
                        if (!findState)
                            return false;
                    }
                    break;
                case SkillAICondition.OnSelfHP:
                    {
                        if (ai.conditionDatas.Length != 2)
                        {
                            Debug.LogError("怪物AI表ID为{0}的数据中，条件数据数量不为2;", ai.id);
                            return false;
                        }
                        float minPct = (float)ai.conditionDatas[0] * 0.01f;/*/ 100.0f*/;
                        float maxPct = (float)ai.conditionDatas[1] * 0.01f;
                        float hp = (float)mSelf.HP;
                        float hpMax = (float)mSelf.GetAtb(AtbType.MaxHP);
                        float rangeMax = hpMax * maxPct;
                        float rangeMin = hpMax * minPct;
                        if (hp < rangeMin || hp > rangeMax)
                        {
                            return false;
                        }
                    }
                    break;
                case SkillAICondition.OnTargetHP:
                    {
                        if (ai.conditionDatas.Length != 2)
                        {
                            Debug.LogError("怪物AI表ID为{0}的数据中，条件数据数量不为2;", ai.id);
                            return false;
                        }
                        Character t = mSelf.GetTarget();
                        if (t == null)
                            return false;
                        float minPct = (float)ai.conditionDatas[0] * 0.01f;/*/ 100.0f*/;
                        float maxPct = (float)ai.conditionDatas[1] * 0.01f;
                        float hp = (float)t.HP;
                        float hpMax = (float)t.GetAtb(AtbType.MaxHP);
                        float rangeMax = hpMax * maxPct;
                        float rangeMin = hpMax * minPct;
                        if (hp < rangeMin || hp > rangeMax)
                        {
                            return false;
                        }
                    }
                    break;
                case SkillAICondition.Default:
                    return true;
                default:
                    return false;
            }

            Character target = mSelf.GetTarget();
            if (target == null)
                return false;
            float distance = SkillUtility.GetDistance(mSelf, target, DistanceCalcType.OuterAB);
            if (distance <= ai.distance && ai.distance < dist)
            {
                dist = ai.distance;
                return true;
            }
            if (mCurkillAI == ai && mSelf.GetSkill() != null)
            {
                return true;
            }
            return false;
        }

        public excel_skill_ai mCurkillAI = null;

        public List<int> mSkillIDs = new List<int>();

        private NPC mSelf = null;
    }
}