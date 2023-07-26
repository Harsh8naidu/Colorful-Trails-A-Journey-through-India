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

    // Use this for initialization
    void Start()
    {
        allActualTiles = new GameObject[width, height];
        SetUp();
    }

    private void SetUp()
    {
        List<int> tileCounts = new List<int>();

        for (int i = 0; i < tiles.Length; i++)
        {
            tileCounts.Add(0);
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i * widthBetweenCell, j * heightBetweenCell);

                int tileIndex = Random.Range(0, tiles.Length);
                int maxIterations = 0;

                while (MatchesAt(i, j, tiles[tileIndex]) && maxIterations < 100)
                {
                    tileIndex = Random.Range(0, tiles.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject tile = Instantiate(tiles[tileIndex], tempPosition, Quaternion.identity, actualGrid);
                tile.name = i + "," + j;
                allActualTiles[i, j] = tile;

                tileCounts[tileIndex]++;
            }
        }

        actualGrid.transform.position = new Vector2(xPosOfActualGrid, yPosOfActualGrid);
    }

    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allActualTiles[column - 1, row].tag == piece.tag && allActualTiles[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allActualTiles[column, row - 1].tag == piece.tag && allActualTiles[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        } else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allActualTiles[column, row - 1].tag == piece.tag && allActualTiles[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allActualTiles[column - 1, row].tag == piece.tag && allActualTiles[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void CollapseTiles()
    {
        for (int i = 0; i < width; i++)
        {
            int emptySpaces = 0;
            for (int j = 0; j < height; j++)
            {
                if (allActualTiles[i, j] == null)
                {
                    emptySpaces++;
                    Debug.Log(emptySpaces);
                }
                else if (emptySpaces > 0)
                {
                    // Move the non-empty tile to the empty space
                    Vector3 newPosition = allActualTiles[i, j].transform.position - new Vector3(0, emptySpaces, 0) + new Vector3(0, -16f * emptySpaces, 0); ;
                    allActualTiles[i, j].GetComponent<Tile>().rowForCheckingMatches = j - emptySpaces;
                    LeanTween.move(allActualTiles[i, j], newPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);

                    allActualTiles[i, j - emptySpaces] = allActualTiles[i, j];
                    allActualTiles[i, j] = null;
                }
            }
        }
    }
}
