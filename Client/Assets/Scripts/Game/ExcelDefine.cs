using System;
using UnityEngine;

public class excel_scn_list : ExcelBase<excel_scn_list>
{
	public int id;
	public string name;
}

public class excel_cha_list : ExcelBase<excel_cha_list>
{
	public int id;
	public string name;
	public string path;
	public float[] scale;
}