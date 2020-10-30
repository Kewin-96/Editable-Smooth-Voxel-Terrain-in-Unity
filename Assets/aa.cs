using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aa : MonoBehaviour
{
    public MeshFilter meshRenderer;
    public MeshCollider meshCollider;
    public Mesh a, b;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.A))
        {
            meshRenderer.mesh = a;
            meshCollider.sharedMesh = a;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            meshRenderer.mesh = b;
            meshCollider.sharedMesh = b;
        }
    }
}
