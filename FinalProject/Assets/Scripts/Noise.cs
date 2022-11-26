using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float[,] GenerateNoiseMap(int width, int height, float scale = 0.00001f)
    {
        if (scale <= 0.0f) scale = 0.00001f;

        float[,] noiseMap = new float[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < height; x++)
            {
                float sX = x / scale;
                float sY = y / scale;

                float pValue = Mathf.PerlinNoise(sX, sY);
                noiseMap[x, y] = pValue;
            }
        }

        return noiseMap;
    }
}
