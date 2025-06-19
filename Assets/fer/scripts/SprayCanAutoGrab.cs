using UnityEngine;

public class SprayCanAutoGrab : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotateSpeed = 5f;
    public float snapDistance = 0.05f;
    public bool alignRotation = true;

    private Transform targetHand;
    private bool isMoving = false;

private void Update()
{
    if (!isMoving || targetHand == null) return;

    // OFFSET local: +X = más cerca (eje rojo), +Z = más adelante (eje azul)
    Vector3 localOffset = new Vector3(-0.05f, 0f, 0.2f); // ajusta estos valores según tu escala

    // Convert local offset to world space
    Vector3 targetPosition = targetHand.TransformPoint(localOffset);
    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

    if (alignRotation)
    {
        // Orientar con: forward = hand.forward (azul), up = hand.right (rojo)
        Quaternion targetRotation = Quaternion.LookRotation(-targetHand.forward, targetHand.right);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
    }

    if (Vector3.Distance(transform.position, targetPosition) < snapDistance)
    {
        isMoving = false;
    }
}

    /// <summary>
    /// Llamar este método desde un gesture para mover el objeto a la mano.
    /// </summary>
    public void MoveToHand(Transform hand)
    {
        if (hand == null) return;

        targetHand = hand;
        isMoving = true;
    }
}