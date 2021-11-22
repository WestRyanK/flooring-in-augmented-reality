using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLayout
{
    public abstract string LayoutName { get; }

    public abstract HashSet<Vector3> GetTilePositions(Vector2 inTileSize, Vector3 inOrigin, Rect inRegionToSpawn);
}
