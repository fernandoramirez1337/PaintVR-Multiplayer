/*
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class XRHandDraw : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform tip;
    [SerializeField] private Grabbable grabbable;
    [SerializeField] private Material drawingMaterial;
    [SerializeField] private AudioSource drawSound;

    [Header("Drawing Settings")]
    [SerializeField] private float minDistanceBeforeNewPoint = 0.008f;
    [SerializeField] private float tubeWidth = 0.01f;
    [SerializeField] private int tubeSides = 8;

    private Vector3 prevPoint = Vector3.zero;
    private List<Vector3> currentPoints = new();
    private TubeRenderer currentSegment;
    private GameObject currentStroke;
    private bool isDrawing = false;
    private Color lastColor;

    void Update()
    {
        if (!isDrawing) return;

        if (drawingMaterial.color != lastColor)
        {
            CreateNewSegment();
        }

        UpdateTube();
    }

    public void GestureStartDrawing()
    {
        if (grabbable == null || grabbable.SelectingPointsCount == 0 || tip == null) return;
        if (!DrawingZoneManager.Instance.IsPointInsideActiveZone(tip.position)) return;

        currentStroke = new GameObject($"Stroke_{Time.time}");

        // PARENTEAR AL CUBO ACTIVO
        GameObject cube = CubeManager.Instance.GetCurrentCube();
        if (cube != null)
        {
            currentStroke.transform.SetParent(cube.transform);
        }

        CreateNewSegment();
        isDrawing = true;
        drawSound?.Play();
    }

    public void GestureStopDrawing()
    {
        if (!isDrawing) return;

        isDrawing = false;
        drawSound?.Stop();

        if (currentStroke != null)
        {
            ActionManager.Instance.RecordDraw(currentStroke);
        }
    }

    private void CreateNewSegment()
    {
        GameObject go = new($"TubeSegment_{Time.time}");
        go.transform.SetParent(currentStroke.transform);

        TubeRenderer tube = go.AddComponent<TubeRenderer>();
        tube._sides = tubeSides;
        tube._radiusOne = tubeWidth;
        tube._radiusTwo = tubeWidth;

        Material matInstance = new Material(drawingMaterial);
        tube.material = matInstance;
        tube.ColliderTrigger = false;

        currentPoints = new();
        currentSegment = tube;
        lastColor = drawingMaterial.color;
    }

    private void UpdateTube()
    {
        if (!DrawingZoneManager.Instance.IsPointInsideActiveZone(tip.position)) return;

        Vector3 point = tip.position;

        if (currentPoints.Count == 0 || Vector3.Distance(prevPoint, point) >= minDistanceBeforeNewPoint)
        {
            prevPoint = point;
            currentPoints.Add(point);
            currentSegment.SetPositions(currentPoints.ToArray());
            currentSegment.GenerateMesh();
        }
    }

    public void SetDrawingMaterial(Material newMat)
    {
        drawingMaterial = newMat;
    }

    public void SetTip(Transform newTip)
    {
        tip = newTip;
    }

    public void SetLineWidth(float width)
    {
        tubeWidth = width;
    }
}
*/