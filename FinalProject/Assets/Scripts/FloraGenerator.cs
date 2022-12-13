using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FloraGenerator
{
    

    public static void DeployFlora(float[,] noiseMap, Biome[] biomes, LayerMask target)
    {
        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);
        float tlx = (width) / -2f;
        float tlz = (height) / -2f;

        // List<GameObject> output = new List<GameObject>();
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
                        if (biomes[i].flora.Length > 0)
                        {
                            Vector3 position =
                                new Vector3(
                                    (x + width / -2f) * 10,
                                    0f,
                                    (y + height / -2f) * -10);
                            // Debug.Log(position + " | " + biomes[i].name);
                            int count = Random.Range(1, (int)(biomes[i].humidity / 10));
                            for (int j = 0; j < count; j++)
                            {
                                Vector3 posInSquare =
                                new Vector3(
                                    Random.Range(position.x - 5, position.x + 5),
                                    1000f,
                                    Random.Range(position.z - 5, position.z + 5));
                                // Debug.Log(posInSquare);
                                Ray ray = new Ray(posInSquare, Vector3.down);
                                // Debug.DrawRay(posInSquare, Vector3.down * 1000f, Color.red, 200, false);
                                RaycastHit hit = new RaycastHit();
                                // Debug.Log(LayerMask.NameToLayer("Mesh Layer"));
                                if (Physics.Raycast(ray, out hit, Mathf.Infinity))
                                {
                                    Debug.Log(hit.point);
                                    
                                    
                                    GameObject.Instantiate(
                                        biomes[i].flora[Random.Range(0, biomes[i].flora.Length)],
                                        hit.point,
                                        Quaternion.identity);
                                    
                                    // output.Add(plant);
                                }

                            }


                        }

                        break;
                    }
                }
            }
        }
        // return output;
    }
}
