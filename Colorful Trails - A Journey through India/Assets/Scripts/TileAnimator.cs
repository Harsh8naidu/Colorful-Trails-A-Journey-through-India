using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LeanTween;
using UnityEngine;

public class TileAnimator : MonoBehaviour
{
    private RectTransform powerUpBubble;
    public delegate void AnimationCompletedDelegate();
    public event AnimationCompletedDelegate AnimationCompleted;

    private void Awake()
    {
        // Assuming the TileAnimator script is attached to a tile GameObject
        // and the PowerUpBubble is a child of the Canvas
        powerUpBubble = GameObject.Find("Canvas/PowerUpBubbleImage").GetComponent<RectTransform>();
    }

    public void PlayAnimation(GameObject tileToMove, int tilesCount)
    {
        if (powerUpBubble == null)
        {
            Debug.LogError("PowerUpBubble not found.");
            return;
        }

        if(tilesCount >= 4)
        {
            Vector3 targetPosition = powerUpBubble.position;

            LeanTween.move(tileToMove, targetPosition, 0.5f)
                .setEase(LeanTweenType.easeOutQuad)
                .setOnComplete(() =>
                {
                // Animation completed, trigger the event
                AnimationCompleted?.Invoke();
                });
        }
    }
}

