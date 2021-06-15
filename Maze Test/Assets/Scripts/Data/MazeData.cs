using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct MazeData :  IComponentData
{
    public int Width;
    public int Height;
}

