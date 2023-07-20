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
    public int columnForCheckingMatches;
    public int rowForCheckingMatches;
    private GenerateGrid grid;
    private int columnForMatches;
    private int rowForMatches;

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
        string directionOfSwipe = "";

        // Parse the row and column values from the tile name
        string[] nameParts = tileName.Split(',');
        if (nameParts.Length == 2 && int.TryParse(nameParts[0].Trim(), out int column) && int.TryParse(nameParts[1].Trim(), out int row))
        {
            // Calculate the name of the other tile based on the swipe direction
            int otherRow = row;
            if (direction == SwipeDirection.Up) // Up
            {
                otherRow += 1;
                directionOfSwipe = "up";
            }
            else if (direction == SwipeDirection.Down) // Down
            {
                otherRow -= 1;
                directionOfSwipe = "down";
            }
            int otherColumn = column;
            if (direction == SwipeDirection.Right) // Right
            {
                otherColumn += 1;
                directionOfSwipe = "right";
            }
            else if (direction == SwipeDirection.Left) // Left
            {
                otherColumn -= 1;
                directionOfSwipe = "left";
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
        GameObject matchedTile = null;
        GameObject[] tilesToBreak = new GameObject[grid.height];
        GameObject[] otherTilesToBreak = new GameObject[grid.height];
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

            if (matchedTile.tag == tile.tag && checkAdjacentRight == checkAdjacentRight1)
            {
                tilesToBreak[checkAdjacentRight] = matchedTile;
                checkAdjacentRight++;
            }
            checkAdjacentRight1++;
        }

        if (checkAdjacentRight == 3 || checkAdjacentRight > 3)
        {
            Debug.Log(checkAdjacentRight);
            for (int j = 0; j < checkAdjacentRight; j++)
            {
                Destroy(tilesToBreak[j]);
            }
        }

        for (int i = otherColumn - 1; i >= 0; i--) // Left tiles
        {
            matchedTileName = i + "," + otherRow;
            matchedTile = GameObject.Find(matchedTileName);

            if (matchedTile.tag == tile.tag && checkAdjacentLeft == checkAdjacentLeft1)
            {
                tilesToBreak[checkAdjacentLeft] = matchedTile;
                if(checkAdjacentRight >= 2)
                {
                    Destroy(tilesToBreak[checkAdjacentLeft]);
                }
                checkAdjacentLeft++;
            }
            checkAdjacentLeft1++;
        }

        if (checkAdjacentLeft == 3 || checkAdjacentLeft > 3)
        {
            Debug.Log(checkAdjacentLeft);
            for (int j = 0; j < checkAdjacentLeft; j++)
            {
                Destroy(tilesToBreak[j]);
            }
        }

        // Check for vertical tiles
        for (int i = otherRow; i < grid.height; i++) // Up tiles
        {
            matchedTileName = otherColumn + "," + i;
            matchedTile = GameObject.Find(matchedTileName);

            if (matchedTile.tag == tile.tag && checkAdjacentUp == checkAdjacentUp1)
            {
                tilesToBreak[checkAdjacentUp] = matchedTile;
                checkAdjacentUp++;
            }
            checkAdjacentUp1++;
        }

        if (checkAdjacentUp == 3 || checkAdjacentUp > 3)
        {
            Debug.Log(checkAdjacentUp);
            for (int j = 0; j < checkAdjacentUp; j++)
            {
                Destroy(tilesToBreak[j]);
            }
        }

        for (int i = otherRow - 1; i >= 0; i--) // Down tiles
        {
            matchedTileName = otherColumn + "," + i;
            matchedTile = GameObject.Find(matchedTileName);

            if (matchedTile.tag == tile.tag && checkAdjacentDown == checkAdjacentDown1)
            {
                tilesToBreak[checkAdjacentDown] = matchedTile;
                if (checkAdjacentUp >= 2)
                {
                    Destroy(tilesToBreak[checkAdjacentDown]);
                }
                checkAdjacentDown++;
            }
            checkAdjacentDown1++;
        }

        if (checkAdjacentDown == 3 || checkAdjacentDown > 3)
        {
            Debug.Log(checkAdjacentDown);
            for (int j = 0; j < checkAdjacentDown; j++)
            {
                Destroy(tilesToBreak[j]);
            }
        }

        checkAdjacentRight = 0;
        checkAdjacentRight1 = 0;
        checkAdjacentLeft = 0;
        checkAdjacentLeft1 = 0;
        checkAdjacentDown = 0;
        checkAdjacentDown1 = 0;
        checkAdjacentUp = 0;
        checkAdjacentUp1 = 0;

        // Finding matches for otherTile
        // ***********************************
        // Check for horizontal matching tiles
        for (int i = column; i < grid.width; i++) // Right tiles
        {
            matchedTileName = i + "," + row;
            matchedTile = GameObject.Find(matchedTileName);

            if (matchedTile.tag == otherTile.tag && checkAdjacentRight == checkAdjacentRight1)
            {
                otherTilesToBreak[checkAdjacentRight] = matchedTile;
                checkAdjacentRight++;
            }
            checkAdjacentRight1++;
        }

        if (checkAdjacentRight == 3 || checkAdjacentRight > 3)
        {
            Debug.Log(checkAdjacentRight);
            for (int j = 0; j < checkAdjacentRight; j++)
            {
                Destroy(otherTilesToBreak[j]);
            }
        }

        for (int i = column - 1; i >= 0; i--) // Left tiles
        {
            matchedTileName = i + "," + row;
            matchedTile = GameObject.Find(matchedTileName);

            if (matchedTile.tag == otherTile.tag && checkAdjacentLeft == checkAdjacentLeft1)
            {
                otherTilesToBreak[checkAdjacentLeft] = matchedTile;
                checkAdjacentLeft++;
            }
            checkAdjacentLeft1++;
        }

        if (checkAdjacentLeft == 3 || checkAdjacentLeft > 3)
        {
            Debug.Log(checkAdjacentLeft);
            for (int j = 0; j < checkAdjacentLeft; j++)
            {
                Destroy(otherTilesToBreak[j]);
            }
        }

        // Check for vertical tiles
        for (int i = row; i < grid.height; i++) // Up tiles
        {
            matchedTileName = column + "," + i;
            matchedTile = GameObject.Find(matchedTileName);

            if (matchedTile.tag == otherTile.tag && checkAdjacentUp == checkAdjacentUp1)
            {
                otherTilesToBreak[checkAdjacentUp] = matchedTile;
                checkAdjacentUp++;
            }
            checkAdjacentUp1++;
        }

        if (checkAdjacentUp == 3 || checkAdjacentUp > 3)
        {
            Debug.Log(checkAdjacentUp);
            for (int j = 0; j < checkAdjacentUp; j++)
            {
                Destroy(otherTilesToBreak[j]);
            }
        }

        for (int i = row - 1; i >= 0; i--) // Down tiles
        {
            matchedTileName = column + "," + i;
            matchedTile = GameObject.Find(matchedTileName);

            if (matchedTile.tag == otherTile.tag && checkAdjacentDown == checkAdjacentDown1)
            {
                otherTilesToBreak[checkAdjacentDown] = matchedTile;
                checkAdjacentDown++;
            }
            checkAdjacentDown1++;
        }

        if (checkAdjacentDown == 3 || checkAdjacentDown > 3)
        {
            Debug.Log(checkAdjacentDown);
            for (int j = 0; j < checkAdjacentDown; j++)
            {
                Destroy(otherTilesToBreak[j]);
            }
        }

        /*if(checkAdjacentRight < 3 || checkAdjacentLeft < 3 || checkAdjacentUp < 3 || checkAdjacentDown < 3) 
        {
            // Animate the movement of the tiles
            LeanTween.move(tile, tileInitialPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.move(otherTile, otherTileInitialPosition, 0.3f).setEase(LeanTweenType.easeOutQuad);
        }*/
    }
}
