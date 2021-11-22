using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2 TileSize;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.localScale = new Vector3(TileSize.x, 1, TileSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
