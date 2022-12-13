using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds helper classes for rendering the map
/// </summary>
public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    /// <summary>
    /// Draw a texture
    /// </summary>
    /// <param name="texture">The texture to draw</param>
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
    }

    /// <summary>
    /// Draw a mesh
    /// </summary>
    /// <param name="meshData">The mesh to draw</param>
    /// <param name="texture">The texture to draw to the mesh</param>
    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.BuildMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
