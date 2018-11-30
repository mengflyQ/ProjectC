using System;
using System.Collections.Generic;
using ZyGames.Framework.Common.Timing;

namespace GameServer.LobbyServer
{
    public class RoomManager : BaseSystem
    {
        public void Match(Player player)
        {
            Room room = null;
            bool full = false;

            if (mCandidateRooms.Count > 0)
            {
                room = mCandidateRooms[0];
                full = room.AddPlayer(player);
                if (full)
                {
                    mCandidateRooms.Remove(room);
                }
                return;
            }
            room = new Room(30.0f, 2);
            mRooms.Add(room);
            full = room.AddPlayer(player);
            if (!full)
            {
                mCandidateRooms.Add(room);
            }
        }

        public void RemoveRoom(Room room)
        {
            mRooms.Remove(room);
            mCandidateRooms.Remove(room);
        }

        protected override void Tick()
        {
            for (int i = mRooms.Count - 1; i >= 0; --i)
            {
                Room room = mRooms[i];
                if (room.IsVanish)
                {
                    int lastIndex = mRooms.Count - 1;
                    Room tmp = room;
                    mRooms[i] = mRooms[lastIndex];
                    mRooms[lastIndex] = room;
                    mRooms.RemoveAt(lastIndex);
                    mCandidateRooms.Remove(room);
                    continue;
                }
                room.Tick();
            }
        }

        static RoomManager mInstance = null;
        public static RoomManager Instance
        {
            get
            {
                if (mInstance == null)
                {
                    mInstance = new RoomManager();
                    BaseSystem.StartTick(mInstance);
                }
                return mInstance;
            }
        }
        public List<Room> mRooms = new List<Room>();
        List<Room> mCandidateRooms = new List<Room>();
    }
}