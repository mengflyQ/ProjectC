using System;
using System.Collections.Generic;

public class GIDManger
{
    public int GetGID()
    {
        int id = 1;
        if (gids.Count == 0)
        {
            gids.Add(id);
            return id;
        }
        for (int i = 0; i < gids.Count + 1; ++i)
        {
            if (i >= gids.Count)
            {
                gids.Add(id);
                gids.Sort(CompareGID);
                return id;
            }
            int gid = gids[i];
            if (id == gid)
            {
                ++id;
                continue;
            }
            gids.Add(gid);
            gids.Sort(CompareGID);
            return gid;
        }
        return 0;
    }

    public void DelGID(int gid)
    {
        int index = FindGIDIndex(gid, 0, gids.Count - 1);
        if (index == -1)
            return;
        gids.RemoveAt(index);
    }

    int FindGIDIndex(int gid, int lowIndex, int highIndex)
    {
        int gidLow = gids[lowIndex];
        int gidHigh = gids[highIndex];

        int midIndex = (lowIndex + highIndex) / 2;

        if (midIndex == lowIndex && midIndex != highIndex)
        {
            if (gidHigh == gid)
                return highIndex;
            else if (gidLow == gid)
                return lowIndex;
            return -1;
        }
        if (gidLow <= gidHigh)
        {
            int gidMid = gids[midIndex];
            if (gidMid == gid)
                return midIndex;
            else if (gidMid > gid)
                return FindGIDIndex(gid, lowIndex, midIndex - 1);
            else
                return FindGIDIndex(gid, midIndex + 1, highIndex);
        }
        return -1;
    }

    int CompareGID(int id1, int id2)
    {
        return id1.CompareTo(id2);
    }

    static GIDManger mInstance = null;
    public static GIDManger Instance
    {
        get
        {
            if (mInstance == null)
                mInstance = new GIDManger();
            return mInstance;
        }
    }

    List<int> gids = new List<int>();
}