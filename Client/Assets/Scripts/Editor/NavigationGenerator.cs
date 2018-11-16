using System;
using UnityEngine;
using UnityEditor;

public class NavMeshGenerator
{
	[MenuItem("Tools/场景/生成行走层")]
	public static void GeneratorNavMesh()
	{
		UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

        UnityEngine.AI.NavMeshTriangulation aMesh = UnityEngine.AI.NavMesh.CalculateTriangulation();

		string scnName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name;
		string directory = Application.dataPath + "/Scenes/" + scnName + "/NavigationModel";
		if (!System.IO.Directory.Exists(directory))
		{
			System.IO.Directory.CreateDirectory(directory);
		}
		string[] files = System.IO.Directory.GetFiles(directory);
		for (int i = 0; i < files.Length; ++i)
		{
			string file = files[i];
			if (System.IO.File.Exists(file))
			{
				System.IO.File.Delete(file);
			}
		}
		string[] folders = System.IO.Directory.GetDirectories(directory);
		for (int i = 0; i < folders.Length; ++i)
		{
			string folder = folders[i];
			if (System.IO.Directory.Exists(folder))
			{
				System.IO.Directory.Delete(folder);
			}
		}

		string fileName = directory + "/NavModel.obj";

		var sw = new System.IO.StreamWriter(fileName);
		sw.WriteLine("# Unity NavModel OBJ File");

		for (int i = 0; i < aMesh.vertices.Length; ++i)
		{
			var sb = new System.Text.StringBuilder("v ", 20);

			float x = aMesh.vertices[i].x;
			x = -x;
			sb.Append(x.ToString()).Append(' ').
			Append(aMesh.vertices[i].y.ToString()).Append(' ').
			Append(aMesh.vertices[i].z.ToString());
			sw.WriteLine(sb);
		}
		for (int i = 0; i < aMesh.vertices.Length; i++)
		{
			var sb = new System.Text.StringBuilder("vt ", 22);
			sb.Append((0.0f).ToString()).Append(' ').
			Append((0.0f).ToString());
			sw.WriteLine(sb);
		}
		for (int i = 0; i < aMesh.indices.Length; i += 3)
		{
			var sb = new System.Text.StringBuilder("f ", 43);
			sb.Append(aMesh.indices[i+2]+1).Append(' ').
			Append(aMesh.indices[i+1]+1).Append(' ').
			Append(aMesh.indices[i]+1);
			sw.WriteLine(sb);
		}

		sw.Close();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

	[MenuItem("Tools/场景/导出场景")]
	public static void ExportScene()
	{
		GameObject[] gos = new GameObject[3];
		GameObject terrain = GameObject.Find("Scene/_Terrain");
		GameObject wall = GameObject.Find("Scene/Wall");
		GameObject stairs = GameObject.Find("Scene/Stairs");
		gos[0] = terrain;
		gos[1] = wall;
		gos[2] = stairs;

		string scnName = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene ().name;
		FBXExporter.ExportFBX("", scnName, gos, true);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}