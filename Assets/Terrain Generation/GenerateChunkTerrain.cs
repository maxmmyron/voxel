using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GenerateChunkTerrain : MonoBehaviour
{
    public static int chunkSize;
    
    [SerializeField]
    private Material material;

    public int[,,] voxelPoints = new int[chunkSize+2, chunkSize+2, chunkSize+2];

    private void Start()
    {
        gameObject.transform.parent = GameObject.Find("Terrain").transform;
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }

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
}
