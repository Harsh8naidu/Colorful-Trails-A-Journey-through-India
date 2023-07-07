using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenLiquidAnimation : MonoBehaviour
{
    public Transform goldenLiquidImage;

    private float initialPositionX = 295.44f;
    private float finalPositionX = 249.0629f;
    public float duration = 1f;

    private void Start()
    {
        StartCoroutine(MoveSprite());
    }

    private IEnumerator MoveSprite()
    {
        float elapsedTime = 0f;

        while (true)
        {
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                float newX = Mathf.Lerp(initialPositionX, finalPositionX, t);
                Vector3 newPosition = new Vector3(newX, goldenLiquidImage.position.y, goldenLiquidImage.position.z);
                goldenLiquidImage.position = newPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Swap initial and final positions
            float temp = initialPositionX;
            initialPositionX = finalPositionX;
            finalPositionX = temp;

            elapsedTime = 0f;
        }
    }
}
