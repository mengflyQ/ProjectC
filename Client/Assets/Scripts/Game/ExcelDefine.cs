using System;
using UnityEngine;

public class excel_scn_list : ExcelBase<excel_scn_list>
{
	public int id;
	public string name;
    public int temp;
}

public class excel_cha_list : ExcelBase<excel_cha_list>
{
	public int id;
	public string name;
	public string path;
	public float[] scale;
}

public class excel_anim_list : ExcelBase<excel_anim_list>
{
    public int id;
    public string name;
}