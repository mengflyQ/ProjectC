using System;
using System.Collections.Generic;

namespace GameServer.LobbyServer
{
    public class BaseSystem
    {
        protected virtual void Tick()
        {

        }

        public static void StartTick(BaseSystem system)
        {
            if (mTickSystems.Contains(system))
            {
                return;
            }
            mTickSystems.Add(system);
        }

        public static void StopTick(BaseSystem system)
        {
            mTickSystems.Remove(system);
        }

        public static void DoTick()
        {
            for (int i = 0; i < mTickSystems.Count; ++i)
            {
                BaseSystem sys = mTickSystems[i];
                sys.Tick();
            }
        }
        static List<BaseSystem> mTickSystems = new List<BaseSystem>();
    }
}
