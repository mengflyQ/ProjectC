using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NavMeshDecal : MonoBehaviour
{
    [Header("材质")]
    public Material mMaterial;
    [Header("离地高度")]
    public float mOffsetDistance = 0.009f;
    [Header("上表面高度")]
    public float mUpRadius = 0.0f;
    [Header("下表面高度")]
    public float mDownRadius = 0.0f;
    [Header("附加半径")]
    public Vector2 mAreaSizeOffset = Vector2.zero;
    [Header("顶点颜色")]
    public Color mColor = Color.white;

    public float mSegVal = 3.0f;

    private Color mLastColor = Color.white;
    private MeshRenderer renderer = null;

    public uint NavLayer
    {
        set;
        get;
    }

    public Material material
    {
        get
        {
            return mMaterial;
        }
    }

    void Start()
    {
        mLastColor = mColor;
        BuildMesh();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 size = transform.lossyScale;
        Vector3 min = -size * 0.5f;
        Vector3 max = size * 0.5f;
        min.y -= mDownRadius;
        max.y += mUpRadius;
        min.x -= mAreaSizeOffset.x;
        max.x += mAreaSizeOffset.x;
        min.z -= mAreaSizeOffset.y;
        max.z += mAreaSizeOffset.y;
        Vector3 center = (max - min) * 0.5f + min;
        size = (max - min);

        Gizmos.DrawWireCube(center, size);
    }

    void Update()
    {
        if (mLastColor != mColor)
        {
            UpdateColor();
            mLastColor = mColor;
        }
    }

    public Bounds GetBounds()
    {
        Vector3 size = transform.lossyScale;
        Vector3 min = -size / 2f;
        Vector3 max = size / 2f;
        min.y -= mDownRadius;
        max.y += mUpRadius;
        min.x -= mAreaSizeOffset.x;
        max.x += mAreaSizeOffset.x;
        min.z -= mAreaSizeOffset.y;
        max.z += mAreaSizeOffset.y;

        Vector3[] vts = new Vector3[]
        {
            new Vector3(min.x, min.y, min.z),
            new Vector3(max.x, min.y, min.z),
            new Vector3(min.x, max.y, min.z),
            new Vector3(max.x, max.y, min.z),

            new Vector3(min.x, min.y, max.z),
            new Vector3(max.x, min.y, max.z),
            new Vector3(min.x, max.y, max.z),
            new Vector3(max.x, max.y, max.z),
        };

        for (int i = 0; i < 8; i++)
        {
            vts[i] = transform.TransformDirection(vts[i]);
        }

        min = max = vts[0];
        foreach (Vector3 v in vts)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        return new Bounds(transform.position, max - min);
    }

    public void UpdateColor()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null)
            return;
        Mesh mesh = filter.mesh;
        Color[] colors = mesh.colors;
        for (int i = 0; i < colors.Length; ++i)
        {
            colors[i] = mColor;
        }
        mesh.colors = colors;
    }

    public void BuildMesh()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        if (filter == null) filter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer r = GetComponent<MeshRenderer>();
        if (r == null) r = gameObject.AddComponent<MeshRenderer>();
        r.material = mMaterial;
        renderer = r;

        if (mMaterial == null)
        {
            filter.mesh = null;
            return;
        }

        Bounds bounds = GetBounds();

        List<Vector3> verts = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Color> colors = new List<Color>();
        List<int> indices = new List<int>();
        
        float segCount = Mathf.Round(mSegVal);
        float pointCount = segCount + 1.0f;

        Vector3 size = transform.lossyScale;
        Vector3 min = -size * 0.5f;
        Vector3 max = size * 0.5f;
        min.y = 0.0f;
        max.y = 0.0f;
        min.x -= mAreaSizeOffset.x;
        max.x += mAreaSizeOffset.x;
        min.z -= mAreaSizeOffset.y;
        max.z += mAreaSizeOffset.y;
        size = (max - min);

        float lenX = size.x;
        float lenZ = size.z;
        float cellX = lenX / segCount;
        float cellZ = lenZ / segCount;
        Vector3 ux = new Vector3(transform.right.x, 0.0f, transform.right.z);
        ux.Normalize();
        Vector3 uz = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
        uz.Normalize();
        Vector3 upos = bounds.center;
        for (int i = 0; i < pointCount; ++i)
        {
            for (int j = 0; j < pointCount; ++j)
            {
                Vector3 vx = cellX * (float)j * ux;
                Vector3 vy = cellZ * (float)i * uz;
                Vector3 u = vx + vy;
                u += upos;
                u -= (lenX * 0.5f * ux + lenZ * 0.5f * uz);
                Vector3 vert = new Vector3(u.x, 0.0f, u.z);

                Vector2 uv = new Vector2((float)j / segCount, (float)i / segCount);
                float h = 0.0f;
                if (!NavigationSystem.GetLayerHeight(vert, NavLayer, out h))
                {
                    h = transform.position.y;
                }
                vert.y = h + mOffsetDistance;// - transform.position.y;

                vert = transform.worldToLocalMatrix.MultiplyPoint(vert);
                verts.Add(vert);
                uvs.Add(uv);
                colors.Add(mColor);
            }
        }
        for (int i = 0; i < segCount; ++i)
        {
            for (int j = 0; j < segCount; ++j)
            {
                int ptCount = (int)pointCount;
                indices.Add((i + 1) * ptCount + j);

                indices.Add((i + 1) * ptCount + (j + 1));
                indices.Add(i * ptCount + j);

                indices.Add(i * ptCount + (j + 1));

                indices.Add(i * ptCount + j);
                indices.Add((i + 1) * ptCount + (j + 1));
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.triangles = indices.ToArray();
        if (mesh != null)
        {
            mesh.name = "NavMeshDecal";
        }
        filter.mesh = mesh;
    }
}