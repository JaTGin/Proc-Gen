using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate Perlin noise and map it to a 2D array
/// </summary>
public class Noise
{
    /// <summary>
    /// Build the actual noise map
    /// </summary>
    /// <param name="width">width</param>
    /// <param name="height">height</param>
    /// <param name="scale">scale- how zoomed in to the map are we?</param>
    /// <param name="octaves">how detailed should the noise be?</param>
    /// <param name="persistence">How muich should each octave contribute to the structure of the noise map?</param>
    /// <param name="lacunarity">How quickly should frequency increase for each octave?</param>
    /// <param name="seed">The random seed for random generation</param>
    /// <param name="offset">Offset on the noise map to read from</param>
    /// <returns> 2D array of height information</returns>
    public static float[,] GenerateNoiseMap(int width, int height, float scale, int octaves, float persistence, float lacunarity, int seed, Vector2 offset)
    {
        // Set up RNG and offsets
        System.Random rng = new System.Random(seed);

        // Each octave is randomly sampled from a different point on the map
        Vector2[] offsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            float oX = rng.Next(-99999, 99999) + offset.x;
            float oY = rng.Next(-99999, 99999) + offset.y;
            offsets[i] = new Vector2(oX, oY);
        }

        // Ensure that scale can never be 0
        if (scale <= 0.0f) scale = 0.00001f;

        // Build the return data and get min and max noise values
        float[,] noiseMap = new float[width, height];
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // Loop through the map
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1f, frequency = 1f, noiseHeight = 0f;

                // For each octave, get perlin noise value based on scaled X and Y
                // Then multiply noise height by amplitude and recalculate amplitude and frequency
                // Amplitude: difference between minimum and maximum values the function could have
                for (int i = 0; i < octaves; i++)
                {
                    float sX = (x - width / 2) / scale * frequency + offsets[i].x;
                    float sY = (y - height / 2) / scale * frequency + offsets[i].y;

                    float pValue = Mathf.PerlinNoise(sX, sY) * 2 - 1;

                    noiseHeight += pValue * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }
                // Set max and min, then ass the point ot the noise map
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize the noise map to ensure it remains between 0 and 1
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        // Return the noise map
        return noiseMap;
    }
}
