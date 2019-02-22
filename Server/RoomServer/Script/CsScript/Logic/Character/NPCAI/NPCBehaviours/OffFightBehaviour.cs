using System;
using System.Collections.Generic;
using MathLib;
using GameServer.RoomServer;

namespace NPCFramework
{
    public class OffFightBehaviour : BaseBehaviour
    {
        public override void Enter()
        {
            int hpMax = mNPC.GetAtb(AtbType.MaxHP);
            mNPC.SetAtb(AtbType.HP, hpMax);

            mNPC.SetCannotFlag(CannotFlag.CannotHurtByPhysic, OptType.AI, true);
            mNPC.SetCannotFlag(CannotFlag.CannotHurtByMagic, OptType.AI, true);

            mNPC.SearchMove(mContext.mOrigPosition);

            mNPC.SetTarget(null);

            HateSystem.Instance.Clear(mNPC);
        }

        public override void Exit()
        {
            mNPC.SetCannotFlag(CannotFlag.CannotHurtByPhysic, OptType.AI, false);
            mNPC.SetCannotFlag(CannotFlag.CannotHurtByMagic, OptType.AI, false);

            mNPC.Direction = mContext.mOrigDirection;
        }

        public override void LogicTick()
        {
            if (!mNPC.IsSearchMoving())
            {
                mNPC.SearchMove(mContext.mOrigPosition);
            }
            Vector3 dir = mContext.mOrigPosition - mNPC.Position;
            if (dir.Length() <= 0.5f)
            {
                mContext.mOrigDirection = Vector3.forward;
                mContext.mOrigPosition = Vector3.zero;
                mNPC.mBehaviourMachine.SetBehaviour(BehaviourType.Normal, mContext);
            }
        }
    }
}
