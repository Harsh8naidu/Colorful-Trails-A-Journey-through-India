using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using static LeanTween;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    private GameObject tile;
    private GameObject otherTile;
    string otherTileName;
    Vector2 swipeStart;
    Vector2 swipeEnd;
    float minimumDistance = 10f;
    public bool isMatched = false;
    private GenerateGrid grid;
    public int rowForCheckingMatches;
    public int columnForCheckingMatches;

    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    private void Start()
    {
        grid = FindObjectOfType<GenerateGrid>();
    }

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                swipeStart = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                swipeEnd = touch.position;
            }
        }

        // mouse touch simulation
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            swipeEnd = Input.mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        tile = eventData.pointerCurrentRaycast.gameObject;
        GetSwipeDirection();
    }

    private void GetSwipeDirection()
    {
        float distance = Vector2.Distance(swipeStart, swipeEnd);
        if (distance > minimumDistance)
        {
            if (IsVerticalSwipe())
            {
                if (swipeEnd.y > swipeStart.y) // vertical
                {
                    // Up
                    ProcessSwipe(SwipeDirection.Up);
                }
                else
                {
                    // Down
                    ProcessSwipe(SwipeDirection.Down);
                }
            }
            else // horizontal
            {
                if (swipeEnd.x > swipeStart.x)
                {
                    // Right
                    ProcessSwipe(SwipeDirection.Right);
                }
                else
                {
                    // Left
                    ProcessSwipe(SwipeDirection.Left);
                }
            }
        }
    }

    bool IsVerticalSwipe()
    {
        float vertical = Mathf.Abs(swipeEnd.y - swipeStart.y);
        float horizontal = Mathf.Abs(swipeEnd.x - swipeStart.x);

        if (vertical > horizontal)
            return true;
        return false;
    }

    public void ProcessSwipe(SwipeDirection direction)
    {
        // Get the name of the current tile
        string tileName = tile.name;

        // Parse the row and column values from the tile name
        string[] nameParts = tileName.Split(',');
        if (nameParts.Length == 2 && int.TryParse(nameParts[0].Trim(), out int column) && int.TryParse(nameParts[1].Trim(), out int row))
        {
            // Calculate the name of the other tile based on the swipe direction
            int otherRow = row;
            if (direction == SwipeDirection.Up) // Up
            {
                otherRow += 1;
            }
            else if (direction == SwipeDirection.Down) // Down
            {
                otherRow -= 1;
            }
            int otherColumn = column;
            if (direction == SwipeDirection.Right) // Right
            {
                otherColumn += 1;
            }
            else if (direction == SwipeDirection.Left) // Left
            {
                otherColumn -= 1;
            }
            otherTileName = otherColumn.ToString() + "," + otherRow.ToString();

            otherTile = GameObject.Find(otherTileName);

            // Perform any actions with the other tile
            if (otherTile != null)
            {
                // Store the initial positions of the tiles
                Vector3 tileInitialPosition = tile.transform.position;
                Vector3 otherTileInitialPosition = otherTile.transform.position;

                // Animate the movement of the tiles
                LeanTween.move(tile, otherTileInitialPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);
                LeanTween.move(otherTile, tileInitialPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);

                string tempTile = tile.name;
                tile.name = otherTile.name;
                otherTile.name = tempTile;

                

                FindMatches(row, column, otherRow, otherColumn, tile, otherTile, tileInitialPosition, otherTileInitialPosition);
            }
        }
        else
        {
            // Handle invalid tile name format
            Debug.LogWarning("Invalid tile name format: " + tileName);
        }
    }

    private void FindMatches(int row, int column, int otherRow, int otherColumn, GameObject tile, GameObject otherTile, Vector3 tileInitialPosition, Vector3 otherTileInitialPosition)
    {
        string matchedTileName = "";
        string tileTag = tile.tag;
        string otherTileTag = otherTile.tag;
        GameObject matchedTile = null;
        bool isTilesDestroyed = false;
        GameObject[] tilesToBreak = new GameObject[grid.height];
        GameObject[] otherTilesToBreak = new GameObject[grid.height];
        int numberOfTilesToDestory = 0;
        int checkAdjacentRight = 1;
        int checkAdjacentRight1 = 1;
        int checkAdjacentLeft = 1;
        int checkAdjacentLeft1 = 1;
        int checkAdjacentDown = 1;
        int checkAdjacentDown1 = 1;
        int checkAdjacentUp = 1;
        int checkAdjacentUp1 = 1;

        tilesToBreak[0] = tile;

        // Finding matches for tile
        // Check for horizontal matching tiles
        for (int i = otherColumn; i < grid.width; i++) // Right tiles
        {
            matchedTileName = i + "," + otherRow;
            matchedTile = GameObject.Find(matchedTileName);
            string matchedTileTag = matchedTile.tag;

            if (matchedTileTag == tileTag && checkAdjacentRight == checkAdjacentRight1)
            {
                tilesToBreak[numberOfTilesToDestory] = matchedTile;
                checkAdjacentRight++;
                numberOfTilesToDestory++;
            }
            checkAdjacentRight1++;
        }

        for (int i = otherColumn - 1; i > 0; i--) // Left tiles
        {
            matchedTileName = i + "," + otherRow;
            matchedTile = GameObject.Find(matchedTileName);
            string matchedTileTag = matchedTile.tag;

            if (matchedTileTag == tileTag && checkAdjacentLeft == checkAdjacentLeft1)
            {
                tilesToBreak[numberOfTilesToDestory] = matchedTile;
                checkAdjacentLeft++;
                numberOfTilesToDestory++;
            }
            checkAdjacentLeft1++;
        }

        // Destroying Horizontal matching tiles
        if(numberOfTilesToDestory >= 3)
        {
            for (int i = 0; i < numberOfTilesToDestory; i++)
            {
                Destroy(tilesToBreak[i]);
                int[] tileCoordinates = GetTileCoordinates(tilesToBreak[i]);
                grid.allActualTiles[tileCoordinates[0], tileCoordinates[1]] = null;
            }

            isTilesDestroyed = true;
        }

        numberOfTilesToDestory = 0;

        // Finding Vertical matching tiles
        for (int i = otherRow; i < grid.width; i++) // Up tiles
        {
            matchedTileName = otherColumn + "," + i;
            matchedTile = GameObject.Find(matchedTileName);
            string matchedTileTag = matchedTile.tag;

            if (matchedTileTag == tileTag && checkAdjacentUp == checkAdjacentUp1)
            {
                tilesToBreak[numberOfTilesToDestory] = matchedTile;
                checkAdjacentUp++;
                numberOfTilesToDestory++;
            }
            checkAdjacentUp1++;
        }

        for (int i = otherRow - 1; i > 0; i--) // Down tiles
        {
            matchedTileName = otherColumn + "," + i;
            matchedTile = GameObject.Find(matchedTileName);
            string matchedTileTag = matchedTile.tag;

            if (matchedTileTag == tileTag && checkAdjacentDown == checkAdjacentDown1)
            {
                tilesToBreak[numberOfTilesToDestory] = matchedTile;
                checkAdjacentDown++;
                numberOfTilesToDestory++;
            }
            checkAdjacentDown1++;
        }

        // Destroying Vertical matching tiles
        if(numberOfTilesToDestory >= 3)
        {
            for (int i = 0; i < numberOfTilesToDestory; i++)
            {
                Destroy(tilesToBreak[i]);
                int[] tileCoordinates = GetTileCoordinates(tilesToBreak[i]);
                grid.allActualTiles[tileCoordinates[0], tileCoordinates[1]] = null;
            }

            isTilesDestroyed = true;
        }

        if (!isTilesDestroyed)
        {
            // Animate the movement of the tiles
            LeanTween.move(tile, tileInitialPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.move(otherTile, otherTileInitialPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);
        }

        if (isTilesDestroyed)
        {
            grid.CollapseTiles();
        }
    }

    // Inside the Tile script
    private int[] GetTileCoordinates(GameObject tileObject)
    {
        if(tileObject != null)
        {
            string tileName = tileObject.name;
            string[] nameParts = tileName.Split(',');
            if (nameParts.Length == 2 && int.TryParse(nameParts[0].Trim(), out int column) && int.TryParse(nameParts[1].Trim(), out int row))
            {
                return new int[] { column, row };
            }
        }
        return new int[] { -1, -1 };
    }

}
