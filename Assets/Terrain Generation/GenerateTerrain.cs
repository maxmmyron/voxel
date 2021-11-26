using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTerrain : MonoBehaviour
{
    [SerializeField]
    private GameObject chunk;

    [SerializeField]
    private Transform character;

    [SerializeField]
    private NoiseSettings noiseSettings;

    [SerializeField]
    private int chunkSize = 16;

    [SerializeField]
    private Vector3 noiseOffset;

    [SerializeField]
    private int renderDistance = 8;
    
    [SerializeField]
    private float secondsToWait = 0.2f;

    // keep a list of all generated chunks, so we can quickly save and load these instead of regenerating them
    public static Dictionary<Vector3, GenerateChunkTerrain> generatedChunks = new Dictionary<Vector3, GenerateChunkTerrain>();
    // keep a list of all currently rendered chunks
    public static Dictionary<Vector3, GenerateChunkTerrain> renderedChunks = new Dictionary<Vector3, GenerateChunkTerrain>();

    List<GenerateChunkTerrain> chunkPool = new List<GenerateChunkTerrain>();

    List<Vector3> ungeneratedChunks = new List<Vector3>();

    private static FastNoiseLite noise = new FastNoiseLite();

    private void Start()
    {
        GenerateChunkTerrain.chunkSize = chunkSize;
        GenerateChunks(true);
    }

    private void Update()
    {
        GenerateChunks();
    }

    // Build a chunk
    private void BuildChunk(Vector3 chunkPosition)
    {
        

        GameObject chunkGameObject = Instantiate(chunk, chunkPosition, Quaternion.identity); // instantiate a new chunk
        GenerateChunkTerrain chunkToBuild = chunkGameObject.GetComponent<GenerateChunkTerrain>();

        if (!generatedChunks.ContainsKey(chunkPosition))
        {
            for (int x = 0; x < chunkToBuild.voxelPoints.GetLength(0); x++)
                for (int y = 0; y < chunkToBuild.voxelPoints.GetLength(1); y++)
                    for (int z = 0; z < chunkToBuild.voxelPoints.GetLength(2); z++)
                        chunkToBuild.voxelPoints[x, y, z] = GetPointFromNoise(new Vector3(x, y, z), chunkPosition);

            chunkToBuild.BuildMesh(chunkToBuild.voxelPoints);

            generatedChunks.Add(chunkPosition, chunkToBuild);
        } else
        {
            chunkToBuild.BuildMesh(generatedChunks[chunkPosition].voxelPoints);
        }
        
        renderedChunks.Add(chunkPosition, chunkToBuild);
    }

    // Generate chunks
    Vector3 oldCharacterChunkPosition = new Vector3(-1, -1, -1);
    private void GenerateChunks(bool generateInstantly = false)
    {
        //get the position for the chunk the character is currently in
        Vector3 characterChunkPosition = new Vector3(
            Mathf.FloorToInt(character.position.x / chunkSize) * chunkSize,
            Mathf.FloorToInt(character.position.y / chunkSize) * chunkSize,
            Mathf.FloorToInt(character.position.z / chunkSize) * chunkSize);

        
        if(oldCharacterChunkPosition.x != characterChunkPosition.x || 
            oldCharacterChunkPosition.y != characterChunkPosition.y || 
            oldCharacterChunkPosition.z != characterChunkPosition.z) // if the player enters a new chunk
        {
            oldCharacterChunkPosition = characterChunkPosition;

            // iterate in a cubic region surrounding the player
            for (int x = (int)characterChunkPosition.x - chunkSize * renderDistance; x <= (int)characterChunkPosition.x + chunkSize * renderDistance; x += chunkSize)
                for (int y = (int)characterChunkPosition.y - chunkSize * renderDistance; y <= (int)characterChunkPosition.y + chunkSize * renderDistance; y += chunkSize)
                    for (int z = (int)characterChunkPosition.z - chunkSize * renderDistance; z <= (int)characterChunkPosition.z + chunkSize * renderDistance; z += chunkSize)
                    {
                        Vector3 chunkPosition = new Vector3(x, y, z);

                        // determine if chunks already exists or will exist
                        // if so, no need to generate
                        if(!renderedChunks.ContainsKey(chunkPosition) && !ungeneratedChunks.Contains(chunkPosition))
                        {
                            if (generateInstantly)
                                BuildChunk(chunkPosition);
                            else
                                ungeneratedChunks.Add(chunkPosition);
                        }
                    }

            // to remove chunks too far away
            List<Vector3> chunksToDestroy = new List<Vector3>();

            foreach(KeyValuePair<Vector3, GenerateChunkTerrain> chunkPair in renderedChunks)
            {
                Vector3 chunkPos = chunkPair.Key;

                // add all chunks outside of chunk render distance relative to player
                if(Mathf.Abs(characterChunkPosition.x - chunkPos.x) > chunkSize * (renderDistance + 3) || 
                    Mathf.Abs(characterChunkPosition.y - chunkPos.y) > chunkSize * (renderDistance + 3) || 
                    Mathf.Abs(characterChunkPosition.z - chunkPos.z) > chunkSize * (renderDistance + 3))
                        chunksToDestroy.Add(chunkPos);
            }

            //remove chunks that may be generated in the future
            foreach(Vector3 chunkPos in ungeneratedChunks)
            {
                // add all chunks outside of chunk render distance relative to player
                if (Mathf.Abs(characterChunkPosition.x - chunkPos.x) > chunkSize * (renderDistance + 1) || 
                    Mathf.Abs(characterChunkPosition.y - chunkPos.y) > chunkSize * (renderDistance + 1) || 
                    Mathf.Abs(characterChunkPosition.z - chunkPos.z) > chunkSize * (renderDistance + 1))
                        ungeneratedChunks.Remove(chunkPos);
            }

            // Remove chunks designated for removal
            foreach (Vector3 chunkPos in chunksToDestroy)
            {
                renderedChunks[chunkPos].gameObject.SetActive(false);
                renderedChunks.Remove(chunkPos);
            }

            StartCoroutine(DelayChunkGeneration());
        }
    }

    // Delay chunk generation while we build ungenerated chunks
    IEnumerator DelayChunkGeneration()
    {
        while(ungeneratedChunks.Count > 0)
        {
            BuildChunk(ungeneratedChunks[0]);
            ungeneratedChunks.RemoveAt(0);
        }

        yield return new WaitForSeconds(secondsToWait);
    }

    int GetPointFromNoise(Vector3 position, Vector3 chunkPosition)
    {
        // get an adjuested, absolute position based on position of voxel in chunk + chunk position
        Vector3 adjustedPosition = position + chunkPosition;

        // get factor & frequency values from noiseSettings, since we don't want to mutate the original values
        float factor = noiseSettings.factor;
        float frequency = noiseSettings.frequency;

        // set the threshold to half of the original frequency
        float threshold = noiseSettings.frequency * 0.5f;

        if (adjustedPosition.y < noiseSettings.noiseFloor && adjustedPosition.y >= noiseSettings.falloffFloor)
            threshold -= Mathf.Abs((noiseSettings.noiseFloor - adjustedPosition.y) * noiseSettings.falloff);
        if(adjustedPosition.y < noiseSettings.falloffFloor)
            threshold -= Mathf.Abs((noiseSettings.noiseFloor - noiseSettings.falloffFloor) * noiseSettings.falloff);

        // keep track of noise
        float noiseValue = 0f;

        // loop over octaves
        for(int i = 0; i < noiseSettings.octaves; i++)
        {
            // add noise value adjuested for frequency and factor
            noiseValue += noise.GetNoise(adjustedPosition.x * frequency, adjustedPosition.y * frequency, adjustedPosition.z * frequency) * factor;

            // adjust factor and frequency values based on persistance and roughness desired
            factor *= noiseSettings.persistance;
            frequency *= noiseSettings.roughness;
        }

        // return based on threshold
        return noiseValue >= threshold ? 0 : 1;

        // create a new ComputeBuffer with a size of the voxelPoints.
        // the "4" here references how each int is 32 bits, or 4 bytes.
    }
}
