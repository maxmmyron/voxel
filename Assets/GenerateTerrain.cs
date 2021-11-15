using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [SerializeField]
    private GameObject Chunk;

    [SerializeField]
    private int terrainSize = 2;

    [SerializeField]
    private int chunkSize = 2;

    // Start is called before the first frame update
    private void Start()
    {
        for (int x = 0; x < terrainSize; x++)
        {
            for (int y = 0; y < terrainSize; y++)
            {
                for (int z = 0; z < terrainSize; z++)
                {
                    GameObject chunkPrefab = Instantiate(Chunk) as GameObject;
                    chunkPrefab.transform.position = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                    chunkPrefab.GetComponent<GenerateChunkTerrain>().chunkSize = chunkSize;
                    chunkPrefab.GetComponent<GenerateChunkTerrain>().offset = new Vector3(x * chunkSize, y * chunkSize, z * chunkSize);
                }
            }
        }
    }
}
