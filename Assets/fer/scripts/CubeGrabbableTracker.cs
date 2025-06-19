/*
using UnityEngine;
using Oculus.Interaction;

[RequireComponent(typeof(Grabbable))]
public class CubeGrabbableTracker : MonoBehaviour
{
    private Grabbable grabbable;

    private void Awake()
    {
        grabbable = GetComponent<Grabbable>();
    }

    private void OnEnable()
    {
        grabbable.WhenPointerEventRaised += OnGrabEvent;
    }

    private void OnDisable()
    {
        grabbable.WhenPointerEventRaised -= OnGrabEvent;
    }

    private void OnGrabEvent(PointerEvent evt)
    {
        if (evt.Type == PointerEventType.Select)
        {
            if (TryGetComponent(out BoxCollider box))
            {
                if (DrawingZoneManager.Instance != null)
                {
                    DrawingZoneManager.Instance.SetActiveZone(box);
                }

                if (CubeManager.Instance != null)
                {
                    CubeManager.Instance.SetActiveCube(gameObject);
                }
            }
        }
    }
}
*/