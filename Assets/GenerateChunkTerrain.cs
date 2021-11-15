using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateChunkTerrain : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab;

    public int chunkSize = 16;

    [SerializeField]
    private static float threshold = 0.5f;

    [SerializeField]
    private static float scale = 0.05f;

    public int offset;
    
    [SerializeField]
    private Material material;

    private List<Mesh> meshes = new List<Mesh>();

    private void Start()
    {
        gameObject.transform.position *= chunkSize; // transform position based on chunksize

        float startTime = Time.realtimeSinceStartup; // Debug

        #region Instantiate Chunk Terrain

        List<CombineInstance> voxelData = new List<CombineInstance>(); // create a list to describe all of the prefab meshes to be combined together later

        MeshFilter voxelMesh = Instantiate(prefab, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>(); // Create a single prefab and store only the mesh from it
        
        // loop through the entire chunk
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    float noiseValue = Perlin3D((x + offset + gameObject.transform.position.x) * scale, (y + offset + gameObject.transform.position.y) * scale, (z + offset + gameObject.transform.position.z) * scale);
                    if (noiseValue >= threshold)
                    {
                        voxelMesh.transform.position = new Vector3(x + gameObject.transform.position.x, y + gameObject.transform.position.y, z + gameObject.transform.position.z);
                        //voxelMesh.sharedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

                        CombineInstance combineInstance = new CombineInstance
                        {
                            mesh = voxelMesh.sharedMesh,
                            transform = voxelMesh.transform.localToWorldMatrix,
                        }; // copy the mesh and transform data off of the prefab voxel

                        voxelData.Add(combineInstance); // add the voxel data to the voxelData list
                    }
                }
            }
        }

        Destroy(voxelMesh.gameObject); // remove the original voxel prefab

        #endregion

        #region Seperate Chunk Data

        List<List<CombineInstance>> voxelDataLists = new List<List<CombineInstance>>(); // create a list of lists to store 

        int vertexCount = 0;

        voxelDataLists.Add(new List<CombineInstance>());
        for(int i = 0; i < voxelData.Count; i++)
        {
            vertexCount += voxelData[i].mesh.vertexCount;
            if(vertexCount > 65536) // if > 65536, then we need to add all these verticies to a mesh and make a new one
            {
                vertexCount = 0;
                voxelDataLists.Add(new List<CombineInstance>());
                i--;
            } else
            {
                voxelDataLists.Last().Add(voxelData[i]);
            }
        }

        #endregion

        #region Create Chunk Mesh

        gameObject.transform.SetParent(GameObject.Find("Terrain").transform); // set parent to Terrain object

        if(gameObject.transform.childCount > 0)
            GameObject.Destroy(gameObject.transform.GetChild(0).gameObject); // remove the extra child mesh from the original chunk created, as other chunks are instantiated from that one

        foreach (List<CombineInstance> data in voxelDataLists)
        {

            GameObject chunkMesh = new GameObject("ChunkMesh"); // Create a new mesh gameobject

            chunkMesh.layer = LayerMask.NameToLayer("Ground"); // set chunkmesh layer to groundMask Layer to allow for character controller collision checking

            chunkMesh.transform.parent = gameObject.transform; // set parent to this current Chunk

            MeshFilter meshFilter = chunkMesh.AddComponent<MeshFilter>(); // add mesh filter
            MeshRenderer meshRenderer = chunkMesh.AddComponent<MeshRenderer>(); // add mesh renderer

            meshRenderer.material = material; // set material

            meshFilter.mesh.CombineMeshes(data.ToArray()); // set mesh to voxelData

            meshes.Add(meshFilter.mesh); // add mesh 

            chunkMesh.AddComponent<MeshCollider>().sharedMesh = meshFilter.sharedMesh; // create a collider for the mesh
        }

        #endregion

        Debug.Log("Load time: " + (Time.realtimeSinceStartup - startTime) + "s");

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
