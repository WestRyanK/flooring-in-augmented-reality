using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum FlooringOptionEnum
{
    [EnumName("Tile 0")]
    tile0 = 0,
    [EnumName("Tile 1")]
    tile1 = 1,
    [EnumName("Tile 2")]
    tile2 = 2,
    [EnumName("Tile 3")]
    tile3 = 3,
    [EnumName("Tile 4")]
    tile4 = 4,
    [EnumName("Lava")]
    lava = 5
}

[Serializable]
public struct FlooringOptionEnumToTexture2D
{
    public FlooringOptionEnum _flooringOptionEnum;
    public Texture2D _texture2D;
}