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
            room = new Room(30.0f, 1);
            mRooms.Add(room);
            mRooms.Sort(CompareRoom);
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

        public int GenRoomID()
        {
            int cid = 0;
            for (int i = 0; i < mRooms.Count; ++i)
            {
                Room r = mRooms[i];
                if (cid != r.ID)
                {
                    return cid;
                }
                else
                {
                    ++cid;
                }
            }
            return -1;
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
                    mRooms.Sort(CompareRoom);
                    continue;
                }
                room.Tick();
            }
        }

        int CompareRoom(Room r1, Room r2)
        {
            return r1.ID.CompareTo(r2.ID);
        }

        Room BinarySearchRoom(int low, int high, int id)
        {
            Room highRoom = mRooms[high];
            Room lowRoom = mRooms[low];

            int mid = (low + high) / 2;

            if (lowRoom.ID <= highRoom.ID)
            {
                Room midRoom = mRooms[mid];
                if (midRoom.ID == id)
                    return midRoom;
                else if (midRoom.ID > id)
                    return BinarySearchRoom(low, mid - 1, id);
                else
                    return BinarySearchRoom(mid + 1, high, id);
            }
            return null;
        }

        public Room FindRoom(int id)
        {
            if (mRooms == null || mRooms.Count <= 0)
                return null;

            return BinarySearchRoom(0, mRooms.Count - 1, id);
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