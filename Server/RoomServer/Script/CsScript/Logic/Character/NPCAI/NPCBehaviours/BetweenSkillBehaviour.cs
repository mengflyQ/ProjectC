using System;
using System.Collections.Generic;
using MathLib;
using GameServer.RoomServer;

namespace NPCFramework
{
    public class BetweenSkillBehaviour : BaseBehaviour
    {
        public override void Enter()
        {
            mDuration = Mathf.RandRange(mNPC.mNpcAI.skillMinInterval, mNPC.mNpcAI.skillMaxInterval);
            mStartTime = Time.ElapsedSeconds;

            mContext.DoSearchTarget();
        }

        public override void LogicTick()
        {
            if (mContext.CanOffFight())
            {
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.OffFight, mContext);
                return;
            }
            float time = Time.ElapsedSeconds - mStartTime;
            if (time > mDuration)
            {
                Character target = mNPC.GetTarget();
                if (target == null)
                {
                    mContext.mSkillIDs.Clear();
                    mContext.mCurkillAI = null;
                    mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.OffFight, mContext);
                }
                else
                {
                    if (mContext.mSkillIDs.Count == 0)
                    {
                        mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.IntoFight, mContext);
                    }
                    else
                    {
                        mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.UseSkill, mContext);
                    }
                }
            }
        }

        public float mDuration;
        public float mStartTime;
    }
}