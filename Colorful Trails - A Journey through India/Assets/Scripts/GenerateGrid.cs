using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGrid : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public Transform actualGrid;
    public float xPosOfActualGrid;
    public float yPosOfActualGrid;
    public GameObject[] tiles;
    public GameObject[,] allActualTiles;
    public float widthBetweenCell;
    public float heightBetweenCell;
    public int targetX;
    public int targetY;

    // Use this for initialization
    void Start()
    {
        allActualTiles = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                Vector2 tempPositon = new Vector2(i * widthBetweenCell, j * heightBetweenCell);
                int tilestoUse = Random.Range(0, tiles.Length);
                GameObject tile = Instantiate(tiles[tilestoUse], tempPositon, Quaternion.identity, actualGrid);
                tile.name = i + "," + j;
                allActualTiles[i, j] = tile;
            }
        }
        actualGrid.transform.position = new Vector2(xPosOfActualGrid, yPosOfActualGrid);
    }
}
