using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NoiseMapGeneration : MonoBehaviour
{
    public int width;
    public int height;
    public float scale;

    public void GenerateNoiseMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(width, height, scale);

        MapDisplay display = GetComponent<MapDisplay>();
        display.DrawNoiseMap(noiseMap);
    }
}
