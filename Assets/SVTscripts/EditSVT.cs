using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditSVT : MonoBehaviour
{
    public static void DecreaseVoxel(float change, ChunkSVT chunk, Vector3 hitWorldLoc)
    {
        // relative location of hit
        Vector3 relativeLocation = hitWorldLoc - chunk.transform.position;

        // indexes of aproximated voxel
        int ix = (int)Mathf.Round(relativeLocation.x / (float)ChunkSVT.gridcellWidth);
        int iy = (int)Mathf.Round(relativeLocation.y / (float)ChunkSVT.gridcellWidth);
        int iz = (int)Mathf.Round(relativeLocation.z / (float)ChunkSVT.gridcellWidth);

        // voxel change
        chunk.voxels[ix][iy][iz] += change;
        if (chunk.voxels[ix][iy][iz] > 255)
            chunk.voxels[ix][iy][iz] = 255;
        if (chunk.voxels[ix][iy][iz] < 0)
            chunk.voxels[ix][iy][iz] = 0;

        // update SVT [UNOPTIMIZED!!!]
        chunk.ConvertVoxelsToSVT();
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
        
    }
}
