using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScrollMenuCities : MonoBehaviour
{
    // Public variables

    public RectTransform panel; // To hold the ScrollPanel
    public Button[] cities; // To hold the cities as buttons
    public RectTransform center; // Center to compare the distance for each button

    // Private variables

    private float[] distance;
    private bool dragging = false; // Will be true while we drag the panel
    private int citiesDistance; // Will hold the distance between the buttons
    private int minCityNum; // To hold the number of the button, with smallest distance to center

    private void Start()
    {
        int citiesLength = cities.Length;
        distance = new float[citiesLength];

        // Get distance between buttons
        citiesDistance = (int)Mathf.Abs(cities[1].GetComponent<RectTransform>().anchoredPosition.x - cities[0].GetComponent<RectTransform>().anchoredPosition.x);
        Debug.Log(citiesDistance);
    }

    void Update()
    {
        for(int i = 0; i < cities.Length; i++)
        {
            distance[i] = Mathf.Abs(center.transform.position.x - cities[i].transform.position.x);
        }

        float minDistance = Mathf.Min(distance);

        for(int index = 0; index < cities.Length; index++)
        {
            if(minDistance == distance[index])
            {
                minCityNum = index;
            }
        }

        if (!dragging)
        {
            LerpToButton(minCityNum * citiesDistance);
        }

    }

    void LerpToButton(float position)
    {
        float newX = Mathf.Lerp(panel.anchoredPosition.x, position, Time.deltaTime * 5f);
        Vector2 newPosition = new Vector2(newX, panel.anchoredPosition.y);

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
