using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class NavMeshDecal : MonoBehaviour
{
    [Header("材质")]
    public Material material;
    [Header("离地高度")]
    public float offsetY = 0.05f;
    [Header("上表面高度")]
    public float upHeight = 0.0f;
    [Header("下表面高度")]
    public float downHeight = 0.0f;
    [Header("附加半径")]
    public Vector2 areaSizeOffset = Vector2.zero;
    [Header("X分段数")]
    public int mSegCountX = 5;
    [Header("Z分段数")]
    public int mSegCountZ = 5;
    
    [System.NonSerialized]
    public uint mNavLayer = 0;

    private MeshFilter mMeshFilter = null;
    private MeshRenderer mMeshRenderer = null;

    void Awake()
    {
        mMeshFilter = GetComponent<MeshFilter>();
        mMeshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        BuildMesh();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Vector3 size = transform.lossyScale;
        Vector3 min = -size * 0.5f;
        Vector3 max = size * 0.5f;
        min.y -= downHeight;
        max.y += upHeight;
        min.x -= areaSizeOffset.x;
        max.x += areaSizeOffset.x;
        min.z -= areaSizeOffset.y;
        max.z += areaSizeOffset.y;
        Vector3 center = (max - min) * 0.5f + min;
        size = (max - min);

        Gizmos.DrawWireCube(center, size);
    }

    public Bounds GetBounds()
    {
        Vector3 size = transform.lossyScale;
        Vector3 min = -size / 2f;
        Vector3 max = size / 2f;
        min.y -= downHeight;
        max.y += upHeight;
        min.x -= areaSizeOffset.x;
        max.x += areaSizeOffset.x;
        min.z -= areaSizeOffset.y;
        max.z += areaSizeOffset.y;

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
        for (int i = 0; i < vts.Length; ++i)
        {
            Vector3 v = vts[i];
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        return new Bounds(transform.position, max - min);
    }

    public void UpdateMesh()
    {
        if (mMeshFilter == null)
            return;
        if (mMeshFilter.mesh == null)
            return;
        Vector3[] vs = new Vector3[mMeshFilter.mesh.vertexCount];
        for (int i = 0; i < mMeshFilter.mesh.vertexCount; ++i)
        {
            Vector3 v = mMeshFilter.mesh.vertices[i];
            v = transform.localToWorldMatrix.MultiplyPoint(v);

            float h = 0.0f;
            if (!NavigationSystem.GetLayerHeight(v, mNavLayer, out h))
            {
//				mNavSys.GetLayer(v, out mNavLayer);
                h = transform.position.y;
            }
            v.y = h;
            v.y += offsetY;

            v = transform.worldToLocalMatrix.MultiplyPoint(v);
            vs[i] = v;
        }
        mMeshFilter.mesh.vertices = vs;
    }

    public void BuildMesh()
    {
        if (mMeshFilter == null)
            return;
        if (mMeshRenderer == null)
            return;
        mMeshRenderer.material = material;

        if (material == null)
        {
            mMeshFilter.mesh = null;
            return;
        }

        verts.Clear();
        uvs.Clear();
        indices.Clear();
        {
            int pointCountX = mSegCountX + 1;
            int pointCountZ = mSegCountZ + 1;

            Vector3 size = transform.lossyScale;
            Vector3 min = -size * 0.5f;
            Vector3 max = size * 0.5f;
            min.y = 0.0f;
            max.y = 0.0f;
            min.x -= areaSizeOffset.x;
            max.x += areaSizeOffset.x;
            min.z -= areaSizeOffset.y;
            max.z += areaSizeOffset.y;
            size = (max - min);

            float lenX = size.x;
            float lenZ = size.z;
            float cellX = lenX / mSegCountX;
            float cellZ = lenZ / mSegCountZ;
            Vector3 ux = new Vector3(transform.right.x, 0.0f, transform.right.z);
            ux.Normalize();
            Vector3 uz = new Vector3(transform.forward.x, 0.0f, transform.forward.z);
            uz.Normalize();
            Vector3 upos = transform.position;
            for (int i = 0; i < pointCountZ; ++i)
            {
                for (int j = 0; j < pointCountX; ++j)
                {
                    Vector3 vx = cellX * (float)j * ux;
                    Vector3 vy = cellZ * (float)i * uz;
                    Vector3 u = vx + vy;
                    u += upos;
                    u -= (lenX * 0.5f * ux + lenZ * 0.5f * uz);
                    Vector3 vert = new Vector3(u.x, transform.position.y, u.z);

                    Vector2 uv = new Vector2((float)j / mSegCountX, (float)i / mSegCountZ);
                    float h = 0.0f;
                    if (!NavigationSystem.GetLayerHeight(vert, mNavLayer, out h))
                    {
                        //mNavLayer = NavigationSystem.GetLayer(vert);
                        h = transform.position.y;
                    }
                    vert.y = h + offsetY;// - transform.position.y;

                    vert = transform.worldToLocalMatrix.MultiplyPoint(vert);
                    verts.Add(vert);
                    uvs.Add(uv);
                }
            }
            for (int i = 0; i < mSegCountZ; ++i)
            {
                for (int j = 0; j < mSegCountX; ++j)
                {
                    int ptCount = (int)pointCountX;
                    indices.Add((i + 1) * ptCount + j);

                    indices.Add((i + 1) * ptCount + (j + 1));
                    indices.Add(i * ptCount + j);

                    indices.Add(i * ptCount + (j + 1));

                    indices.Add(i * ptCount + j);
                    indices.Add((i + 1) * ptCount + (j + 1));
                }
            }
        }

        Mesh mesh = null;
        if (mMeshFilter.mesh != null)
            mesh = mMeshFilter.mesh;
        else
            mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = indices.ToArray();

        verts.Clear();
        uvs.Clear();
        indices.Clear();

        if (mesh != null)
        {
            mesh.name = "NavMeshDecal";
        }
        mMeshFilter.mesh = mesh;
    }

    static List<Vector3> verts = new List<Vector3>();
    static List<Vector2> uvs = new List<Vector2>();
    static List<int> indices = new List<int>();
}