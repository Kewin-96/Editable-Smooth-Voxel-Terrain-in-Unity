using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  This static class cointains methods used to editing SVT terrain when project is running
/// </summary>
public static class EditSVT
{
    /// <summary>
    /// Test method - increases or decreases value of 1 voxel. Takes the nearest voxel relative to the hit location.
    /// </summary>
    /// <param name="change">by how much the value is to be changed</param>
    /// <param name="chunk">reference to hit chunk</param>
    /// <param name="hitWorldLoc">world location of hit</param>
    public static void ChangeVoxelValue(float change, ChunkSVT chunk, Vector3 hitWorldLoc)
    {
        // relative location of hit
        Vector3 relativeLocation = hitWorldLoc - chunk.transform.position;

        // indexes of aproximated voxel
        int ix = (int)Mathf.Round(relativeLocation.x / ChunkSVT.gridcellWidth);
        int iy = (int)Mathf.Round(relativeLocation.y / ChunkSVT.gridcellWidth);
        int iz = (int)Mathf.Round(relativeLocation.z / ChunkSVT.gridcellWidth);

        // check point voxel-location
        if (ix < 0)
            ix = 0;
        if (ix > ChunkSVT.chunkWidthGC)
            ix = ChunkSVT.chunkWidthGC;
        if (iy < 0)
            iy = 0;
        if (iy >= ChunkSVT.chunkHeightGC)
            iy = ChunkSVT.chunkHeightGC - 1;
        if (iz < 0)
            iz = 0;
        if (iz > ChunkSVT.chunkWidthGC)
            iz = ChunkSVT.chunkWidthGC;

        Debug.Log("relativeLocation = " + relativeLocation);
        Debug.Log(ix + ", " + iy + ", " + iz);

        // voxel change
        if (ix != ChunkSVT.chunkWidthGC && iz != ChunkSVT.chunkWidthGC )
        {
            chunk.voxels[ix][iy][iz] += change;
            if (chunk.voxels[ix][iy][iz] > ChunkSVT.maxVoxelValue)
                chunk.voxels[ix][iy][iz] = ChunkSVT.maxVoxelValue;
            if (chunk.voxels[ix][iy][iz] < 0)
                chunk.voxels[ix][iy][iz] = 0;
        }
        else if (ix != ChunkSVT.chunkWidthGC && iz == ChunkSVT.chunkWidthGC)
        {
            if (chunk.neighborhood[6] == null)
                return;
            chunk.neighborhood[6].voxels[ix][iy][0] += change;
            if (chunk.neighborhood[6].voxels[ix][iy][0] > ChunkSVT.maxVoxelValue)
                chunk.neighborhood[6].voxels[ix][iy][0] = ChunkSVT.maxVoxelValue;
            if (chunk.neighborhood[6].voxels[ix][iy][0] < 0)
                chunk.neighborhood[6].voxels[ix][iy][0] = 0;
        }
        else if (ix != ChunkSVT.chunkWidthGC && iz != ChunkSVT.chunkWidthGC)
        {
            if (chunk.neighborhood[4] == null)
                return;
            chunk.neighborhood[4].voxels[0][iy][iz] += change;
            if (chunk.neighborhood[4].voxels[0][iy][iz] > ChunkSVT.maxVoxelValue)
                chunk.neighborhood[4].voxels[0][iy][iz] = ChunkSVT.maxVoxelValue;
            if (chunk.neighborhood[4].voxels[0][iy][iz] < 0)
                chunk.neighborhood[4].voxels[0][iy][iz] = 0;
        }
        else
        {
            if (chunk.neighborhood[7] == null)
                return;
            chunk.neighborhood[7].voxels[0][iy][0] += change;
            if (chunk.neighborhood[7].voxels[0][iy][0] > ChunkSVT.maxVoxelValue)
                chunk.neighborhood[7].voxels[0][iy][0] = ChunkSVT.maxVoxelValue;
            if (chunk.neighborhood[7].voxels[0][iy][0] < 0)
                chunk.neighborhood[7].voxels[0][iy][0] = 0;
        }
        
        // update SVT
        if (ix == 0 && iz == 0) // 0
        {
            chunk.ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[0] != null)
                chunk.neighborhood[0].ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[1] != null)
                chunk.neighborhood[1].ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[3] != null)
                chunk.neighborhood[3].ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
        }
        else if (ix > 0 && ix < ChunkSVT.chunkWidthGC - 1 && iz == 0) // 1
        {
            chunk.ConvertVoxelsToSVT(ix - 1, iy - 1, 0, 2, 2, 1);
            if (chunk.neighborhood[1] != null)
                chunk.neighborhood[1].ConvertVoxelsToSVT(ix - 1, iy - 1, 15, 2, 2, 1);
        }
        else if (ix == ChunkSVT.chunkWidthGC && iz == 0) // 2
        {
            chunk.ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[1] != null)
                chunk.neighborhood[1].ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[2] != null)
                chunk.neighborhood[2].ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[4] != null)
                chunk.neighborhood[4].ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);
        }
        else if (ix == 0 && iz > 0 && iz < ChunkSVT.chunkWidthGC - 1) // 3
        {
            chunk.ConvertVoxelsToSVT(0, iy - 1, iz - 1, 1, 2, 2);
            if (chunk.neighborhood[3] != null)
                chunk.neighborhood[3].ConvertVoxelsToSVT(15, iy - 1, iz - 1, 1, 2, 2);

        }
        else if (ix == ChunkSVT.chunkWidthGC && iz > 0 && iz < ChunkSVT.chunkWidthGC - 1) // 4
        {
            chunk.ConvertVoxelsToSVT(15, iy - 1, iz - 1, 1, 2, 2);
            if (chunk.neighborhood[4] != null)
                chunk.neighborhood[4].ConvertVoxelsToSVT(0, iy - 1, iz - 1, 1, 2, 2);

        }
        else if (ix == 0 && iz == ChunkSVT.chunkWidthGC) // 5
        {
            chunk.ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[3] != null)
                chunk.neighborhood[3].ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[5] != null)
                chunk.neighborhood[5].ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[6] != null)
                chunk.neighborhood[6].ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);

        }
        else if (ix > 0 && ix < ChunkSVT.chunkWidthGC - 1 && iz == ChunkSVT.chunkWidthGC) // 6
        {
            chunk.ConvertVoxelsToSVT(ix - 1, iy - 1, 15, 2, 2, 1);
            if (chunk.neighborhood[6] != null)
                chunk.neighborhood[6].ConvertVoxelsToSVT(ix - 1, iy - 1, 0, 2, 2, 1);

        }
        else if (ix == ChunkSVT.chunkWidthGC - 1 && iz == ChunkSVT.chunkWidthGC) // 7
        {
            chunk.ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[4] != null)
                chunk.neighborhood[4].ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[6] != null)
                chunk.neighborhood[6].ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[7] != null)
                chunk.neighborhood[7].ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);
        }
        else // -
        {
            chunk.ConvertVoxelsToSVT(ix - 1, iy - 1, iz - 1, 2, 2, 2);
        }

        /*// update SVT
        if (ix == 0 && iz == 0) // 0
        {
            chunk.ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[0] != null)
                chunk.neighborhood[0].ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[1] != null)
                chunk.neighborhood[1].ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[3] != null)
                chunk.neighborhood[3].ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
        }
        else if (ix > 0 && ix < ChunkSVT.chunkWidthGC - 1 && iz == 0) // 1
        {
            chunk.ConvertVoxelsToSVT(ix - 1, iy - 1, 0, 2, 2, 1);
            if (chunk.neighborhood[1] != null)
                chunk.neighborhood[1].ConvertVoxelsToSVT(ix - 1, iy - 1, 15, 2, 2, 1);
        }
        else if (ix == ChunkSVT.chunkWidthGC - 1 && iz == 0) // 2
        {
            chunk.ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[1] != null)
                chunk.neighborhood[1].ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[2] != null)
                chunk.neighborhood[2].ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[4] != null)
                chunk.neighborhood[4].ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);
        }
        else if (ix == 0 && iz > 0 && iz < ChunkSVT.chunkWidthGC - 1) // 3
        {
            chunk.ConvertVoxelsToSVT(0, iy - 1, iz - 1, 1, 2, 2);
            if (chunk.neighborhood[3] != null)
                chunk.neighborhood[3].ConvertVoxelsToSVT(15, iy - 1, iz - 1, 1, 2, 2);

        }
        else if (ix == ChunkSVT.chunkWidthGC - 1 && iz > 0 && iz < ChunkSVT.chunkWidthGC - 1) // 4
        {
            chunk.ConvertVoxelsToSVT(15, iy - 1, iz - 1, 1, 2, 2);
            if (chunk.neighborhood[4] != null)
                chunk.neighborhood[4].ConvertVoxelsToSVT(0, iy - 1, iz - 1, 1, 2, 2);

        }
        else if (ix == 0 && iz == ChunkSVT.chunkWidthGC - 1) // 5
        {
            chunk.ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[3] != null)
                chunk.neighborhood[3].ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[5] != null)
                chunk.neighborhood[5].ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[6] != null)
                chunk.neighborhood[6].ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);

        }
        else if (ix > 0 && ix < ChunkSVT.chunkWidthGC - 1 && iz == ChunkSVT.chunkWidthGC - 1) // 6
        {
            chunk.ConvertVoxelsToSVT(ix - 1, iy - 1, 15, 2, 2, 1);
            if (chunk.neighborhood[6] != null)
                chunk.neighborhood[6].ConvertVoxelsToSVT(ix - 1, iy - 1, 0, 2, 2, 1);

        }
        else if (ix == ChunkSVT.chunkWidthGC - 1 && iz == ChunkSVT.chunkWidthGC - 1) // 7
        {
            chunk.ConvertVoxelsToSVT(15, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[4] != null)
                chunk.neighborhood[4].ConvertVoxelsToSVT(0, iy - 1, 15, 1, 2, 1);
            if (chunk.neighborhood[6] != null)
                chunk.neighborhood[6].ConvertVoxelsToSVT(15, iy - 1, 0, 1, 2, 1);
            if (chunk.neighborhood[7] != null)
                chunk.neighborhood[7].ConvertVoxelsToSVT(0, iy - 1, 0, 1, 2, 1);
        }
        else // -
        {
            chunk.ConvertVoxelsToSVT(ix - 1, iy - 1, iz - 1, 2, 2, 2);
        }*/


        ///...
            /*if (chunk.neighborhood[0] != null)
                chunk.neighborhood[0].ConvertVoxelsToSVT();
            if (chunk.neighborhood[1] != null)
                chunk.neighborhood[1].ConvertVoxelsToSVT();
            if (chunk.neighborhood[3] != null)
                chunk.neighborhood[3].ConvertVoxelsToSVT();*/
    }
}
