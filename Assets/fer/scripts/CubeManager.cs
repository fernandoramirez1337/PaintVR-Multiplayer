using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    public static CubeManager Instance { get; private set; }

    [Header("Cube Settings")]
    [Tooltip("Prefab del cubo que define una zona de dibujo.")]
    public GameObject cubePrefab;

    [Tooltip("Distancia desde la palma donde se generará el cubo.")]
    public float spawnDistance = 0.02f;

    [Tooltip("Transform de la palma izquierda.")]
    public Transform leftPalmTransform;

    [Tooltip("Transform de la palma derecha.")]
    public Transform rightPalmTransform;

    private readonly List<GameObject> cubes = new();
    private GameObject currentCube;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    /// <summary>
    /// Llamado desde un gesto para crear un nuevo cubo desde una palma específica.
    /// </summary>
    /// <param name="isRightHand">True si es la palma derecha, False si es la izquierda.</param>
    public void OnGesturePerformed(bool isRightHand)
    {
        Transform palm = isRightHand ? rightPalmTransform : leftPalmTransform;
        GameObject cube = SpawnCubeAtPalm(palm);
        if (cube != null)
        {
            SetActiveCube(cube);
            ActionManager.Instance.RecordLayer(cube);
        }
    }

    /// <summary>
    /// Instancia un nuevo cubo en la posición de la palma especificada.
    /// </summary>
    private GameObject SpawnCubeAtPalm(Transform palm)
    {
        if (cubePrefab == null || palm == null)
        {
            Debug.LogWarning("CubeManager: cubePrefab o alguna palma no asignada.");
            return null;
        }

        Vector3 spawnPos = palm.position - palm.up * spawnDistance;
        GameObject newCube = Instantiate(cubePrefab, spawnPos, Quaternion.identity);
        cubes.Add(newCube);
        return newCube;
    }

    public void RemoveCube(GameObject cube)
    {
        if (cube == null) return;

        if (currentCube == cube)
        {
            DrawingZoneManager.Instance.ClearActiveZone();
            currentCube = null;
        }

        cubes.Remove(cube);
        Destroy(cube);
    }

    public void SetActiveCube(GameObject cube)
    {
        if (cube == null) return;

        if (!cubes.Contains(cube))
            cubes.Add(cube);

        currentCube = cube;

        if (cube.TryGetComponent(out BoxCollider zone))
        {
            DrawingZoneManager.Instance.SetActiveZone(zone);
        }
        else
        {
            Debug.LogWarning("CubeManager: el cubo no tiene BoxCollider.");
        }
    }

    public void AddCube(GameObject cube)
    {
        if (cube != null && !cubes.Contains(cube))
        {
            cubes.Add(cube);
        }
    }

    public GameObject GetCurrentCube() => currentCube;
}