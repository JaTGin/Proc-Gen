using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This class builds Texture2D objects from maps generated elsewhere.
/// </summary>
public static class TextureMapper
{
    /// <summary>
    /// Generates a colored texture2D object and returns it
    /// </summary>
    /// <param name="colors">The color map</param>
    /// <param name="width">Width of the plane</param>
    /// <param name="height">Height of the plane</param>
    /// <returns>A 2D texture with colors representing differing environments on the map</returns>
    public static Texture2D MapColorTexture(Color[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
    
    /// <summary>
    /// Creates a 2D texture from a perlin noise map
    /// </summary>
    /// <param name="noiseMap">The noisemap</param>
    /// <returns>A usable 2D texture of our noise</returns>
    public static Texture2D MapTextureFromNoise(float[,] noiseMap)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        // black-and white map to be returned
        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Lerp between black and white based on the noise value to determine color
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }

        return MapColorTexture(colorMap, width, height);
    }
}
