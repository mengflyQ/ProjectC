using System;
using System.Collections.Generic;
using System.IO;
using MathLib;

namespace GameServer.RoomServer
{
    public static class NavigationSystem
    {
        public static void LoadAllNavigation()
        {
            for (int i = 0; i < excel_scn_list.Count; ++i)
            {
                excel_scn_list scnList = excel_scn_list.GetByIndex(i);
                if (scnList.temp > 0)
                {
                    LoadingNav(scnList.name);
                }
            }
        }

        static void LoadingNav(string scnName)
        {
            string fileName = @"../Data/Navigation/" + scnName + "/navigation.nav";
            if (!File.Exists(fileName))
                return;
            using (FileStream fsRead = new FileStream(fileName, FileMode.Open))
            {
                byte[] datas = new byte[fsRead.Length];
                fsRead.Read(datas, 0, datas.Length);
                fsRead.Close();

                bool rst = NavSystemImport.Nav_CreateFromMemory(datas, scnName);
                if (!rst)
                {
                    Debug.LogError("[navigation]创建导航失败");
                }
            }
        }

        public static void OnExitScene()
        {
            NavSystemImport.Nav_Release();
        }

        public static uint GetLayer(Vector3 pos)
        {
            NAV_VEC3 vPos = new NAV_VEC3(pos);

            uint layer;
            bool rst = NavSystemImport.Nav_GetLayer(ref vPos, out layer);
            if (!rst)
            {
                Console.WriteLine("[navigation]获取layer失败");
            }

            return layer;
        }

        public static bool GetLayerHeight(Vector3 pos, uint layer, out float h)
        {
            NAV_VEC3 vPos = new NAV_VEC3(pos);

            return NavSystemImport.Nav_GetLayerHeight(ref vPos, layer, out h);
        }

        public static bool LineCastEdge(Vector3 start, Vector3 end, uint layer, out Vector3 hitPoint, out Vector3 edgePoint0, out Vector3 edgePoint1)
        {
            NAV_VEC3 vStart = new NAV_VEC3(start);
            NAV_VEC3 vEnd = new NAV_VEC3(end);

            NAV_VEC3 vHitPoint, vEdgePoint0, vEdgePoint1;

            bool rst = NavSystemImport.Nav_LineCastEdge(ref vStart, ref vEnd, layer, out vHitPoint, out vEdgePoint0, out vEdgePoint1);

            hitPoint = vHitPoint.ToVector3();
            edgePoint0 = vEdgePoint0.ToVector3();
            edgePoint1 = vEdgePoint1.ToVector3();

            return rst;
        }

        public static bool LineCast(Vector3 start, Vector3 end, uint layer, out Vector3 hitPoint)
        {
            NAV_VEC3 vStart = new NAV_VEC3(start);
            NAV_VEC3 vEnd = new NAV_VEC3(end);

            NAV_VEC3 vHitPoint;

            bool rst = NavSystemImport.Nav_LineCast(ref vStart, ref vEnd, layer, out vHitPoint);

            hitPoint = vHitPoint.ToVector3();
            return rst;
        }

        public static bool LineTest(Vector3 start, Vector3 end, uint layer)
        {
            NAV_VEC3 vStart = new NAV_VEC3(start);
            NAV_VEC3 vEnd = new NAV_VEC3(end);

            return NavSystemImport.Nav_LineTest(ref vStart, ref vEnd, layer);
        }

        public static bool RayCastNav(Vector3 start, Vector3 end, out Vector3 hitPos)
        {
            NAV_VEC3 vStart = new NAV_VEC3(start);
            NAV_VEC3 vEnd = new NAV_VEC3(end);

            NAV_VEC3 vHitPoint;

            bool rst = NavSystemImport.Nav_RayCastNav(ref vStart, ref vEnd, out vHitPoint);

            hitPos = vHitPoint.ToVector3();

            return rst;
        }

        public static bool Nav_CalcLayerPath(Vector3 start, Vector3 end, uint layer, out Vector3[] path)
        {
            NAV_VEC3 vStart = new NAV_VEC3(start);
            NAV_VEC3 vEnd = new NAV_VEC3(end);

            NAV_VEC3[] pathBuffer;

            bool rst = NavSystemImport.Nav_CalcLayerPath(ref vStart, ref vEnd, layer, out pathBuffer);

            path = null;
            if (rst)
            {
                path = new Vector3[pathBuffer.Length];
                for (int i = 0; i < pathBuffer.Length; ++i)
                {
                    path[i] = pathBuffer[i].ToVector3();
                }
            }
            return rst;
        }
    }
}
