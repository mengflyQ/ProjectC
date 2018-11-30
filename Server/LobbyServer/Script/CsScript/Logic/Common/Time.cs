using System;
using System.Collections.Generic;
using System.Diagnostics;
using ZyGames.Framework.Common.Timing;

namespace GameServer.LobbyServer
{
    public static class Time
    {
        public static void Start()
        {
            mStopWatch = new Stopwatch();
            mTimer = new SyncTimer(Tick, 0, 33);

            mTimer.Start();
            mStopWatch.Start();
            DeltaTime = 0.0f;
            mLastTime = mStopWatch.ElapsedMilliseconds;
        }

        public static void Stop()
        {
            mStopWatch.Stop();
            mTimer.Stop();
        }

        static void Tick(object state)
        {
            long curTime = mStopWatch.ElapsedMilliseconds;
            DeltaTime = (float)(curTime - mLastTime) * 0.001f;
            mLastTime = curTime;

            BaseSystem.DoTick();
        }

        public static float DeltaTime
        {
            private set;
            get;
        }

        public static float ElapsedSeconds
        {
            get
            {
                return (float)mStopWatch.ElapsedMilliseconds * 0.001f;
            }
        }

        static Stopwatch mStopWatch = null;
        static SyncTimer mTimer = null;
        static long mLastTime;
    }
}
