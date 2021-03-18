using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVTterrain : MonoBehaviour
{
    public MeshCollider meshCollider;
    public Transform characterRef;
    public List<ChunkSVT> terrain;

    // Start is called before the first frame update
    void Start()
    {
        terrain = new List<ChunkSVT>();
        terrain.Add(new ChunkSVT(this, transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < terrain.Count; i++)  // OPTIMIZE - when game will generate 100000 chunks, that loop will executes 100000 times - it should execute on LOADED chunks !!!
        {
            terrain[i].Update();
        }
    }
}
