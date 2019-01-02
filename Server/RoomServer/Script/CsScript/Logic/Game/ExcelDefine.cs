using System;

public class excel_scn_list : ExcelBase<excel_scn_list>
{
    public string name;
    public int temp;
    public float[] min;
    public float[] max;
    public float viewDist;
}

public class excel_cha_class : ExcelBase<excel_cha_class>
{
    public string name;
    public int chaListID;
    public int[] skillIDs;
}

public class excel_cha_list : ExcelBase<excel_cha_list>
{
    public string name;
    public string path;
    public float[] scale;
    public float radius;
    public int campID;
}

public class excel_anim_list : ExcelBase<excel_anim_list>
{
    public string name;
}

public class excel_skill_list : ExcelBase<excel_skill_list>
{
    public string name;
    public int[] stages;
    public int[] hits;
    public int trait;
    public float maxDistance;
    public int targetType;
}

public class excel_skill_stage : ExcelBase<excel_skill_stage>
{
    public string name;
    public int[] events;
    public int trait;
    public int time;
    public int nextStageID;
}

public class excel_skill_event : ExcelBase<excel_skill_event>
{
    public string name;
    public int triggerType;
    public int triggerParam1;
    public int triggerParam2;
    public int eventType;
    public int evnetParam1;
    public int evnetParam2;
    public int evnetParam3;
    public int evnetParam4;
    public int evnetParam5;
    public int evnetParam6;
    public int evnetParam7;
    public int evnetParam8;
    public string evnetParam9;
    public int trait;
}

public class excel_skill_hit : ExcelBase<excel_skill_hit>
{
    public string name;
    public int hitType;
    public int hitData1;
    public int hitData2;
    public int hitData3;
    public int targetType;
    public int maxHitCount;
    public int targetMaxHitCnt;
}

public class excel_cha_camp_list : ExcelBase<excel_cha_camp_list>
{
    public string name;
    public int[] enemyCamps;
    public int[] friendCamps;
    public int teamFriend;
}