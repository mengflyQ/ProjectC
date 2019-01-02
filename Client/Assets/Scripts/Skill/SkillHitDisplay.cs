using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillHitDisplay : MonoBehaviour
{
    public excel_skill_hit mHitExcel;
    public float mDisplayTime = 1.0f;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        SkillHitShape shape = (SkillHitShape)mHitExcel.hitType;

        float data1 = (float)mHitExcel.hitData1 * 0.001f;
        float data2 = (float)mHitExcel.hitData2 * 0.001f;
        float height = (float)mHitExcel.hitData3 * 0.001f;

        switch (shape)
        {
            case SkillHitShape.FanSingle:
            case SkillHitShape.FanMultiple:
                {
                    data2 *= 1000.0f;

                    Vector3 dir = transform.forward;
                    dir.y = 0.0f;
                    dir.Normalize();
                    Matrix4x4 mat = Matrix4x4.identity;
                    Quaternion rot = Quaternion.identity;

                    Vector3 pos = transform.position;
                    Vector3[,] connLine = new Vector3[2, 3];

                    for (int face = 0; face < 2; ++face)
                    {
                        pos.y += ((float)face * height);

                        rot = Quaternion.AngleAxis(data2, Vector3.up);
                        mat.SetTRS(Vector3.zero, rot, Vector3.one);
                        Vector3 pt1 = mat.MultiplyVector(dir * data1) + pos;
                        rot = Quaternion.AngleAxis(-data2, Vector3.up);
                        mat.SetTRS(Vector3.zero, rot, Vector3.one);
                        Vector3 pt2 = mat.MultiplyPoint(dir * data1) + pos;

                        Vector3 pt = pos;

                        connLine[face, 0] = pt;
                        connLine[face, 1] = pt1;
                        connLine[face, 2] = pt2;

                        Gizmos.DrawLine(pt, pt1);
                        Gizmos.DrawLine(pt, pt2);

                        Vector3 lastPt = pt2;
                        for (float a = -data2; a <= data2; ++a)
                        {
                            rot = Quaternion.AngleAxis(a, Vector3.up);
                            mat.SetTRS(Vector3.zero, rot, Vector3.one);
                            Vector3 pts = mat.MultiplyPoint(dir * data1) + pos;
                            Gizmos.DrawLine(lastPt, pts);
                            lastPt = pts;
                        }
                        Gizmos.DrawLine(lastPt, pt1);
                    }

                    Gizmos.DrawLine(connLine[0, 0], connLine[1, 0]);
                    Gizmos.DrawLine(connLine[0, 1], connLine[1, 1]);
                    Gizmos.DrawLine(connLine[0, 2], connLine[1, 2]);
                }
                break;
            case SkillHitShape.RectSingle:
            case SkillHitShape.RectMultiple:
                {
                    Vector3 right = transform.right;
                    right.y = 0.0f;
                    right.Normalize();

                    Vector3 forward = transform.forward;
                    forward.y = 0.0f;
                    forward.Normalize();

                    Vector3 pos = transform.position;
                    Vector3[,] connLine = new Vector3[2, 4];

                    for (int face = 0; face < 2; ++face)
                    {
                        pos.y += ((float)face * height);

                        Vector3 pt1 = right * data1 * 0.5f;
                        Vector3 pt2 = -right * data1 * 0.5f;
                        Vector3 pt3 = forward * data2;
                        Vector3 pt4 = Vector3.zero;

                        Vector3 v1 = pt1 + pt4 + pos;
                        Vector3 v2 = pt2 + pt4 + pos;
                        Vector3 v3 = pt2 + pt3 + pos;
                        Vector3 v4 = pt1 + pt3 + pos;

                        connLine[face, 0] = v1;
                        connLine[face, 1] = v2;
                        connLine[face, 2] = v3;
                        connLine[face, 3] = v4;

                        Gizmos.DrawLine(v1, v2);
                        Gizmos.DrawLine(v2, v3);
                        Gizmos.DrawLine(v3, v4);
                        Gizmos.DrawLine(v4, v1);
                    }
                    Gizmos.DrawLine(connLine[0, 0], connLine[1, 0]);
                    Gizmos.DrawLine(connLine[0, 1], connLine[1, 1]);
                    Gizmos.DrawLine(connLine[0, 2], connLine[1, 2]);
                    Gizmos.DrawLine(connLine[0, 3], connLine[1, 3]);
                }
                break;
            case SkillHitShape.CircleSingle:
            case SkillHitShape.CircleMultiple:
                {
                    Vector3 dir = transform.forward;
                    dir.y = 0.0f;
                    dir.Normalize();

                    Matrix4x4 mat = Matrix4x4.identity;
                    Quaternion rot = Quaternion.identity;

                    Vector3 pos = transform.position;
                    Vector3[,] connLine = new Vector3[2, 8];

                    for (int face = 0; face < 2; ++face)
                    {
                        pos.y += ((float)face * height);

                        rot = Quaternion.AngleAxis(0.0f, Vector3.up);
                        mat.SetTRS(Vector3.zero, rot, Vector3.one);
                        Vector3 ptLast = mat.MultiplyPoint(dir * data1) + pos;

                        for (float a = 0.0f; a <= 360.0f; ++a)
                        {
                            rot = Quaternion.AngleAxis(a, Vector3.up);
                            mat.SetTRS(Vector3.zero, rot, Vector3.one);
                            Vector3 pt = mat.MultiplyPoint(dir * data1) + pos;

                            if (a == 0.0f)
                                connLine[face, 0] = pt;
                            if (a == 90.0f)
                                connLine[face, 1] = pt;
                            if (a == 180.0f)
                                connLine[face, 2] = pt;
                            if (a == 270.0f)
                                connLine[face, 3] = pt;
                            if (a == 45.0f)
                                connLine[face, 4] = pt;
                            if (a == 135.0f)
                                connLine[face, 5] = pt;
                            if (a == 225.0f)
                                connLine[face, 6] = pt;
                            if (a == 315.0f)
                                connLine[face, 7] = pt;

                            Gizmos.DrawLine(ptLast, pt);
                            ptLast = pt;
                        }
                    }
                    Gizmos.DrawLine(connLine[0, 0], connLine[1, 0]);
                    Gizmos.DrawLine(connLine[0, 1], connLine[1, 1]);
                    Gizmos.DrawLine(connLine[0, 2], connLine[1, 2]);
                    Gizmos.DrawLine(connLine[0, 3], connLine[1, 3]);
                    Gizmos.DrawLine(connLine[0, 4], connLine[1, 4]);
                    Gizmos.DrawLine(connLine[0, 5], connLine[1, 5]);
                    Gizmos.DrawLine(connLine[0, 6], connLine[1, 6]);
                    Gizmos.DrawLine(connLine[0, 7], connLine[1, 7]);
                }
                break;
        }
    }

    private void Update()
    {
        mDisplayTime -= Time.deltaTime;
        if (mDisplayTime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}