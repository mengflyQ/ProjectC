using System;
using System.Collections.Generic;
using UnityEngine;

public class MarkPoint : MonoBehaviour
{
    void OnDrawGizmos()
    {
        if (gismosMesh == null)
        {
            gismosMesh = new Mesh();
            gismosMesh.name = "MarkPointMesh";
            Vector3[] vertices = new Vector3[11];
            vertices[0] = new Vector3(-0.5f, 0.0f, -0.5f);
            vertices[1] = new Vector3(-0.5f, 0.0f,  0.5f);
            vertices[2] = new Vector3( 0.5f, 0.0f, -0.5f);
            vertices[3] = new Vector3( 0.5f, 0.0f,  0.5f);
            vertices[4] = new Vector3(-0.5f, 1.8f, -0.5f);
            vertices[5] = new Vector3(-0.5f, 1.8f,  0.5f);
            vertices[6] = new Vector3( 0.5f, 1.8f, -0.5f);
            vertices[7] = new Vector3( 0.5f, 1.8f,  0.5f);
            vertices[8] = new Vector3(-0.5f, 1.3f,  0.5f);
            vertices[9] = new Vector3( 0.5f, 1.3f,  0.5f);
            vertices[10] = new Vector3(0.0f, 1.55f, 1.5f);
            int[] indices = new int[] {
                0, 2, 1,
                3, 1, 2,
                4, 5, 6,
                7, 6, 5,
                0, 4, 2,
                6, 2, 4,
                2, 6, 3,
                7, 3, 6,
                3, 9, 1,
                8, 1, 9,
                1, 5, 0,
                4, 0, 5,
                7, 5, 10,
                5, 8, 10,
                8, 9, 10,
                9, 7, 10
            };
            gismosMesh.vertices = vertices;
            gismosMesh.triangles = indices;
            gismosMesh.RecalculateNormals();
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawMesh(gismosMesh, transform.position, transform.rotation);
    }

    static Mesh gismosMesh = null;
}