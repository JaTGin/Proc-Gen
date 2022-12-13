using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds a 3D mesh from our heightmap and settings
/// </summary>
public static class TerrainGenerator
{
    /// <summary>
    /// This method assembles a mesh based on our map
    /// </summary>
    /// <param name="noiseMap">The perlin noise we generated</param>
    /// <param name="heightMultiplier">Determines the strength of the mountains we're generating</param>
    /// <param name="heightCurve">Evaluates the noise map to make mountains higher and oceans flatter</param>
    /// <returns>a data object used to build a mesh</returns>
    public static MeshData GenerateTerrain(float[,] noiseMap, float heightMultiplier, AnimationCurve heightCurve)
    {
        // Get width and height of the map
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        // Offsets from the top left corner
        float tlx = (width - 1) / -2f;
        float tlz = (height - 1) / 2f;

        // Return data
        MeshData meshData = new MeshData(width, height);
        // The current index of the mesh- we store verts in a 1D array for readability
        int meshDataIndex = 0;

        // Iterate through each point in our perlin noise map
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculate the location of this point's corresponding vertex
                meshData.verts[meshDataIndex] = new Vector3(tlx + x, heightCurve.Evaluate(noiseMap[x, y]) * heightMultiplier, tlz - y);
                // Also calculate UVs to map our texture to this mesh later
                meshData.uvs[meshDataIndex] = new Vector2(x / (float)width, y / (float)height);

                // Build the two triangles that make up a square on our mesh grid- remember, all meshes are made of tris
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTri(meshDataIndex, meshDataIndex + width + 1, meshDataIndex + width);
                    meshData.AddTri(meshDataIndex + width + 1, meshDataIndex, meshDataIndex + 1);
                }

                // Increment the mesh data index
                meshDataIndex++;
            }
        }

        // Return the mesh we built
        return meshData;
    }
}

/// <summary>
/// Class to store data relating to our mesh
/// </summary>
public class MeshData
{
    public Vector3[] verts; // Vertices
    public int[] tris; // Triangles (groups of 3 verts)
    public Vector2[] uvs; // UV coordinates

    int triCount; // The number of tris in our mesh
    
    // Ctor
    public MeshData(int width, int height)
    {
        verts = new Vector3[width * height];
        tris = new int[(width - 1) * (height - 1) * 6];
        uvs = new Vector2[width * height];
    }

    /// <summary>
    /// Adds a triangle to the mesh
    /// </summary>
    /// <param name="a">point</param>
    /// <param name="b">point</param>
    /// <param name="c">point</param>
    public void AddTri(int a, int b, int c)
    {
        tris[triCount] = a;
        tris[triCount + 1] = b;
        tris[triCount + 2] = c;
        triCount += 3;
    }

    /// <summary>
    /// Builds a mesh object from our data
    /// </summary>
    /// <returns>The object we're building</returns>
    public Mesh BuildMesh()
    {
        // Create the object, set its values and return
        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        
        GameObject gameMesh = GameObject.Find("Mesh");
        GameObject.Destroy(gameMesh.GetComponent<MeshCollider>());
        gameMesh.AddComponent<MeshCollider>();
        gameMesh.GetComponent<MeshCollider>().sharedMesh = mesh;
        return mesh;
    }
}