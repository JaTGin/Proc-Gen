using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Struct to hold altitude data on differing types of terrain
/// </summary>
[System.Serializable]
public struct Biome {
    public string name;
    public float altitude;
    public float humidity;
    public Color color;
    public GameObject[] flora;
}

/// <summary>
/// Builds terrain based on settings 
/// </summary>
public class NoiseMapGeneration : MonoBehaviour
{
    public int width; // Width of the mesh
    public int height; // Height of the mesh
    public float scale; // Scale of the noise
    public int octaves; // Calculates noise stats based on number of octaves
    public float persistence; // Lower persistence causes better grouping of similar heights- allows for mountains and lakes
    public float lacunarity; // Controls frequency of 'gaps' in noise
    public int seed; // Settable seed for random generation
    public Vector2 offset; // Changes position on the noise map that is being drawn
    public float heightMultiplier; // Scale of height at which mountains are allowed to be drawn
    public bool drawColor; // Are we drawing color or noise?
    public bool drawMesh; // Are we drawing the mesh, or just the 2D map?
    public bool loadFlora; // Are we loading flora?
    public AnimationCurve meshHeightCurve; // Curves mountains to be higher and oceans to be roughly the same height
    public Biome[] biomes; // The biomes- name, color, and height and humidity descriptors
    public LayerMask meshLayer;
    private float[,] noiseMap;

    void Start()
    {
        GenerateNoiseMap();
        if (loadFlora) FloraGenerator.DeployFlora(noiseMap, biomes, meshLayer);
    }

    /// <summary>
    /// Build the colored map and assemble the mesh
    /// </summary>
    public void GenerateNoiseMap()
    {
        // Get the black and white noise
        noiseMap = Noise.GenerateNoiseMap(width, height, scale, octaves, persistence, lacunarity, seed, offset);

        // Setup new color map
        Color[] colorMap = new Color[width * height];
        // Debug.Log(width + " | " + height + " | mapgen");
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Get the current height and assemble the biome accordingly
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < biomes.Length; i++)
                {
                    if (currentHeight <= biomes[i].altitude)
                    {
                        colorMap[y * width + x] = biomes[i].color;
                        
                        break;
                    }
                }
            }
        }

        // Assemble a texture and mesh from the maps we already have and draw them
        MapDisplay display = GetComponent<MapDisplay>();
        // TextureMapper mapper = GetComponent<TextureMapper>();
        if (drawMesh) display.DrawMesh(TerrainGenerator.GenerateTerrain(noiseMap, heightMultiplier, meshHeightCurve), TextureMapper.MapColorTexture(colorMap, width, height));
        else
        {
            if (!drawColor) display.DrawTexture(TextureMapper.MapTextureFromNoise(noiseMap));
            else display.DrawTexture(TextureMapper.MapColorTexture(colorMap, width, height));
        }
    }
}
