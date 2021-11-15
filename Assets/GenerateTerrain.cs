using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [SerializeField]
    private GameObject Chunk;

    [SerializeField]
    private int terrainSize = 2;

    private void Start()
    {
        for (int x = 0; x < terrainSize; x++)
        {
            for (int y = 0; y < terrainSize; y++)
            {
                for (int z = 0; z < terrainSize; z++)
                {
                    GameObject chunkPrefab = Instantiate(Chunk) as GameObject;
                    chunkPrefab.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }
}
