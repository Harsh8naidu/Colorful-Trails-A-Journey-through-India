using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockAnimation : MonoBehaviour
{
    public Transform minuteHandTransform;
    public Transform hourHandTransform;
    public float minuteRotationSpeed = 360f / 60f;  // Degrees per second
    public float hourRotationSpeed = 360f / (60f * 6f);  // Degrees per second

    private void Update()
    {
        // Calculate the current rotation for each hand
        float minuteRotation = -Time.realtimeSinceStartup * minuteRotationSpeed;
        float hourRotation = -Time.realtimeSinceStartup * hourRotationSpeed;

        // Apply the rotations to the corresponding transforms
        minuteHandTransform.eulerAngles = new Vector3(0f, 0f, minuteRotation);
        hourHandTransform.eulerAngles = new Vector3(0f, 0f, hourRotation);
    }
}

