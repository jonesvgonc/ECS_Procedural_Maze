using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public struct MazeNode 
{
    public float3 NodeCenter;
    public Position NodePosition;
    public WallState WallState;
}
