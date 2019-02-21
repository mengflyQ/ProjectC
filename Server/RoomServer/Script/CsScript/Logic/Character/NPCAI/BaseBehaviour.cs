using System;
using System.Collections.Generic;
using GameServer.RoomServer;
using MathLib;

namespace NPCFramework
{
    public class BaseBehaviour
    {
        public virtual void Enter() { }

        public virtual void LogicTick() { }

        public virtual void Exit() { }

        public virtual void DoEvent(BehaviourEvent e, params object[] objs) { }

        public NPC mNPC = null;

        public BehaviourContext mContext = null;
    }
}