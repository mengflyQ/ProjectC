using System;
using System.Collections.Generic;
using GameServer.RoomServer;
using MathLib;
using System.IO;
using LitJson;

public class MarkPoint
{
    public string name;
    public Vector3 position;
    public Vector3 direction;
}

public enum RefreshType
{
    RepeatRefresh,
    OnceRefresh,
    OtherRefresh
}

public enum RefreshCharacterType
{
    Active,
    Passitive,
    Neutral,
}

public class RefreshSystem : BaseSystem
{
    public override void Initialize()
    {
    }

    public List<Character> Refresh(Scene scn)
    {
        LoadScnMark(scn.ScnID, scn.ScnList.markPath);
        List<excel_refresh> excels = ExcelLoader.LoadRefreshExcel(scn.ScnList.id);
        if (excels == null)
            return null;
        return RefreshList(scn);
    }

    void LoadScnMark(int scnID, string markPath)
    {
        if (string.IsNullOrEmpty(markPath))
            return;

        List<MarkPoint> scnMarkPoints = null;
        if (!mScnMarkPoints.TryGetValue(scnID, out scnMarkPoints))
        {
            scnMarkPoints = new List<MarkPoint>();
            mScnMarkPoints.Add(scnID, scnMarkPoints);
        }
        else
        {
            return;
        }

        string jsonText = string.Empty;
        using (FileStream fsRead = new FileStream("../Data/RefreshInfo/" + markPath + ".json", FileMode.Open))
        {
            byte[] configDatas = new byte[fsRead.Length];
            fsRead.Read(configDatas, 0, configDatas.Length);
            jsonText = System.Text.Encoding.UTF8.GetString(configDatas);
            fsRead.Close();
        }
        JsonData json = JsonMapper.ToObject(jsonText);

        JsonData markPoints = json["MarkPoints"];
        for (int i = 0; i < markPoints.Count; ++i)
        {
            JsonData markPoint = markPoints[i];
            JsonData nameJson = markPoint["name"];
            JsonData posJson = markPoint["Pos"];
            JsonData dirJson = markPoint["Dir"];

            MarkPoint pt = new MarkPoint();
            pt.name = nameJson.AsString;
            pt.position = new Vector3(posJson[0].AsFloat, posJson[1].AsFloat, posJson[2].AsFloat);
            pt.direction = new Vector3(dirJson[0].AsFloat, dirJson[1].AsFloat, dirJson[2].AsFloat);
            scnMarkPoints.Add(pt);
        }
    }

    List<Character> RefreshList(Scene scn)
    {
        List<excel_refresh> refreshExcels = null;
        List<MarkPoint> markExcels = null;
        if (!mScnRefreshDatas.TryGetValue(scn.ScnID, out refreshExcels))
        {
            return null;
        }
        if (!mScnMarkPoints.TryGetValue(scn.ScnID, out markExcels))
        {
            return null;
        }
        List<Character> rst = new List<Character>();
        for (int i = 0; i < refreshExcels.Count; ++i)
        {
            excel_refresh refreshExcel = refreshExcels[i];
            for (int j = 0; j < refreshExcel.refreshCount; ++j)
            {
                if (refreshExcel.refreshType == (int)RefreshType.RepeatRefresh
                    || refreshExcel.refreshType == (int)RefreshType.OnceRefresh)
                {
                    Character cha = Refresh(refreshExcel, scn);
                    rst.Add(cha);
                }
            }
        }
        return rst;
    }

    public Character Refresh(excel_refresh refreshExcel, Scene scn)
    {
        int index = Mathf.RandRange(0, refreshExcel.birthpoint.Length - 1);
        string birthPointName = refreshExcel.birthpoint[index];
        MarkPoint markPoint = GetMarkPoint(scn.ScnID, birthPointName);
        if (markPoint == null)
            return null;
        Vector3 targetPos = markPoint.position;
        Vector3 dir = new Vector3(Mathf.RandRange(-1.0f, 1.0f), 0.0f, Mathf.RandRange(-1.0f, 1.0f));
        dir.Normalize();
        float dist = Mathf.RandRange(0.0f, 1.0f) * refreshExcel.refreshDist;
        targetPos += (dist * dir);
        Vector3 hitPos = Vector3.zero;

        uint layer = NavigationSystem.GetLayer(markPoint.position);
        if (NavigationSystem.LineCast(markPoint.position, targetPos, layer, out hitPos))
        {
            targetPos = hitPos;
        }
        float h = 0.0f;
        if (NavigationSystem.GetLayerHeight(targetPos, layer, out h))
        {
            targetPos.y = h;
        }
        NPC npc = new NPC();
        npc.Position = targetPos;
        npc.Direction = markPoint.direction;
        npc.gid = GIDManger.Instance.GetGID();
        npc.mRefreshList = refreshExcel;
        npc.mChaList = excel_cha_list.Find(refreshExcel.chaListID);
        scn.AddNPC(npc);

        ScnNPCInfo msg = new ScnNPCInfo();
        msg.gid = npc.gid;
        msg.position = Vector3Packat.FromVector3(npc.Position);
        msg.direction = Vector3Packat.FromVector3(npc.Direction);
        msg.chaListID = refreshExcel.chaListID;

        for (int i = 0; i < scn.GetPlayerCount(); ++i)
        {
            Player player = scn.GetPlayerByIndex(i);
            NetWork.NotifyMessage<ScnNPCInfo>(player.UserID, STC.STC_RefreshNPC, msg);
        }

        npc.PacketAtb();

        return npc;
    }

    public MarkPoint GetMarkPoint(int scnID, string name)
    {
        List<MarkPoint> markPoints = null;
        if (!mScnMarkPoints.TryGetValue(scnID, out markPoints))
        {
            return null;
        }
        for (int i = 0; i < markPoints.Count; ++i)
        {
            MarkPoint pt = markPoints[i];
            if (pt.name == name)
                return pt;
        }
        return null;
    }

    public Dictionary<int, List<excel_refresh>> mScnRefreshDatas = new Dictionary<int,List<excel_refresh>>();
    public Dictionary<int, List<MarkPoint>> mScnMarkPoints = new Dictionary<int,List<MarkPoint>>();

    static RefreshSystem mInstance = null;
    public static RefreshSystem Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new RefreshSystem();
                mInstance.Initialize();
            }
            return mInstance;
        }
    }
}