using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateChunkTerrain : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    public int chunkSize;

    [SerializeField]
    private static float threshold = 0.5f;

    [SerializeField]
    private static float scale = 0.05f;

    [SerializeField]
    private static int seed = 1;

    public Vector3 offset;

    private void Start()
    {
        
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    
                    if (Perlin3D((x + seed + offset.x) * scale, (y + seed + offset.y) * scale, (z + seed + offset.z) * scale) >= threshold)
                    {
                        Instantiate(prefab, new Vector3(x + offset.x, y + offset.y, z + offset.z), Quaternion.identity);
                    }
                }
            }
        }
    }

    public static float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
}
