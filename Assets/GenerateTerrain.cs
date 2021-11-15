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
    private int offset = 1;

    private int oldOffset;

    private void Generate()
    {
        for (int x = 0; x < terrainSize; x++)
        {
            for (int y = 0; y < terrainSize; y++)
            {
                for (int z = 0; z < terrainSize; z++)
                {
                    GameObject chunkPrefab = Instantiate(Chunk) as GameObject;
                    chunkPrefab.transform.position = new Vector3(x, y, z);
                    chunkPrefab.GetComponent<GenerateChunkTerrain>().offset = offset;
                }
            }
        }
    }

    private void Start()
    {
        oldOffset = offset;
        Generate();
    }

    
    private void Update()
    {
        if (oldOffset != offset)
        {
            for (int i = 1; i < gameObject.transform.childCount; i++)
                GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
            Generate();
            oldOffset = offset;
        }
    }
}
