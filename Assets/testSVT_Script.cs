using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testSVT_Script : MonoBehaviour
{
    Vector3[] newVertices;
    Vector2[] newUV;
    int[] newTriangles;
    public MeshCollider meshCollider;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        //Mesh mesh = GetComponent<MeshFilter>().mesh;  //w przypadku tej wersji mesh robi sie czarny :d ... nie wiem dlaczego
        mesh.Clear();
        changeVertices();
        changeTriangles();
        mesh.vertices = newVertices;
        mesh.uv = newUV;
        mesh.triangles = newTriangles;
        meshCollider.sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void changeVertices()
    {
        newVertices = new Vector3[3];
        newVertices[0] = new Vector3(0, 0, 0);
        newVertices[1] = new Vector3(0, 0, 10);
        newVertices[2] = new Vector3(10, 0, 0);
    }

    void changeTriangles()
    {
        newTriangles = new int[3];
        newTriangles[0] = 0;
        newTriangles[1] = 1;
        newTriangles[2] = 2;
    }
}
