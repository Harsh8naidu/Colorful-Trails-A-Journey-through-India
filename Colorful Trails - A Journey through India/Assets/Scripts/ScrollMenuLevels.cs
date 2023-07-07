using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollMenuLevels : MonoBehaviour
{
    // Public variables

    public RectTransform panel; // To hold the ScrollPanel
    public Button[] levels; // To hold the cities as buttons
    public RectTransform center; // Center to compare the distance for each button

    // Private variables

    private float[] distance;
    private bool dragging = false; // Will be true while we drag the panel
    private int levelsDistance; // Will hold the distance between the buttons
    private int minLevelNum; // To hold the number of the button, with smallest distance to center

    private void Start()
    {
        int levelsLength = levels.Length;
        distance = new float[levelsLength];

        // Get distance between buttons
        levelsDistance = (int)Mathf.Abs(levels[1].GetComponent<RectTransform>().anchoredPosition.x - levels[0].GetComponent<RectTransform>().anchoredPosition.x);
    }

    void Update()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            distance[i] = Mathf.Abs(center.transform.position.x - levels[i].transform.position.x);
        }

        float minDistance = Mathf.Min(distance); // Get the min distance

        for (int index = 0; index < levels.Length; index++)
        {
            if (minDistance == distance[index])
            {
                minLevelNum = index;
            }
        }

        if (!dragging)
        {
            LerpToButton(minLevelNum * -levelsDistance);
        }

    }

    void LerpToButton(int position)
    {
        float minPosition = -((levels.Length - 1) * levelsDistance);
        float maxPosition = 0;

        // Clamp the position between the leftmost and rightmost positions
        float newX = Mathf.Clamp(position, minPosition, maxPosition);
        Vector2 newPosition = new Vector2(Mathf.Lerp(panel.anchoredPosition.x, newX, Time.deltaTime * 5f), panel.anchoredPosition.y);

        panel.anchoredPosition = newPosition;

    }

    public void StartDrag()
    {
        dragging = true;
    }

    public void EndDrag()
    {
        dragging = false;
    }
}
