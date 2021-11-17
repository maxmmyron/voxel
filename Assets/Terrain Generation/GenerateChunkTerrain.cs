using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateChunkTerrain : MonoBehaviour
{
    public static int chunkSize;

    //[SerializeField]
    //private float threshold = 0.0f;

    //[SerializeField]
    //private float scale = 0.05f;
    
    [SerializeField]
    private Material material;

    //public Vector3 noiseOffset;

    public int[,,] voxelPoints = new int[chunkSize+2, chunkSize+2, chunkSize+2];

    private void Start()
    {
        gameObject.transform.parent = GameObject.Find("Terrain").transform;
        gameObject.layer = LayerMask.NameToLayer("Ground");

        //GenerateNoisePoints(voxelPoints);

        //BuildMesh(voxelPoints);
    }

    // Generates a 3D array of block types, where 0 is ground and 1 is air. 
    /*private void GenerateNoisePoints(int[,,] pointArray)
    {
        for(int x = 0; x < pointArray.GetLength(0); x++)
            for(int y = 0; y < pointArray.GetLength(1); y++)
                for(int z = 0; z < pointArray.GetLength(2); z++)
                {
                    float noiseValue = noise.GetSimplex((x + gameObject.transform.position.x) * scale, (y + gameObject.transform.position.y) * scale, (z + gameObject.transform.position.z) * scale);
                    pointArray[x, y, z] = noiseValue >= threshold ? 0 : 1;
                }
    }*/

    public void BuildMesh(int[,,] pointArray)
    {
        Mesh chunkMesh = new Mesh();

        List<Vector3> meshVertices = new List<Vector3>();
        List<int> meshTriangles = new List<int>();

        for (int x = 1; x < pointArray.GetLength(0) - 1; x++)
            for (int z = 1; z < pointArray.GetLength(2) - 1; z++)
                for (int y = 1; y < pointArray.GetLength(1) - 1; y++)
                {
                    if (pointArray[x, y, z] != 1)
                    {
                        Vector3 voxelPos = new Vector3(x - 1, y, z - 1);

                        int numFaces = 0;
                        //no land above, build top face
                        if (y < pointArray.GetLength(1) - 1 && pointArray[x, y + 1, z] == 1)
                        {
                            meshVertices.Add(voxelPos + new Vector3(0, 1, 0));
                            meshVertices.Add(voxelPos + new Vector3(0, 1, 1));
                            meshVertices.Add(voxelPos + new Vector3(1, 1, 1));
                            meshVertices.Add(voxelPos + new Vector3(1, 1, 0));
                            numFaces++;
                        }

                        //bottom
                        if (y > 0 && pointArray[x, y - 1, z] == 1)
                        {
                            meshVertices.Add(voxelPos + new Vector3(0, 0, 0));
                            meshVertices.Add(voxelPos + new Vector3(1, 0, 0));
                            meshVertices.Add(voxelPos + new Vector3(1, 0, 1));
                            meshVertices.Add(voxelPos + new Vector3(0, 0, 1));
                            numFaces++;
                        }

                        //front
                        if (pointArray[x, y, z - 1] == 1)
                        {
                            meshVertices.Add(voxelPos + new Vector3(0, 0, 0));
                            meshVertices.Add(voxelPos + new Vector3(0, 1, 0));
                            meshVertices.Add(voxelPos + new Vector3(1, 1, 0));
                            meshVertices.Add(voxelPos + new Vector3(1, 0, 0));
                            numFaces++;
                        }

                        //right
                        if (pointArray[x + 1, y, z] == 1)
                        {
                            meshVertices.Add(voxelPos + new Vector3(1, 0, 0));
                            meshVertices.Add(voxelPos + new Vector3(1, 1, 0));
                            meshVertices.Add(voxelPos + new Vector3(1, 1, 1));
                            meshVertices.Add(voxelPos + new Vector3(1, 0, 1));
                            numFaces++;
                        }

                        //back
                        if (pointArray[x, y, z + 1] == 1)
                        {
                            meshVertices.Add(voxelPos + new Vector3(1, 0, 1));
                            meshVertices.Add(voxelPos + new Vector3(1, 1, 1));
                            meshVertices.Add(voxelPos + new Vector3(0, 1, 1));
                            meshVertices.Add(voxelPos + new Vector3(0, 0, 1));
                            numFaces++;
                        }

                        //left
                        if (pointArray[x - 1, y, z] == 1)
                        {
                            meshVertices.Add(voxelPos + new Vector3(0, 0, 1));
                            meshVertices.Add(voxelPos + new Vector3(0, 1, 1));
                            meshVertices.Add(voxelPos + new Vector3(0, 1, 0));
                            meshVertices.Add(voxelPos + new Vector3(0, 0, 0));
                            numFaces++;
                        }


                        int tl = meshVertices.Count - 4 * numFaces;
                        for (int i = 0; i < numFaces; i++)
                            meshTriangles.AddRange(new int[] { tl + i * 4, tl + i * 4 + 1, tl + i * 4 + 2, tl + i * 4, tl + i * 4 + 2, tl + i * 4 + 3 });
                    }
                }
        chunkMesh.vertices = meshVertices.ToArray();
        chunkMesh.triangles = meshTriangles.ToArray();

        chunkMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshFilter>().sharedMesh = chunkMesh;

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;

        GetComponent<MeshRenderer>().material = material;
    }

    /*public static float Fast3D(float x, float y, float z)
    {
        float ab = noise.GetSimplex(x, y);
        float bc = noise.GetSimplex(y, z);
        float ac = noise.GetSimplex(x, z);

        float ba = noise.GetSimplex(y, x);
        float cb = noise.GetSimplex(z, y);
        float ca = noise.GetSimplex(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }*/
}
