using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SVTterrain is terrain made of SVT-chunks. Class contains 
/// </summary>
public class SVTterrain : MonoBehaviour
{
    public Transform characterRef;                                  // reference to character
    public GameObject chunkPrefab;                                  // reference to prefab with ChunkSVT script
    public List<GameObject> terrainChunks = new List<GameObject>(); // list of all created chunks
    private Vector2Int lastChunkPlayerLocation;                     // last player location described in chunks
    private bool lastChunkPlayerLocationNotInitialized = true;      // is "player location described in chunk" not initialized

    /// <summary>
    /// Always executes when player changes chunk position
    /// </summary>
    public void CheckAndCreateChunks(Vector2Int playerOnChunkLocation)
    {
        // Make first chunks within draw distance
        int xIndex = playerOnChunkLocation.x, yIndex = playerOnChunkLocation.y;
        for (int x = xIndex - MainScript.DrawDistance; x < xIndex + MainScript.DrawDistance + 1; x++)
            for (int y = yIndex - MainScript.DrawDistance; y < yIndex + MainScript.DrawDistance + 1; y++)
            {
                // Check if chunk already exist ...
                bool chunkExist = false;
                for (int i = 0; i < terrainChunks.Count; i++)
                    if ((terrainChunks[i].GetComponent<ChunkSVT>()).xIndex == x && (terrainChunks[i].GetComponent<ChunkSVT>()).yIndex == y)
                        chunkExist = true;

                // ... if not, create it
                if (chunkExist == false)
                {
                    terrainChunks.Add((Instantiate(chunkPrefab) as GameObject));
                    (terrainChunks[terrainChunks.Count-1].GetComponent<ChunkSVT>()).ChunkInit(this, transform.position, x, y);
                }
            }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>    
    void Start()
    {
        
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Chunks Update() method execution
        for(int i = 0; i < terrainChunks.Count; i++)  // OPTIMIZE - when game will generate 100000 chunks, that loop will executes 100000 times - it should execute on LOADED chunks !!!
        {
            (terrainChunks[i].GetComponent<ChunkSVT>()).Update();
        }

        // Calculate player chunk position
        Vector3 playerWorldLocation = characterRef.transform.position;
        Vector3 terrainWorldLocation = transform.position;
        Vector3 playerRelativeLocation = playerWorldLocation - terrainWorldLocation;
        Vector2Int playerOnChunkLocation = new Vector2Int((int)(playerRelativeLocation.x / ChunkSVT.gridcellWidth / 16), (int)(playerRelativeLocation.z / ChunkSVT.gridcellWidth / 16));
        if (playerRelativeLocation.x < 0)
            playerOnChunkLocation += new Vector2Int(-1, 0);
        if (playerRelativeLocation.z < 0)
            playerOnChunkLocation += new Vector2Int(0, -1);
        //Debug.Log(playerOnChunkLocation);

        // Check and update chunks if player changed chunk position
        if(lastChunkPlayerLocationNotInitialized == true || playerOnChunkLocation != lastChunkPlayerLocation)
        {
            lastChunkPlayerLocationNotInitialized = false;
            lastChunkPlayerLocation = playerOnChunkLocation;
            CheckAndCreateChunks(playerOnChunkLocation);
        }
    }
}
