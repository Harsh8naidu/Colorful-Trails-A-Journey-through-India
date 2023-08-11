using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LeanTween;

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
    private Vector2[,] positionsOfTiles;
    public bool isTilesBroken;
    public ScoreManager scoreManager;

    // Use this for initialization
    void Start()
    {
        positionsOfTiles = new Vector2[width, height];
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
        if (column > 1 && row > 1)
        {
            if (allActualTiles[column - 1, row].tag == piece.tag && allActualTiles[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allActualTiles[column, row - 1].tag == piece.tag && allActualTiles[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if (column <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if (allActualTiles[column, row - 1].tag == piece.tag && allActualTiles[column, row - 2].tag == piece.tag)
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

    public void FindAndDestroyMatches(GameObject tile, string directionOfSwipe)
    {
        GameObject referenceToSwipedTile = tile;
        List<GameObject> tilesToDestroy = new List<GameObject>();
        List<GameObject> referenceToDestroyedTiles = new List<GameObject>();

        string tileName = tile.name;
        string[] nameParts = tileName.Split(',');
        int row = int.Parse(nameParts[0]);
        int column = int.Parse(nameParts[1]);

        // Check for horizontal matches
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allActualTiles[i, j] != null)
                {
                    if (j == column)
                    {
                        string currentTileTag = allActualTiles[i, j].tag;
                        string tileTag = tile.tag;

                        // Checking For Horizontal Tiles
                        if (i > 0 && i < width - 1)
                        {
                            if (allActualTiles[i - 1, j] != null && allActualTiles[i + 1, j] != null)
                            {
                                if (allActualTiles[i - 1, j].tag == tileTag && allActualTiles[i, j].tag == tileTag)
                                {
                                    if (allActualTiles[i - 1, j] != tile && allActualTiles[i, j] != tile && allActualTiles[i, j] != allActualTiles[i - 1, j])
                                    {
                                        tilesToDestroy.Add(allActualTiles[i - 1, j]);
                                        tilesToDestroy.Add(allActualTiles[i, j]);
                                        tilesToDestroy.Add(tile);
                                    }
                                }
                                else if (allActualTiles[i, j].tag == tileTag && allActualTiles[i + 1, j].tag == tileTag)
                                {
                                    if (allActualTiles[i, j] != tile && allActualTiles[i + 1, j] != tile && allActualTiles[i, j] != allActualTiles[i + 1, j])
                                    {
                                        tilesToDestroy.Add(tile);
                                        tilesToDestroy.Add(allActualTiles[i, j]);
                                        tilesToDestroy.Add(allActualTiles[i + 1, j]);
                                    }

                                }
                                else if (allActualTiles[i - 1, j].tag == tileTag && allActualTiles[i + 1, j].tag == tileTag)
                                {
                                    if (allActualTiles[i - 1, j] != tile && allActualTiles[i + 1, j] != tile && allActualTiles[i - 1, j] != allActualTiles[i + 1, j])
                                    {
                                        tilesToDestroy.Add(allActualTiles[i - 1, j]);
                                        tilesToDestroy.Add(tile);
                                        tilesToDestroy.Add(allActualTiles[i + 1, j]);
                                    }
                                }
                            }
                        }
                    }

                    if (i == row)
                    {
                        string currentTileTag = allActualTiles[i, j].tag;
                        string tileTag = tile.tag;

                        // Checking For Vertical Tiles
                        if (j > 0 && j < width - 1)
                        {
                            if (allActualTiles[i, j - 1] != null && allActualTiles[i, j + 1] != null)
                            {
                                if (allActualTiles[i, j - 1].tag == tileTag && allActualTiles[i, j].tag == tileTag)
                                {
                                    if (allActualTiles[i, j - 1] != tile && allActualTiles[i, j] != tile && allActualTiles[i, j - 1] != allActualTiles[i, j])
                                    {
                                        tilesToDestroy.Add(allActualTiles[i, j - 1]);
                                        tilesToDestroy.Add(allActualTiles[i, j]);
                                        tilesToDestroy.Add(tile);
                                    }
                                }
                                else if (allActualTiles[i, j].tag == tileTag && allActualTiles[i, j + 1].tag == tileTag)
                                {
                                    if (allActualTiles[i, j] != tile && allActualTiles[i, j + 1] != tile && allActualTiles[i, j] != allActualTiles[i, j + 1])
                                    {
                                        tilesToDestroy.Add(tile);
                                        tilesToDestroy.Add(allActualTiles[i, j]);
                                        tilesToDestroy.Add(allActualTiles[i, j + 1]);
                                    }
                                }
                                else if (allActualTiles[i, j - 1].tag == tileTag && allActualTiles[i, j + 1].tag == tileTag)
                                {
                                    if (allActualTiles[i, j - 1] != tile && allActualTiles[i, j + 1] != tile && allActualTiles[i, j - 1] != allActualTiles[i, j + 1])
                                    {
                                        tilesToDestroy.Add(allActualTiles[i, j - 1]);
                                        tilesToDestroy.Add(tile);
                                        tilesToDestroy.Add(allActualTiles[i, j + 1]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        List<GameObject> duplicateTilesToDestroy = new List<GameObject>();

        duplicateTilesToDestroy = tilesToDestroy;

        referenceToDestroyedTiles = tilesToDestroy.Distinct().ToList();

        foreach (var piece in tilesToDestroy)
        {
            if(piece != null)
            {
                piece.GetComponent<TileAnimator>().PlayAnimation(piece, referenceToDestroyedTiles.Count);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (tilesToDestroy.Contains(allActualTiles[i, j]))
                {
                    if (directionOfSwipe == "left" && allActualTiles[i, j] == tilesToDestroy[tilesToDestroy.Count - 1] && i - 1 >= 0 && allActualTiles[i - 1, j] != null)
                    {
                        allActualTiles[i - 1, j] = null;
                    }
                    else if (directionOfSwipe == "right" && allActualTiles[i, j] == tilesToDestroy[0] && i + 1 < width && allActualTiles[i + 1, j] != null)
                    {
                        allActualTiles[i + 1, j] = null;
                    }
                    else
                    {
                        allActualTiles[i, j] = null;
                    }
                }
            }
        }

        CollapseTiles(duplicateTilesToDestroy);

        for (int i = 0; i < tilesToDestroy.Count; i++)
        {
            if (tilesToDestroy[i] != null)
            {
                GameObject piece = tilesToDestroy[i];
                TileAnimator tileAnimator = piece.GetComponent<TileAnimator>();

                // Subscribe to animation completion event
                tileAnimator.AnimationCompleted += () =>
                {
                    Destroy(piece); // Destroy the tile after the animation is completed
                };

                tileAnimator.PlayAnimation(piece, referenceToDestroyedTiles.Count);
            }
            isTilesBroken = true;
        }

        scoreManager.HandleScore(referenceToDestroyedTiles.Count);
    }

    public void CollapseTiles(List<GameObject> destroyedTiles)
    {
        // First pass: Mark the tiles that need to be moved
        for (int i = 0; i < width; i++)
        {
            int emptySpaces = 0;
            for (int j = 0; j < height; j++)
            {
                if (allActualTiles[i, j] == null)
                {
                    emptySpaces++;
                }
                else if (emptySpaces > 0)
                {
                    // Mark the tile to be moved
                    allActualTiles[i, j].GetComponent<Tile>().rowForCheckingMatches = j - emptySpaces;
                }
            }
        }

        // Second pass: Move the marked tiles to their new positions
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allActualTiles[i, j] == null)
                {
                    // Find the first non-null tile above this empty space
                    for (int k = j + 1; k < height; k++)
                    {
                        if (allActualTiles[i, k] != null)
                        {
                            // Calculate the new position for the tile after collapsing
                            Vector3 newPosition = allActualTiles[i, k].transform.position - new Vector3(0, (j - k) * -17f, 0);

                            // Animate the tile to its new position
                            LeanTween.move(allActualTiles[i, k], newPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);

                            // Update the reference to the tile in the grid
                            allActualTiles[i, j] = allActualTiles[i, k];
                            allActualTiles[i, k] = null;

                            break;
                        }
                    }
                }
            }
        }
    }

    public void RefillingTiles()
    {
        float initialX = -493f;
        float initialY = 197f;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allActualTiles[i, j] == null)
                {
                    // Calculate the initial position based on grid position and offsets
                    Vector2 initialPosition = new Vector2(initialX + (i * 138f), initialY - (j * 105f));

                    int tilesToUse = Random.Range(0, tiles.Length);
                    GameObject piece = Instantiate(tiles[tilesToUse], Vector3.zero, Quaternion.identity, actualGrid);

                    // Set the anchored position of the RectTransform
                    RectTransform pieceRectTransform = piece.GetComponent<RectTransform>();
                    pieceRectTransform.anchoredPosition = initialPosition;

                    // Use LeanTween to move the instantiated prefab to the correct position
                    Vector2 targetPosition = positionsOfTiles[i, j];
                    LeanTween.move(pieceRectTransform, targetPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);

                    allActualTiles[i, j] = piece;
                }
            }
        }

        CheckForMatchesBeforeNextMove();

        CorrectNames();
    }

    public void CheckForMatchesBeforeNextMove()
    {
        List<GameObject> tilesToDestroyBeforeNextMove = new List<GameObject>();

        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allActualTiles[i, j] != null)
                {
                    string currentTileTag = allActualTiles[i, j].tag;
                    if(i > 0 && i < width - 1)
                    {
                        if(allActualTiles[i - 1, j] != null && allActualTiles[i + 1, j] != null)
                        {
                            if(allActualTiles[i - 1, j].tag == currentTileTag && allActualTiles[i + 1, j].tag == currentTileTag)
                            {
                                tilesToDestroyBeforeNextMove.Add(allActualTiles[i - 1, j]);
                                tilesToDestroyBeforeNextMove.Add(allActualTiles[i, j]);
                                tilesToDestroyBeforeNextMove.Add(allActualTiles[i + 1, j]);
                            }
                        }
                    }

                    if (j > 0 && j < width - 1)
                    {
                        if (allActualTiles[i, j - 1] != null && allActualTiles[i, j + 1] != null)
                        {
                            if (allActualTiles[i, j - 1].tag == currentTileTag && allActualTiles[i, j + 1].tag == currentTileTag)
                            {
                                tilesToDestroyBeforeNextMove.Add(allActualTiles[i, j - 1]);
                                tilesToDestroyBeforeNextMove.Add(allActualTiles[i, j]);
                                tilesToDestroyBeforeNextMove.Add(allActualTiles[i, j + 1]);
                            }
                        }
                    }
                }
            }
        }
    }

    public void CorrectNames()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                allActualTiles[i, j].name = i + "," + j;
            }
        }
    }
}
