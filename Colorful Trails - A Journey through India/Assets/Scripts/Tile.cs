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

    public enum SwipeDirection
    {
        Up,
        Down,
        Left,
        Right
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
        if (nameParts.Length == 2 && int.TryParse(nameParts[0].Trim(), out int row) && int.TryParse(nameParts[1].Trim(), out int column))
        {
            // Calculate the name of the other tile based on the swipe direction
            int otherColumn = column;
            if (direction == SwipeDirection.Up) // Up
            {
                otherColumn += 1;
            }
            else if (direction == SwipeDirection.Down) // Down
            {
                otherColumn -= 1;
            }
            int otherRow = row;
            if (direction == SwipeDirection.Right) // Right
            {
                otherRow += 1;
            }
            else if (direction == SwipeDirection.Left) // Left
            {
                otherRow -= 1;
            }
            otherTileName = otherRow.ToString() + "," + otherColumn.ToString();

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
                Debug.Log(tile.name);
                Debug.Log(otherTile.name);
            }
        }
        else
        {
            // Handle invalid tile name format
            Debug.LogWarning("Invalid tile name format: " + tileName);
        }
    }
}
