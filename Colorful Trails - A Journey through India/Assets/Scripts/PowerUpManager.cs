using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    public RectTransform powerUpLiquid; // Reference to the RectTransform component of the power-up liquid
    private int matchCount = 0; // Number of match pairs found

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the initial position and height of the power-up liquid
        powerUpLiquid.anchoredPosition = new Vector2(powerUpLiquid.anchoredPosition.x, -150f);
        powerUpLiquid.sizeDelta = new Vector2(powerUpLiquid.sizeDelta.x, 41f);
    }

    // Call this method when a match pair is found
    public void IncreaseLiquid()
    {
        matchCount++;

        // Calculate the new height and y position values based on the match count
        float newHeightIncrement = 80f;
        float newYPositionIncrement = 90f;

        Vector2 currentSize = powerUpLiquid.sizeDelta;
        Vector2 currentPosition = powerUpLiquid.anchoredPosition;

        float newHeight = currentSize.y + newHeightIncrement;
        float newYPosition = currentPosition.y + newYPositionIncrement;

        // Round the values to a certain number of decimal places
        int decimalPlaces = 2;
        newHeight = Mathf.Round(newHeight * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
        newYPosition = Mathf.Round(newYPosition * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);

        // Apply the new height and y position values to the power-up liquid
        powerUpLiquid.sizeDelta = new Vector2(currentSize.x, newHeight);
        powerUpLiquid.anchoredPosition = new Vector2(currentPosition.x, newYPosition);

        // Check if the match count has reached 3, then reset it
        if (matchCount >= 3)
        {
            matchCount = 0;
            newHeight = 356f;
            newYPosition = 60f;
            powerUpLiquid.sizeDelta = new Vector2(currentSize.x, newHeight);
            powerUpLiquid.anchoredPosition = new Vector2(currentPosition.x, newYPosition);
        }
    }


}

