using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SwipeMenuCities : MonoBehaviour, IDragHandler, IEndDragHandler
{
    public float thresholdPercentage = 0.2f; // Adjust this value to control the swipe threshold

    private Vector2 startPosition;
    private Vector2 endPosition;
    private float swipeThreshold;

    private RectTransform content;

    private void Start()
    {
        content = GetComponent<ScrollRect>().content;
        swipeThreshold = Mathf.Abs(content.rect.width * thresholdPercentage);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Calculate the swipe distance
        endPosition = eventData.position;
        float swipeDistance = (endPosition.x - startPosition.x);

        // Move the content based on the swipe distance
        content.anchoredPosition += new Vector2(swipeDistance, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Calculate the swipe distance
        endPosition = eventData.position;
        float swipeDistance = (endPosition.x - startPosition.x);

        // Check if the swipe distance exceeds the threshold
        if (Mathf.Abs(swipeDistance) >= swipeThreshold)
        {
            // Determine the direction of the swipe
            int direction = (swipeDistance > 0) ? -1 : 1;

            // Calculate the new position of the content based on the direction
            float newPosition = content.anchoredPosition.x + (content.rect.width * direction);

            // Move the content to the new position
            content.anchoredPosition = new Vector2(newPosition, content.anchoredPosition.y);
        }
        else
        {
            // Snap the content back to its original position
            content.anchoredPosition = new Vector2(startPosition.x, content.anchoredPosition.y);
        }
    }
}
