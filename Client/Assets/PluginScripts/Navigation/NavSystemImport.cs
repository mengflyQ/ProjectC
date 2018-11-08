using UnityEngine;
using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct NAV_VEC3
{
	public float x, y, z;

	public NAV_VEC3(Vector3 v)
	{
		x = v.x;
		y = v.y;
		z = v.z;
	}

	public Vector3 ToVector3()
	{
		Vector3 v = new Vector3(x, y, z);
		return v;
	}
}

[StructLayout(LayoutKind.Sequential)]
public struct NAV_VEC2 { public float x, z; }

public sealed class NavSystemImport
{
	#if UNITY_IOS && !UNITY_EDITOR
	public const string dllName = "__Internal";
	#else
	public const string dllName = "NavSystem";
	#endif

	#region Labrary_Function

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_Create(string path, string scnName);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_CreateFromMemory(byte[] data, string scnName);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_LoadSketchSceneFromFile(string path);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_LoadSketchSceneFromMemory(byte[] data);

	[DllImport(NavSystemImport.dllName)]
	public static extern void Nav_ReleaseLayer(uint layer);

	[DllImport(NavSystemImport.dllName)]
	public static extern void Nav_ReleaseLayerByIndex(uint index);

	[DllImport(NavSystemImport.dllName)]
	public static extern void Nav_ReleaseLayerByScnName(string scnName);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_HasSketchScene();

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetLayerCount(ref uint layerCount);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetLayerByIndex(uint index, out uint layer);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetScnNameByIndex(uint index, out IntPtr scnName);
	public static bool Nav_GetScnNameByIndex(uint index, out string scnName)
	{
		scnName = null;
		IntPtr p;
		bool rst = Nav_GetScnNameByIndex(index, out p);
		if (!rst) return false;
		scnName = Marshal.PtrToStringAnsi(p);
		Marshal.Release(p);
		return rst;
	}

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetScnNameByLayer(uint layer, ref string scnName);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetLayer(ref NAV_VEC3 pos, out uint layer);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetNavHeight(ref NAV_VEC3 pos, out float height, out uint layer);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetLayerHeight(ref NAV_VEC3 pos, uint layer, out float height);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_LineCast(ref NAV_VEC3 start, ref NAV_VEC3 end, uint layer, out NAV_VEC3 hitPos);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_LineCastEdge(ref NAV_VEC3 start, ref NAV_VEC3 end, uint layer, out NAV_VEC3 hitPos, out NAV_VEC3 edgePoint0, out NAV_VEC3 edgePoint1);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_LineTest(ref NAV_VEC3 start, ref NAV_VEC3 end, uint layer);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_RayCastLayer(ref NAV_VEC3 start, ref NAV_VEC3 end, uint layer, out NAV_VEC3 hitPos);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_RayCastNav(ref NAV_VEC3 start, ref NAV_VEC3 end, out NAV_VEC3 hitPos);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_CalcLayerPath(ref NAV_VEC3 start, ref NAV_VEC3 end, uint layer, out IntPtr pathBuffer, ref uint pathNodeCount);
	public static bool Nav_CalcLayerPath(ref NAV_VEC3 start, ref NAV_VEC3 end, uint layer, out NAV_VEC3[] pathBuffer)
	{
		IntPtr p;
		uint pathNodeCount = 0;
		bool rst = Nav_CalcLayerPath(ref start, ref end, layer, out p, ref pathNodeCount);
		CopyArrayFromPointer(out pathBuffer, p, (int)pathNodeCount);
		Nav_ReleasePath(ref p);
		return rst;
	}

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_CalcPath(ref NAV_VEC3 start, ref NAV_VEC3 end, out IntPtr pathBuffer, ref uint pathNodeCount);
	public static bool Nav_CalcPath(ref NAV_VEC3 start, ref NAV_VEC3 end, out NAV_VEC3[] pathBuffer)
	{
		IntPtr p;
		uint pathNodeCount = 0;
		bool rst = Nav_CalcPath(ref start, ref end, out p, ref pathNodeCount);
		CopyArrayFromPointer(out pathBuffer, p, (int)pathNodeCount);
		Nav_ReleasePath(ref p);
		return rst;
	}

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_ReleasePath(ref IntPtr pathBuffer);

	[DllImport(NavSystemImport.dllName)]
	public static extern void Nav_Release();

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetTrianglesByIndex(out IntPtr verticesBuffer, ref uint verticesCount, uint layer);
	public static bool Nav_GetTrianglesByIndex(out NAV_VEC3[] verticesBuffer, ref uint verticesCount, uint layer)
	{
		IntPtr p;
		bool rst = Nav_GetTrianglesByIndex(out p, ref verticesCount, layer);
		CopyArrayFromPointer(out verticesBuffer, p, (int)verticesCount);
		Nav_ReleaseTriangles(ref p);
		return rst;
	}

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_ReleaseTriangles(ref IntPtr verticesBuffer);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetLayerGateCount(uint layer, out uint gateCount);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_IsLayerGatePassable(uint layer, uint gateIndex, out bool passable);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_SetLayerGatePassable(uint layer, uint gateIndex, bool passable);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetLayerCloseGates(uint layer, out IntPtr verticesBuffer, out uint verticesCount);
	public static bool Nav_GetLayerCloseGates(uint layer, out NAV_VEC3[] verticesBuffer, out uint verticesCount)
	{
		IntPtr p;
		bool rst = Nav_GetLayerCloseGates(layer, out p, out verticesCount);
		CopyArrayFromPointer(out verticesBuffer, p, (int)verticesCount);
		Nav_ReleaseLayerCloseGates(ref p);
		return rst;
	}

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_ReleaseLayerCloseGates(ref IntPtr verticesBuffer);

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_GetLayerEdges(uint layer, out IntPtr verticesBuffer, out uint edgeCount);
	public static bool Nav_GetLayerEdges(uint layer, out NAV_VEC3[] verticesBuffer, out uint edgeCount)
	{
		IntPtr p;
		bool rst = Nav_GetLayerEdges(layer, out p, out edgeCount);
		CopyArrayFromPointer(out verticesBuffer, p, (int)edgeCount * 2);
		Nav_ReleaseLayerEdges(ref p);
		return rst;
	}

	[DllImport(NavSystemImport.dllName)]
	public static extern bool Nav_ReleaseLayerEdges(ref IntPtr verticesBuffer);

	#endregion

	private static bool CopyArrayFromPointer<T>(out T[] buffer, IntPtr p, int count) where T : struct
	{
		if (count <= 0)
		{
			buffer = null;
			return false;
		}
		buffer = new T[count];
		byte[] data = new byte[Marshal.SizeOf(typeof(T)) * count];
		Marshal.Copy(p, data, 0, Marshal.SizeOf(typeof(T)) * count);
		IntPtr structPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
		for (int i = 0; i < count; i++)
		{
			//将byte数组拷到分配好的内存空间
			Marshal.Copy(data, Marshal.SizeOf(typeof(T)) * i, structPtr, Marshal.SizeOf(typeof(T)));
			//将内存空间转换为目标结构体
			T obj = (T)Marshal.PtrToStructure(structPtr, typeof(T));
			buffer[i] = obj;
		}
		//释放内存空间
		Marshal.FreeHGlobal(structPtr);
		return true;
	}
}

