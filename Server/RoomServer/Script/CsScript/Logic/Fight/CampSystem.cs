using System;
using System.Collections.Generic;

public static class CampSystem
{
    public static bool IsEnemy(Character cha1, Character cha2)
    {
        int campID1 = cha1.mChaList.campID;
        int campID2 = cha2.mChaList.campID;

        excel_cha_camp_list camp1 = excel_cha_camp_list.Find(campID1);
        excel_cha_camp_list camp2 = excel_cha_camp_list.Find(campID2);
        if (camp1 == null || camp2 == null)
            return false;

        for (int i = 0; i < camp1.enemyCamps.Length; ++i)
        {
            int enemyCamp1 = camp1.enemyCamps[i];
            if (enemyCamp1 == camp2.id)
                return true;
        }

        for (int i = 0; i < camp2.enemyCamps.Length; ++i)
        {
            int enemyCamp2 = camp2.enemyCamps[i];
            if (enemyCamp2 == camp1.id)
                return true;
        }

        return false;
    }

    public static bool IsFriend(Character cha1, Character cha2)
    {
        int campID1 = cha1.mChaList.campID;
        int campID2 = cha2.mChaList.campID;

        excel_cha_camp_list camp1 = excel_cha_camp_list.Find(campID1);
        excel_cha_camp_list camp2 = excel_cha_camp_list.Find(campID2);
        if (camp1 == null || camp2 == null)
            return false;

        for (int i = 0; i < camp1.friendCamps.Length; ++i)
        {
            int enemyCamp1 = camp1.friendCamps[i];
            if (enemyCamp1 == camp2.id)
                return true;
        }

        for (int i = 0; i < camp2.friendCamps.Length; ++i)
        {
            int enemyCamp2 = camp2.friendCamps[i];
            if (enemyCamp2 == camp1.id)
                return true;
        }

        return false;
    }
}