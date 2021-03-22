using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SVTterrain : MonoBehaviour
{
    public Transform characterRef;
    public GameObject chunkPrefab;
    public List<GameObject> terrainGO;
    public Vector2Int lastChunkPlayerLocation;
    public bool lastChunkPlayerLocationNotInitialized = true;

    /// <summary>
    /// Always executes when player changed chunk position
    /// </summary>
    public void CheckAndCreateChunks(Vector2Int playerOnChunkLocation)
    {
        // Make first chunks within draw distance
        int xIndex = playerOnChunkLocation.x, yIndex = playerOnChunkLocation.y;
        for (int x = xIndex - MainScript.drawDistance; x < xIndex + MainScript.drawDistance + 1; x++)
            for (int y = yIndex - MainScript.drawDistance; y < yIndex + MainScript.drawDistance + 1; y++)
            {
                // Check current indexed chunk if it exist already ...
                bool chunkExist = false;
                for (int i = 0; i < terrainGO.Count; i++)
                    if ((terrainGO[i].GetComponent<ChunkSVT>()).xIndex == x && (terrainGO[i].GetComponent<ChunkSVT>()).yIndex == y)
                        chunkExist = true;

                // ... if not, create it
                if (chunkExist == false)
                {
                    terrainGO.Add((Instantiate(chunkPrefab) as GameObject));
                    (terrainGO[terrainGO.Count-1].GetComponent<ChunkSVT>()).ChunkPreInit(this, transform.position, x, y);
                }
            }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>    
    void Start()
    {
        terrainGO = new List<GameObject>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Chunks Update() method execution
        for(int i = 0; i < terrainGO.Count; i++)  // OPTIMIZE - when game will generate 100000 chunks, that loop will executes 100000 times - it should execute on LOADED chunks !!!
        {
            (terrainGO[i].GetComponent<ChunkSVT>()).Update();
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
