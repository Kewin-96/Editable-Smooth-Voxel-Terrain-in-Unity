using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk_SVT : MonoBehaviour
{
    public float[][][] voxels;                  // voxels value in 3D array
    public Gridcell[][][] grid;                 // Grid [x][y][z] - y is vertical axis !!!
    public Vector3[] vertices;                  // all vertices of all triangles of chunk (every 3 continously vertices is 1 triangle)
    public Vector2[] UV;                        // UV - empty for now
    public int[] verticesIndexes;               // indexes of all vertices (every 3 continously vertices is 1 triangle)
    public MeshCollider meshCollider;           // mesh collider
    /// <summary>
    /// [0] [1] [2]
    /// [3] [-] [4]
    /// [5] [6] [7]
    /// </summary>
    public Chunk_SVT[] neighborhood;

    // Start is called before the first frame update
    void Start()
    {
        // initialization of voxels
        voxels = new float[SVT.chunkWidth][][];
        for (int i = 0; i < SVT.chunkWidth; i++)
        {
            voxels[i] = new float[SVT.chunkHeight][];
            for (int j = 0; j < SVT.chunkHeight; j++)
                voxels[i][j] = new float[SVT.chunkWidth];
        }

        // initialization of grid
        grid = new Gridcell[SVT.chunkWidth][][];
        for (int i = 0; i < SVT.chunkWidth; i++)
        {
            grid[i] = new Gridcell[SVT.chunkHeight][];
            for (int j = 0; j < SVT.chunkHeight; j++)
            {
                grid[i][j] = new Gridcell[SVT.chunkWidth];
                for (int k = 0; k < SVT.chunkWidth; k++)
                {
                    /*ref float a = ref grid[i][j][k].val[0];
                    grid[i][j][k].val[0] = voxels[i][j][k];*/
                    /*grid[i][j][k].val[] = voxels[i][j][k];
                    grid[i][j][k].val[] = voxels[i][j][k];
                    grid[i][j][k].val[] = voxels[i][j][k];
                    grid[i][j][k].val[] = voxels[i][j][k];
                    grid[i][j][k].val[] = voxels[i][j][k];
                    grid[i][j][k].val[] = voxels[i][j][k];
                    grid[i][j][k].val[] = voxels[i][j][k];
                    grid[i][j][k].val[] = voxels[i][j][k];*/
                }
            }
        }

        // ************************
        // ***** DEBUG - TEST *****
        // ************************
        Gridcell g_test = new Gridcell();
        g_test.p[0] = new Vector3(0, 0, 0);
        g_test.p[1] = new Vector3(10, 0, 0);
        g_test.p[2] = new Vector3(10, 10, 0);
        g_test.p[3] = new Vector3(0, 10, 0);
        g_test.p[4] = new Vector3(0, 0, 10);
        g_test.p[5] = new Vector3(10, 0, 10);
        g_test.p[6] = new Vector3(10, 10, 10);
        g_test.p[7] = new Vector3(0, 10, 10);
        g_test.val[0] = 255;
        g_test.val[1] = 255;
        g_test.val[2] = 255;
        g_test.val[3] = 255;
        g_test.val[4] = 150;
        g_test.val[5] = 130;
        g_test.val[6] = 110;
        g_test.val[7] = 80;
        Triangle[] triangles_test = SVT.PolygoniseCube(g_test, 128);

        // "Throwing" array of triangles of 1 gridcell into mesh
        //   mesh object
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;         // setting mesh for game object
        //Mesh mesh = GetComponent<MeshFilter>().mesh;  // w przypadku tej wersji mesh robi sie czarny :d ... nie wiem dlaczego
        mesh.Clear();

        //   all vertices of all triangles
        vertices = new Vector3[triangles_test.Length * 3];
        for (int i = 0; i < triangles_test.Length; i++)
        {
            vertices[i * 3] = triangles_test[i].p[0];
            vertices[i * 3 + 1] = triangles_test[i].p[1];
            vertices[i * 3 + 2] = triangles_test[i].p[2];
        }

        //    indexes of each vertice
        verticesIndexes = new int[triangles_test.Length * 3];
        for (int i = 0; i < triangles_test.Length * 3; i++)
            verticesIndexes[i] = i;

        mesh.vertices = vertices;               // all vertices of all triagles
        mesh.uv = UV;                           // empty array
        mesh.triangles = verticesIndexes;       // indexes of each vertices
        meshCollider.sharedMesh = mesh;         // mesh collider
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
