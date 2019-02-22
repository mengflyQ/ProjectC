using System;
using System.Collections.Generic;
using MathLib;
using GameServer.RoomServer;

namespace NPCFramework
{
    public class IntoFightBehaviour : BaseBehaviour
    {
        public override void LogicTick()
        {
            if (mContext.CanOffFight())
            {
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.OffFight, mContext);
                return;
            }
            Character target = mNPC.GetTarget();
            if (target == null)
            {
                mContext.DoSearchTarget();
            }
            if (target != null)
            {
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.UseSkill, mContext);
            }
        }

        public override void Exit()
        {
            mContext.mCurkillAI = null;
            mContext.mSkillIDs.Clear();
        }
    }
}
