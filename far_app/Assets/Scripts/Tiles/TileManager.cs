using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField]
    private BaseLayout _layout;
    [SerializeField]
    private Tile _tilePrefab;
    [SerializeField]
    private Transform _tilesParent;
    private Camera _camera;
    private Dictionary<Vector3, Tile> _tiles;


    
    // Start is called before the first frame update
    void Start()
    {
        _layout = new StandardLayout();
        _camera = FindObjectOfType<Camera>(false);
        DisplayTilesInRegion(new Rect(0, 0, _tilePrefab.TileSize.x * 10, _tilePrefab.TileSize.y * 10));
    }

    void Update()
    {
        DisplayTilesInRegion(new Rect(0, 0, _tilePrefab.TileSize.x * 10, _tilePrefab.TileSize.y * 10));
    }

    public void DisplayTilesInRegion(Rect inRegion)
    {
        if (_layout != null && _tilePrefab != null && _tilesParent != null)
        {
            if (_tiles == null)
            {
                _tiles = new Dictionary<Vector3, Tile>();
            }

            HashSet<Vector3> tilePositions = _layout.GetTilePositions(_tilePrefab.TileSize, _camera.transform.position, inRegion);
            UpdateVisibleTiles(tilePositions);
        }
    }

    public void UpdateVisibleTiles(HashSet<Vector3> tilePositions)
    {
        List<Vector3> tilesToRemove = new List<Vector3>();
        foreach (Vector3 originalTile in _tiles.Keys)
        {
            if (!tilePositions.Contains(originalTile))
            {
                tilesToRemove.Add(originalTile);
            }
        }
        foreach (Vector3 tileToRemove in tilesToRemove)
        {
            Destroy(_tiles[tileToRemove]);
            _tiles.Remove(tileToRemove);
        }

        foreach (Vector3 tilePosition in tilePositions)
        {
            if (!_tiles.ContainsKey(tilePosition))
            {
                Tile tile = Instantiate<Tile>(_tilePrefab, tilePosition, Quaternion.identity, _tilesParent);
                _tiles.Add(tilePosition, tile);
            }
        }
    }

    public void ClearTiles()
    {
        if (_tiles != null)
        {
            foreach (Tile tile in _tiles.Values)
            {
                Destroy(tile.gameObject);
            }
            _tiles.Clear();
        }
    }
}
