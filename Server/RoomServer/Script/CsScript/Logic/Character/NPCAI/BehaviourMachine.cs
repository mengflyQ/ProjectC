using System;
using System.Collections.Generic;
using MathLib;
using GameServer.RoomServer;

namespace NPCFramework
{
    public enum BehaviourType
    {
        Normal,
        IntoFight,
        UseSkill,
        BetweenSkill,
        OffFight,
    }

    public enum BehaviourEvent
    {

    }

    public class BehaviourMachine
    {
        public void Initialize()
        {
            Register(BehaviourType.Normal, new NormalBehaviour());
            Register(BehaviourType.IntoFight, new IntoFightBehaviour());
            Register(BehaviourType.UseSkill, new UseSkillBehaviour());
            Register(BehaviourType.BetweenSkill, new BetweenSkillBehaviour());
            Register(BehaviourType.OffFight, new OffFightBehaviour());

            SetBehaviour(BehaviourType.Normal, new BehaviourContext(mNPC));
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

        public void SetBehaviour(BehaviourType type, BehaviourContext context)
        {
            BaseBehaviour b = null;
            if (!mBehaviours.TryGetValue(type, out b))
            {
                return;
            }
            BaseBehaviour lastBehaviour = mCurrentBehaviour;
            mCurrentBehaviour = b;
            mCurrentBehaviour.mContext = context;
            if (lastBehaviour != null)
            {
                lastBehaviour.Exit();
            }
            if (mCurrentBehaviour != null)
            {
                mCurrentBehaviour.Enter();
            }
        }

        public void DoEvent(BehaviourEvent e, params object[] objs)
        {
            if (mCurrentBehaviour == null)
                return;
            mCurrentBehaviour.DoEvent(e, objs);
        }

        Dictionary<BehaviourType, BaseBehaviour> mBehaviours = new Dictionary<BehaviourType, BaseBehaviour>();
        BaseBehaviour mCurrentBehaviour = null;
        public NPC mNPC = null;
    }
}