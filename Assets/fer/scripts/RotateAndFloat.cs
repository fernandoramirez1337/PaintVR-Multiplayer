using UnityEngine;

/// <summary>
/// This script makes a GameObject slowly rotate around its Y-axis
/// while smoothly floating up and down.
/// </summary>
public class RotateAndFloat : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("The speed at which the object rotates.")]
    public float rotationSpeed = 20f;

    [Header("Floating Settings")]
    [Tooltip("The maximum height the object will float up and down from its starting point.")]
    public float floatStrength = 0.25f;

    [Tooltip("The speed of the floating motion.")]
    public float floatSpeed = 1f;

    // We store the object's starting position to calculate the float from.
    private Vector3 startPosition;

    /// <summary>
    /// This method is called once when the script instance is first loaded.
    /// </summary>
    void Start()
    {
        // Store the initial position of the GameObject.
        startPosition = transform.position;
    }

    /// <summary>
    /// This method is called once per frame.
    /// </summary>
    void Update()
    {
        // --- ROTATION ---
        // Rotate the object around its own up-axis (Y-axis).
        // Time.deltaTime makes the rotation smooth and independent of the frame rate.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // --- FLOATING ---
        // To create a smooth up and down motion, we use a sine wave.
        // Mathf.Sin() returns a value that oscillates smoothly between -1 and 1.
        // We multiply Time.time by floatSpeed to control the frequency of the wave.
        // We multiply the result by floatStrength to control the amplitude (how high and low it goes).
        float floatY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;

        // Apply the floating motion by creating a new position vector.
        // We start with the original position and add our calculated float value to the Y-coordinate.
        transform.position = startPosition + new Vector3(0, floatY, 0);
    }
}
