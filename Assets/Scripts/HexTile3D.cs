using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexTile3D : MonoBehaviour
{
    [Tooltip("Distance from center to a corner.")]
    public float innerRadius = 1f;
    
    [Tooltip("Thickness / depth of the 3D tile.")]
    public float height = 0.5f;

    [Tooltip("True for Pointy-top orientation, False for Flat-top orientation.")]
    public bool isPointyTop = true;

    private Mesh filterMesh;

    void OnValidate()
    {
        // Re-generate in the editor when variables change
        if (Application.isPlaying == false) GenerateHexPrism();
    }

    void Start()
    {
        GenerateHexPrism();
    }

    public void GenerateHexPrism()
    {
        filterMesh = new Mesh();
        filterMesh.name = "ProceduralHexTile";
        
        // 12 vertices (6 top, 6 bottom)
        Vector3[] vertices = new Vector3[12];
        
        // Calculate outer radius based on trigonometry standard math
        float orientationOffset = isPointyTop ? 30f : 0f;

        // Generate Top and Bottom vertices
        for (int i = 0; i < 6; i++)
        {
            float angleDeg = orientationOffset + (i * 60f);
            float angleRad = Mathf.Deg2Rad * angleDeg;

            float x = innerRadius * Mathf.Cos(angleRad);
            float z = innerRadius * Mathf.Sin(angleRad);

            // Top Face Vertices (Positive Y half-height)
            vertices[i] = new Vector3(x, height / 2f, z);
            // Bottom Face Vertices (Negative Y half-height)
            vertices[i + 6] = new Vector3(x, -height / 2f, z);
        }

        // Define Triangles (Clockwise winding order dictates facing directions)
        int[] triangles = new int[
            (4 * 3) +       // Top face (4 triangles)
            (4 * 3) +       // Bottom face (4 triangles)
            (6 * 2 * 3)     // 6 Sides (2 triangles each)
        ];

        int tIdx = 0;

        // --- TOP FACE ---
        for (int i = 1; i < 5; i++)
        {
            triangles[tIdx++] = 0;
            triangles[tIdx++] = i;
            triangles[tIdx++] = i + 1;
        }

        // --- BOTTOM FACE (Flipped winding order to face downward) ---
        for (int i = 1; i < 5; i++)
        {
            triangles[tIdx++] = 6;
            triangles[tIdx++] = 6 + i + 1;
            triangles[tIdx++] = 6 + i;
        }

        // --- SIDE WALLS ---
        for (int i = 0; i < 6; i++)
        {
            int next = (i + 1) % 6;

            int topCurrent = i;
            int topNext = next;
            int btmCurrent = i + 6;
            int btmNext = next + 6;

            // Triangle 1
            triangles[tIdx++] = topCurrent;
            triangles[tIdx++] = btmCurrent;
            triangles[tIdx++] = topNext;

            // Triangle 2
            triangles[tIdx++] = topNext;
            triangles[tIdx++] = btmCurrent;
            triangles[tIdx++] = btmNext;
        }

        // Assign arrays to Mesh data
        filterMesh.vertices = vertices;
        filterMesh.triangles = triangles;
        
        // Compute standard normal & boundary interactions
        filterMesh.RecalculateNormals();
        filterMesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = filterMesh;
        
        // Add or update an optional MeshCollider for selection tracking
        if (TryGetComponent<MeshCollider>(out MeshCollider mc))
        {
            mc.sharedMesh = filterMesh;
        }
    }
}