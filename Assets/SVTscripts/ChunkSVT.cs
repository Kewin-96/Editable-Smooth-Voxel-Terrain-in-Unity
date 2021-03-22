using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* UNITY
       4--------5       *---4----*
      /|       /|      /|       /|
     / |      / |     7 |      5 |
    /  y     /  |    /  8     /  9
   7--------6   |   *----6---*   |
   |   |    |   |   |   |    |   |
   |   0--x-|---1   |   *---0|---*
   |  /     |  /    11 /     10 /
   | z      | /     | 3      | 1
   |/       |/      |/       |/
   3--------2       *---2----*
   
   // [0] [1] [2]
   // [3] [-] [4] x
   // [5] [6] [7]
   //      z
*/

/// <summary>
/// Gridcell - 8 points with values
/// </summary>
public class Gridcell
{
    public Vector3[] p = new Vector3[8];
    public float[] val = new float[8];
}

/// <summary>
/// Triangle
/// </summary>
public class Triangle
{
    public Vector3[] p = new Vector3[3];
}

public class ChunkSVT : MonoBehaviour
{
    // Data for SVT algorythm
    public static readonly int[/*256*/] edgeTable ={
0x0  , 0x109, 0x203, 0x30a, 0x406, 0x50f, 0x605, 0x70c,
0x80c, 0x905, 0xa0f, 0xb06, 0xc0a, 0xd03, 0xe09, 0xf00,
0x190, 0x99 , 0x393, 0x29a, 0x596, 0x49f, 0x795, 0x69c,
0x99c, 0x895, 0xb9f, 0xa96, 0xd9a, 0xc93, 0xf99, 0xe90,
0x230, 0x339, 0x33 , 0x13a, 0x636, 0x73f, 0x435, 0x53c,
0xa3c, 0xb35, 0x83f, 0x936, 0xe3a, 0xf33, 0xc39, 0xd30,
0x3a0, 0x2a9, 0x1a3, 0xaa , 0x7a6, 0x6af, 0x5a5, 0x4ac,
0xbac, 0xaa5, 0x9af, 0x8a6, 0xfaa, 0xea3, 0xda9, 0xca0,
0x460, 0x569, 0x663, 0x76a, 0x66 , 0x16f, 0x265, 0x36c,
0xc6c, 0xd65, 0xe6f, 0xf66, 0x86a, 0x963, 0xa69, 0xb60,
0x5f0, 0x4f9, 0x7f3, 0x6fa, 0x1f6, 0xff , 0x3f5, 0x2fc,
0xdfc, 0xcf5, 0xfff, 0xef6, 0x9fa, 0x8f3, 0xbf9, 0xaf0,
0x650, 0x759, 0x453, 0x55a, 0x256, 0x35f, 0x55 , 0x15c,
0xe5c, 0xf55, 0xc5f, 0xd56, 0xa5a, 0xb53, 0x859, 0x950,
0x7c0, 0x6c9, 0x5c3, 0x4ca, 0x3c6, 0x2cf, 0x1c5, 0xcc ,
0xfcc, 0xec5, 0xdcf, 0xcc6, 0xbca, 0xac3, 0x9c9, 0x8c0,
0x8c0, 0x9c9, 0xac3, 0xbca, 0xcc6, 0xdcf, 0xec5, 0xfcc,
0xcc , 0x1c5, 0x2cf, 0x3c6, 0x4ca, 0x5c3, 0x6c9, 0x7c0,
0x950, 0x859, 0xb53, 0xa5a, 0xd56, 0xc5f, 0xf55, 0xe5c,
0x15c, 0x55 , 0x35f, 0x256, 0x55a, 0x453, 0x759, 0x650,
0xaf0, 0xbf9, 0x8f3, 0x9fa, 0xef6, 0xfff, 0xcf5, 0xdfc,
0x2fc, 0x3f5, 0xff , 0x1f6, 0x6fa, 0x7f3, 0x4f9, 0x5f0,
0xb60, 0xa69, 0x963, 0x86a, 0xf66, 0xe6f, 0xd65, 0xc6c,
0x36c, 0x265, 0x16f, 0x66 , 0x76a, 0x663, 0x569, 0x460,
0xca0, 0xda9, 0xea3, 0xfaa, 0x8a6, 0x9af, 0xaa5, 0xbac,
0x4ac, 0x5a5, 0x6af, 0x7a6, 0xaa , 0x1a3, 0x2a9, 0x3a0,
0xd30, 0xc39, 0xf33, 0xe3a, 0x936, 0x83f, 0xb35, 0xa3c,
0x53c, 0x435, 0x73f, 0x636, 0x13a, 0x33 , 0x339, 0x230,
0xe90, 0xf99, 0xc93, 0xd9a, 0xa96, 0xb9f, 0x895, 0x99c,
0x69c, 0x795, 0x49f, 0x596, 0x29a, 0x393, 0x99 , 0x190,
0xf00, 0xe09, 0xd03, 0xc0a, 0xb06, 0xa0f, 0x905, 0x80c,
0x70c, 0x605, 0x50f, 0x406, 0x30a, 0x203, 0x109, 0x0   };
    public static readonly int[/*256*/,/*16*/] triTable =
{{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};
    public static readonly int[/*256*/] triTableLengths =
{0,1,1,2,1,2,2,3,1,2,2,3,2,3,3,2,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,3,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,3,2,3,3,2,3,4,4,3,3,4,4,3,4,5,5,2,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,3,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,4,2,3,3,4,3,4,2,3,3,4,4,5,4,5,3,2,3,4,4,3,4,5,3,2,4,5,5,4,5,2,4,1,1,2,2,3,2,3,3,4,2,3,3,4,3,4,4,3,2,3,3,4,3,4,4,5,3,2,4,3,4,3,5,2,2,3,3,4,3,4,4,5,3,4,4,5,4,5,5,4,3,4,4,3,4,5,5,4,4,3,5,2,5,4,2,1,2,3,3,4,3,4,4,5,3,4,4,5,2,3,3,2,3,4,4,5,4,5,5,2,4,3,5,4,3,2,4,1,3,4,4,5,4,5,3,4,4,5,5,2,3,4,2,1,2,3,3,2,3,4,2,1,3,2,4,1,2,1,1,0};

    // Chunks static parameters
    public static readonly int chunkWidthGC = 16;                       // chunk width in gridcells
    public static readonly int chunkHeightGC = 256;                     // chunk height in gridcells
    public static readonly int gridcellWidth = 1;                       // gridcell width
    public static readonly int chunkWidth = chunkWidthGC * gridcellWidth;    // chunk width
    public static readonly int chunkHeight = chunkHeightGC * gridcellWidth;  // chunk height
    public static readonly int isolevel = 128;                          // isolevel

    // Chunk non-static parameters
    public int xIndex;
    public int yIndex;

    // Main data of terrain
    public float[][][] voxels;                  // voxels value in 3D array
    public Gridcell[][][] grid;                 // Grid [x][y][z] - y is vertical axis !!!

    // Fields needed for creating mesh
    public Triangle[] triangles;                // all triangles of chunk
    public Vector3[] vertices;                  // all vertices of all triangles of chunk (every 3 continously vertices is 1 triangle)
    public Vector3[] normals;                   // normals of each vertice
    public Vector2[] UV;                        // UV - empty for now
    public int[] verticesIndexes;               // indexes of all vertices (every 3 continously vertices is 1 triangle)

    // Mesh collider
    public MeshCollider meshCollider;
    public Mesh mesh;                           // mesh

    // References
    public SVTterrain sVTterrain;

    /// [0] [1] [2]
    /// [3] [-] [4] x
    /// [5] [6] [7]
    ///      z/y
    public ChunkSVT[] neighborhood;             // chunks in neighborhood

    /// <summary>
    /// 
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="terraintransform.positionation"></param>
    /// <param name="xIndex"></param>
    /// <param name="yIndex"></param>
    public void ChunkPreInit(SVTterrain terrain, Vector3 terrainWorldLocation, int xIndex, int yIndex)
    {
        sVTterrain = terrain;
        transform.position = terrainWorldLocation + new Vector3(xIndex * chunkWidth, 0, yIndex * chunkWidth);
        Debug.Log(transform.position);
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        ChunkInit();
        neighborhood = GetNeighborhood();
    }

    /// <summary>
    /// Chunk initialization
    /// </summary>
    public void ChunkInit()
    {
        // initialization of voxels
        voxels = new float[chunkWidthGC][][];
        for (int i = 0; i < chunkWidthGC; i++)
        {
            voxels[i] = new float[chunkHeightGC][];
            for (int j = 0; j < chunkHeightGC; j++)
                voxels[i][j] = new float[chunkWidthGC];
        }
        
        // initialization of grid
        grid = new Gridcell[chunkWidthGC][][];
        for (int i = 0; i < chunkWidthGC; i++)
        {
            grid[i] = new Gridcell[chunkHeightGC][];
            for (int j = 0; j < chunkHeightGC; j++)
            {
                grid[i][j] = new Gridcell[chunkWidthGC];
                for (int k = 0; k < chunkWidthGC; k++)
                {
                    grid[i][j][k] = new Gridcell();
                    grid[i][j][k].p[0] = new Vector3(i * gridcellWidth, j * gridcellWidth, k * gridcellWidth);
                    grid[i][j][k].p[1] = new Vector3((i + 1) * gridcellWidth, j * gridcellWidth, k * gridcellWidth);
                    grid[i][j][k].p[2] = new Vector3((i + 1) * gridcellWidth, j * gridcellWidth, (k + 1) * gridcellWidth);
                    grid[i][j][k].p[3] = new Vector3(i * gridcellWidth, j * gridcellWidth, (k + 1) * gridcellWidth);
                    grid[i][j][k].p[4] = new Vector3(i * gridcellWidth, (j + 1) * gridcellWidth, k * gridcellWidth);
                    grid[i][j][k].p[5] = new Vector3((i + 1) * gridcellWidth, (j + 1) * gridcellWidth, k * gridcellWidth);
                    grid[i][j][k].p[6] = new Vector3((i + 1) * gridcellWidth, (j + 1) * gridcellWidth, (k + 1) * gridcellWidth);
                    grid[i][j][k].p[7] = new Vector3(i * gridcellWidth, (j + 1) * gridcellWidth, (k + 1) * gridcellWidth);
                }
            }
        }

        // initialization of neighborhood
        neighborhood = new ChunkSVT[8];

        // inicjalization mesh object
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;         // setting mesh for game object
        
        // DEBUG - test filling of voxel terrain (In this place will be calculating terrain with seed and special mathematical noise function(s))
        for (int i = 0; i < chunkWidthGC; i++)
            for (int j = 0; j < 5; j++)
                for (int k = 0; k < chunkWidthGC; k++)
                {
                    voxels[i][j][k] = 255;
                }
        for (int i = 0; i < chunkWidthGC; i++)
            for (int k = 0; k < chunkWidthGC; k++)
            {
                voxels[i][5][k] = 255 * Mathf.Sin(i) * Mathf.Cos(k);
            }

        // Converts voxels to SVT
        ConvertVoxelsToSVT();
    }

    /// <summary>
    /// Finds 8 neighbors of given chunk
    /// </summary>
    /// <param name="chunk">chunk</param>
    /// <returns>Neighborhood - array of 8 neighbors</returns>
    public ChunkSVT[] GetNeighborhood()
    {
        /// [0] [1] [2]
        /// [3] [-] [4] x
        /// [5] [6] [7]
        ///      z/y
        ChunkSVT[] neighborhood = new ChunkSVT[8];
        for (int i = 0; i < sVTterrain.terrainGO.Count; i++)
        {
            if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().xIndex == xIndex - 1)
            {
                if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex - 1)
                    neighborhood[7] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
                else if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex)
                    neighborhood[4] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
                else if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex + 1)
                    neighborhood[2] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
            }
            if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().xIndex == xIndex)
            {
                if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex - 1)
                    neighborhood[6] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
                else if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex + 1)
                    neighborhood[1] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
            }
            if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().xIndex == xIndex + 1)
            {
                if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex - 1)
                    neighborhood[5] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
                else if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex)
                    neighborhood[3] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
                else if (sVTterrain.terrainGO[i].GetComponent<ChunkSVT>().yIndex == yIndex + 1)
                    neighborhood[0] = sVTterrain.terrainGO[i].GetComponent<ChunkSVT>();
            }
        }
        return neighborhood;
    }

    /// <summary>
    /// Calculates gridcell into triangles
    /// </summary>
    /// <param name="g">input gridcell (8 points, with values)</param>
    /// <param name="iso">isolevel</param>
    /// <returns>Array of triangles (possible number of triangles: 0-5)</returns>
    public static Triangle[] PolygoniseCube(Gridcell g, float iso)
    {
        Triangle[] tri;
        int cubeindex;
        Vector3[] vertlist = new Vector3[12];

        cubeindex = 0;
        if (g.val[0] > iso) cubeindex |= 1;     //0000 0001
        if (g.val[1] > iso) cubeindex |= 2;     //0000 0010
        if (g.val[2] > iso) cubeindex |= 4;     //0000 0100
        if (g.val[3] > iso) cubeindex |= 8;     //0000 1000
        if (g.val[4] > iso) cubeindex |= 16;        //0001 0000
        if (g.val[5] > iso) cubeindex |= 32;        //0010 0000
        if (g.val[6] > iso) cubeindex |= 64;        //0100 0000
        if (g.val[7] > iso) cubeindex |= 128;   //1000 0000

        /* Cube is entirely in/out of the surface */
        if (edgeTable[cubeindex] == 0)
        {
            return new Triangle[0];
        }

        /* Find the vertices where the surface intersects the cube */
        if ((edgeTable[cubeindex] & 1) != 0)
        {
            vertlist[0] = VertexInterp(iso, g.p[0], g.p[1], g.val[0], g.val[1]);
        }
        if ((edgeTable[cubeindex] & 2) != 0)
        {
            vertlist[1] = VertexInterp(iso, g.p[1], g.p[2], g.val[1], g.val[2]);
        }
        if ((edgeTable[cubeindex] & 4) != 0)
        {
            vertlist[2] = VertexInterp(iso, g.p[2], g.p[3], g.val[2], g.val[3]);
        }
        if ((edgeTable[cubeindex] & 8) != 0)
        {
            vertlist[3] = VertexInterp(iso, g.p[3], g.p[0], g.val[3], g.val[0]);
        }
        if ((edgeTable[cubeindex] & 16) != 0)
        {
            vertlist[4] = VertexInterp(iso, g.p[4], g.p[5], g.val[4], g.val[5]);
        }
        if ((edgeTable[cubeindex] & 32) != 0)
        {
            vertlist[5] = VertexInterp(iso, g.p[5], g.p[6], g.val[5], g.val[6]);
        }
        if ((edgeTable[cubeindex] & 64) != 0)
        {
            vertlist[6] = VertexInterp(iso, g.p[6], g.p[7], g.val[6], g.val[7]);
        }
        if ((edgeTable[cubeindex] & 128) != 0)
        {
            vertlist[7] = VertexInterp(iso, g.p[7], g.p[4], g.val[7], g.val[4]);
        }
        if ((edgeTable[cubeindex] & 256) != 0)
        {
            vertlist[8] = VertexInterp(iso, g.p[0], g.p[4], g.val[0], g.val[4]);
        }
        if ((edgeTable[cubeindex] & 512) != 0)
        {
            vertlist[9] = VertexInterp(iso, g.p[1], g.p[5], g.val[1], g.val[5]);
        }
        if ((edgeTable[cubeindex] & 1024) != 0)
        {
            vertlist[10] = VertexInterp(iso, g.p[2], g.p[6], g.val[2], g.val[6]);
        }
        if ((edgeTable[cubeindex] & 2048) != 0)
        {
            vertlist[11] = VertexInterp(iso, g.p[3], g.p[7], g.val[3], g.val[7]);
        }

        /* Create the triangles */
        tri = new Triangle[triTableLengths[cubeindex]];
        for (int i = 0; i < triTableLengths[cubeindex]; i++)
        {
            tri[i] = new Triangle();
            tri[i].p[0] = vertlist[triTable[cubeindex, i * 3]];
            tri[i].p[1] = vertlist[triTable[cubeindex, (i * 3) + 1]];
            tri[i].p[2] = vertlist[triTable[cubeindex, (i * 3) + 2]];
        }

        return tri;
    }

    /// <summary>
    /// Vertex interpolation
    /// </summary>
    /// <param name="isolevel">isolevel</param>
    /// <param name="p1">point no 1</param>
    /// <param name="p2">point no 2</param>
    /// <param name="valp1">point 1 value</param>
    /// <param name="valp2">point 2 value</param>
    /// <returns>Interpolated point</returns>
    public static Vector3 VertexInterp(float isolevel, Vector3 p1, Vector3 p2, float valp1, float valp2)
    {
        float mu;
        Vector3 p;

        if (Mathf.Abs(isolevel - valp1) < 0.00001)
            return (p1);
        if (Mathf.Abs(isolevel - valp2) < 0.00001)
            return (p2);
        if (Mathf.Abs(valp1 - valp2) < 0.00001)
            return (p1);
        mu = (isolevel - valp1) / (valp2 - valp1);
        p.x = p1.x + mu * (p2.x - p1.x);
        p.y = p1.y + mu * (p2.y - p1.y);
        p.z = p1.z + mu * (p2.z - p1.z);

        return (p);
    }

    /// <summary>
    /// Converts gridcells to voxels
    /// </summary>
    /// <param name="x1">x start of update</param>
    /// <param name="y1">y start of update</param>
    /// <param name="z1">z start of update</param>
    /// <param name="xL">x length of update</param>
    /// <param name="yL">y length of update</param>
    /// <param name="zL">z length of update</param>
    /// <param name="chunk">chunk</param>
    /// <param name="neighborChunks">neighbor chunk</param>
    public void ConvertVoxelsToGridcells(int x1, int y1, int z1, int xL, int yL, int zL, ChunkSVT[] neighborChunks)
    {
        for (int i = x1; i < x1 + xL; i++)
        {
            for (int j = y1; j < y1 + yL; j++)
            {
                for (int k = z1; k < z1 + zL; k++)
                {
                    // 0 - gricell vertice number
                    grid[i][j][k].val[0] = voxels[i][j][k];

                    // 1
                    if (i > 0)
                        grid[i - 1][j][k].val[1] = voxels[i][j][k];
                    else
                    {
                        if (neighborChunks[3] != null)
                            neighborChunks[3].grid[i - 1][j][k].val[1] = voxels[i][j][k];
                    }

                    // 2
                    if (i > 0 && k > 0)
                        grid[i - 1][j][k - 1].val[2] = voxels[i][j][k];
                    else if (i > 0)
                    {
                        if (neighborChunks[3] != null)
                            neighborChunks[3].grid[i - 1][j][k - 1].val[2] = voxels[i][j][k];
                    }
                    else if (k > 0)
                    {
                        if (neighborChunks[1] != null)
                            neighborChunks[1].grid[i - 1][j][k - 1].val[2] = voxels[i][j][k];
                    }
                    else
                    {
                        if (neighborChunks[0] != null)
                            neighborChunks[0].grid[i - 1][j][k - 1].val[2] = voxels[i][j][k];
                    }

                    // 3
                    if (k > 0)
                        grid[i][j][k - 1].val[3] = voxels[i][j][k];
                    else
                    {
                        if (neighborChunks[1] != null)
                            neighborChunks[1].grid[i][j][k - 1].val[3] = voxels[i][j][k];
                    }

                    // 4, 5, 6, 7
                    if (j > 0 && j < chunkHeightGC)
                    {
                        // 4
                        grid[i][j - 1][k].val[4] = voxels[i][j][k];

                        // 5
                        if (i > 0)
                            grid[i - 1][j - 1][k].val[5] = voxels[i][j][k];
                        else
                        {
                            if (neighborChunks[3] != null)
                                neighborChunks[3].grid[i - 1][j - 1][k].val[5] = voxels[i][j][k];
                        }

                        // 6
                        if (i > 0 && k > 0)
                            grid[i - 1][j - 1][k - 1].val[6] = voxels[i][j][k];
                        else if (i > 0)
                        {
                            if (neighborChunks[3] != null)
                                neighborChunks[3].grid[i - 1][j - 1][k - 1].val[6] = voxels[i][j][k];
                        }
                        else if (k > 0)
                        {
                            if (neighborChunks[1] != null)
                                neighborChunks[1].grid[i - 1][j - 1][k - 1].val[6] = voxels[i][j][k];
                        }
                        else
                        {
                            if (neighborChunks[0] != null)
                                neighborChunks[0].grid[i - 1][j - 1][k - 1].val[6] = voxels[i][j][k];
                        }

                        // 7
                        if (k > 0)
                            grid[i][j - 1][k - 1].val[7] = voxels[i][j][k];
                        else
                        {
                            if (neighborChunks[1] != null)
                                neighborChunks[1].grid[i][j - 1][k - 1].val[7] = voxels[i][j][k];
                        }
                    }
                    // else: gridcell below or above chunk - do nothing ...
                }
            }
        }
    }

    /// <summary>
    /// Converts grid into triangles
    /// </summary>
    /// <param name="chunk">chunk</param>
    public void ConvertGridToTriangles()
    {
        List<Triangle> trianglesL = new List<Triangle>();
        for (int i = 0; i < grid.Length; i++)
            for (int j = 0; j < grid[i].Length; j++)
                for (int k = 0; k < grid[i][j].Length; k++)
                {
                    Triangle[] trBuf = PolygoniseCube(grid[i][j][k], isolevel);
                    for (int l = 0; l < trBuf.Length; l++)
                        trianglesL.Add(trBuf[l]);
                }
        triangles = new Triangle[trianglesL.Count];
        trianglesL.CopyTo(triangles);
    }

    /// <summary>
    /// Converts all triangles into vertices
    /// </summary>
    /// <param name="triangles">triangles array</param>
    public void ConvertTrianglesToVertices()
    {
        vertices = new Vector3[3 * triangles.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            vertices[3 * i] = triangles[i].p[0];
            vertices[3 * i + 1] = triangles[i].p[1];
            vertices[3 * i + 2] = triangles[i].p[2];
        }

    }

    /// <summary>
    /// Calculates indexes of vertices
    /// </summary>
    /// <param name="chunk"></param>
    public void CalcIndexesOfVertices()
    {
        verticesIndexes = new int[triangles.Length * 3];
        for (int i = 0; i < triangles.Length * 3; i++)
            verticesIndexes[i] = i;
    }

    /// <summary>
    /// Calculates normals of vertices
    /// </summary>
    /// <param name="chunk"></param>
    public void CalcNormals()
    {
        normals = new Vector3[vertices.Length];
        for (int i = 0; i < triangles.Length; i++)
        {
            Vector3 A = new Vector3(triangles[i].p[1].x - triangles[i].p[0].x, triangles[i].p[1].y - triangles[i].p[0].y, triangles[i].p[1].z - triangles[i].p[0].z);
            Vector3 B = new Vector3(triangles[i].p[1].x - triangles[i].p[2].x, triangles[i].p[1].y - triangles[i].p[2].y, triangles[i].p[1].z - triangles[i].p[2].z);
            Vector3 n = new Vector3(A.y * B.z - A.z * B.y, A.z * B.x - A.x * B.z, A.x * B.y - A.y * B.x);
            n.Normalize();
            n.Scale(new Vector3(-1,-1,-1));
            normals[3 * i] = n;
            normals[3 * i + 1] = n;
            normals[3 * i + 2] = n;
        }
    }

    /// <summary>
    /// Updating mesh object and mesh collider
    /// </summary>
    public void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;               // all vertices of all triagles
        mesh.normals = normals;                 // normals
        mesh.uv = UV;                           // UV - empty array for now
        mesh.triangles = verticesIndexes;       // indexes of each vertices
        meshCollider.sharedMesh = mesh;         // mesh collider
    }

    /// <summary>
    /// Converts everything from data to SVT
    /// </summary>
    public void ConvertVoxelsToSVT()
    {
        // converting voxel array to vertices
        ConvertVoxelsToGridcells(0, 0, 0, chunkWidthGC, chunkHeightGC, chunkWidthGC, neighborhood);

        // polygonising
        ConvertGridToTriangles();

        // converting triangles to vertices
        ConvertTrianglesToVertices();

        // calculating indexes of vertices
        CalcIndexesOfVertices();

        // Calculate normals
        CalcNormals();

        // Update mesh
        UpdateMesh();
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary> 
    public void Start()
    {
        
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public void Update()
    {
        
    }
}
