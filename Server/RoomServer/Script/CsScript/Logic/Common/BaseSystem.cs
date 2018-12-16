using System;
using System.Collections.Generic;

namespace GameServer.RoomServer
{
    public class BaseSystem
    {
        public BaseSystem()
        {
            Initialize();
        }

        public virtual void Initialize()
        {

        }

        protected virtual void Tick()
        {

        }

        public static void StartTick(BaseSystem system)
        {
            if (system == null || mTickSystems.Contains(system))
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
                if (sys == null)
                    continue;
                sys.Tick();
            }
        }
        static List<BaseSystem> mTickSystems = new List<BaseSystem>();
    }
}
