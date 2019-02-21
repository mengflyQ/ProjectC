using System;
using System.Collections.Generic;
using GameServer.RoomServer;
using MathLib;

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

    public NPCFramework.BehaviourMachine mBehaviourMachine = new NPCFramework.BehaviourMachine();
}