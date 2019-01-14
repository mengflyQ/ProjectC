using UnityEngine;

public enum SkillWarningType
{
    Fan,
    Circle,
    Rect,
    PreOpFan,
    PreOpCircle,
    PreOpRect,
}

[ExecuteInEditMode]
public class SkillWarning : MonoBehaviour
{
    void Start()
    {
        if (Decal == null)
        {
            Debug.LogError("未找到Decal");
            return;
        }
        if (Decal.material == null)
        {
            Debug.LogError("Decal上没有材质");
            return;
        }
        Decal.upHeight = 2.0f;
        Decal.downHeight = 2.0f;
        Decal.offsetY = 0.2f;
        Decal.BuildMesh();
    }

    void OnEnalbe()
    {
        mStartTime = Time.realtimeSinceStartup;
#if UNITY_EDITOR
        if (Decal != null)
        {
            Decal.upHeight = 2.0f;
            Decal.downHeight = 2.0f;
            Decal.offsetY = 0.2f;
            Decal.BuildMesh();
        }
#endif
    }

    public void Release()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        float time = Time.realtimeSinceStartup - mStartTime;
        if (Duration > 0.0f && time > Duration)
        {
            Reset();
            Release();
            return;
        }
        if (Follow && mFollower != null)
        {
            if (skillWarningType != SkillWarningType.PreOpCircle)
            {
                transform.position = mFollower.transform.position;
            }
            if (skillWarningType != SkillWarningType.PreOpFan && skillWarningType != SkillWarningType.PreOpRect)
            {
                transform.rotation = mFollower.transform.rotation;
            }
            Decal.UpdateMesh();
        }
    }

    void Reset()
    {
        Follow = false;
        mFollower = null;
        Data1 = 0.0f;
        Data2 = 0.0f;
        Duration = 0.0f;
        mStartTime = 0.0f;
    }

    public static void CreateSkillWarning(string path, SkillWarningType type, float data1, float data2, float duration, Vector3 pos, Quaternion rot, bool follow = false, Transform follower = null, System.Action<SkillWarning> callback = null)
    {
        ResourceSystem.LoadAsync<GameObject>(path, (o) => {
            GameObject go = o as GameObject;
            if (go == null)
                return;
            SkillWarning warning = go.GetComponent<SkillWarning>();
            if (warning == null)
                return;
            GameObject obj = GameObject.Instantiate(go);
            if (obj == null)
                return;
            obj.name = "skill_warning_" + type.ToString();
            warning = obj.GetComponent<SkillWarning>();
            if (warning == null)
                return;
            warning.skillWarningType = type;
            warning.Data1 = data1;
            warning.Data2 = data2;
            warning.Duration = duration;
            warning.Follow = follow;
            warning.mFollower = follower;

            warning.mStartTime = Time.realtimeSinceStartup;

            obj.transform.position = pos;
            obj.transform.rotation = rot;

            warning.Decal.upHeight = 2.0f;
            warning.Decal.downHeight = 2.0f;
            warning.Decal.offsetY = 0.2f;
            warning.Decal.BuildMesh();

            if (callback != null)
            {
                callback(warning);
            }
        });
    }

    public bool Follow
    {
        set;
        get;
    }

    float mData1 = 0.0f;
    float mData2 = 0.0f;

    public float Data1
    {
        set
        {
            switch (skillWarningType)
            {
                case SkillWarningType.Fan:
                case SkillWarningType.PreOpFan:
                case SkillWarningType.Circle:
                case SkillWarningType.PreOpCircle:
                    Decal.areaSizeOffset.x = value - 0.5f;
                    Decal.areaSizeOffset.y = value - 0.5f;
                    Decal.transform.localPosition = Vector3.zero;
                    break;
                case SkillWarningType.Rect:
                case SkillWarningType.PreOpRect:
                    Decal.areaSizeOffset.x = value * 0.5f - 0.5f;
                    break;
            }
            mData1 = value;
        }
    }

    public float Data2
    {
        set
        {
            switch (skillWarningType)
            {
                case SkillWarningType.Fan:
                case SkillWarningType.PreOpFan:
                    Decal.material.SetFloat("_HalfRad", value);
                    Decal.transform.localPosition = Vector3.zero;
                    break;
                case SkillWarningType.Rect:
                case SkillWarningType.PreOpRect:
                    Decal.areaSizeOffset.y = value * 0.5f - 0.5f;
                    Vector3 pos = transform.position;
                    pos.z += value * 0.5f;
                    Decal.transform.position = pos;
                    break;
            }
            mData2 = value;
        }
    }

    public uint NavLayer
    {
        set
        {
            if (Decal != null)
            {
                Decal.mNavLayer = value;
            }
        }
    }

    public float Duration
    {
        set;
        get;
    }

    public SkillWarningType skillWarningType
    {
        set;
        get;
    }

    public Transform mFollower
    {
        set;
        get;
    }

    NavMeshDecal mDecal = null;
    NavMeshDecal Decal
    {
        get
        {
            if (mDecal == null)
                mDecal = GetComponentInChildren<NavMeshDecal>();
            return mDecal;
        }
    }
    float mStartTime = 0.0f;
}