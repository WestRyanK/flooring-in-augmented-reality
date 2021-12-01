using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum FlooringOptionEnum
{
    [EnumName("Tile 1")]
    tile1 = 0,
    [EnumName("Tile 2")]
    tile2 = 1,
    [EnumName("Tile 3")]
    tile3 = 2,
    [EnumName("Tile 4")]
    tile4 = 3,
    [EnumName("Lava")]
    lava = 4
}

[Serializable]
public struct FlooringOptionEnumToTexture2D
{
    public FlooringOptionEnum _flooringOptionEnum;
    public Texture2D _texture2D;
}