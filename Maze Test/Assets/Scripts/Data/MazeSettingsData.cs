using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct MazeSettingsData : IComponentData
{
    public int Width;
    public int Height;
}
