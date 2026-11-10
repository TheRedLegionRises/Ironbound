
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class HexTileMesh : MonoBehaviour
{
    public float radius = 1f;
    public float height = 0.2f;

    void Awake()
    {
        GenerateMesh();
    }

    void GenerateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Hexagonal Tile";

        Vector3[] vertices = new Vector3[14];

        // Top and bottom center vertices.
        vertices[0] = new Vector3(0, height / 2f, 0);
        vertices[7] = new Vector3(0, -height / 2f, 0);

        // Six vertices for the top and bottom.
        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.Deg2Rad * (60f * i);

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            vertices[i + 1] = new Vector3(x, height / 2f, z);
            vertices[i + 8] = new Vector3(x, -height / 2f, z);
        }

        int[] triangles = new int[72];
        int index = 0;

        for (int i = 0; i < 6; i++)
        {
            int next = (i + 1) % 6;

            // Top face.
            triangles[index++] = 0;
            triangles[index++] = i + 1;
            triangles[index++] = next + 1;

            // Bottom face (reverse winding).
            triangles[index++] = 7;
            triangles[index++] = next + 8;
            triangles[index++] = i + 8;

            // Side faces.
            triangles[index++] = i + 1;
            triangles[index++] = i + 8;
            triangles[index++] = next + 8;

            triangles[index++] = i + 1;
            triangles[index++] = next + 8;
            triangles[index++] = next + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}
