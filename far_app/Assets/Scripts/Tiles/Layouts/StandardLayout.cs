using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardLayout : BaseLayout
{
    public override string LayoutName { get {
        return "StandardLayout";
    } }

    public override HashSet<Vector3> GetTilePositions(Vector2 inTileSize, Vector3 inOrigin, Rect inRegionToSpawn)
    {
        Vector2 origin2d = new Vector2(inOrigin.x, inOrigin.z);

        HashSet<Vector3> tilePositions = new HashSet<Vector3>();

        Vector2 startTemp = (inRegionToSpawn.min - origin2d) / inTileSize;
        Vector2Int startIndices = new Vector2Int(Mathf.RoundToInt(startTemp.x), Mathf.RoundToInt(startTemp.y));
        
        Vector2 endTemp = (inRegionToSpawn.max - origin2d) / inTileSize;
        Vector2Int endIndices = new Vector2Int(Mathf.RoundToInt(endTemp.x), Mathf.RoundToInt(endTemp.y));

        for (int y = startIndices.y; y <= endIndices.y; y++)
        {
            for (int x = startIndices.x; x <= endIndices.x; x++)
            {
                Vector3 tilePosition = TilePositionFromIndex(inOrigin, inTileSize, x, y);
                tilePositions.Add(tilePosition);
            }
        }

        return tilePositions;
    }

    public Vector3 TilePositionFromIndex(Vector3 inOrigin, Vector2 inTileSize, int inIndexX, int inIndexY)
    {
        Vector3 tilePosition = new Vector3(
            inOrigin.x + inIndexX * inTileSize.x,
            inOrigin.y,
            inOrigin.z + inIndexY * inTileSize.y);
        return tilePosition;
    }
}
