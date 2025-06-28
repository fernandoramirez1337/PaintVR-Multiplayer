// BarController.cs
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class BarController : MonoBehaviour
{
    [Header("UI Elements")]
    private VisualElement ui;
    private Button prevButton;
    private Button nextButton;
    private Button undoButton;

    [Header("Replica Zone Control")]
    public List<GameObject> replicaZones;

    private int currentIndex = 0;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void Start()
    {
        UpdateActiveZone();
    }

    private void OnEnable()
    {
        prevButton = ui.Q<Button>("PrevButton");
        nextButton = ui.Q<Button>("NextButton");
        undoButton = ui.Q<Button>("UndoButton");

        if (prevButton != null) prevButton.clicked += OnPrevButtonClicked;
        if (nextButton != null) nextButton.clicked += OnNextButtonClicked;
        if (undoButton != null) undoButton.clicked += OnUndoButtonClicked;
    }

    private void OnDisable()
    {
        if (prevButton != null) prevButton.clicked -= OnPrevButtonClicked;
        if (nextButton != null) nextButton.clicked -= OnNextButtonClicked;
        if (undoButton != null) undoButton.clicked -= OnUndoButtonClicked;
    }

    private void OnNextButtonClicked()
    {
        currentIndex++;
        if (currentIndex >= replicaZones.Count) currentIndex = 0;
        UpdateActiveZone();
    }

    private void OnPrevButtonClicked()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = replicaZones.Count - 1;
        UpdateActiveZone();
    }
    
    private void OnUndoButtonClicked()
    {
        // --- SOLUCIÓN DEFINITIVA: Enviar una petición al servidor ---
        if (DrawingZoneManager.Instance != null)
        {
            Debug.Log($"[BarController] Cliente solicitando deshacer en la zona {currentIndex}.");
            // Este RPC puede ser llamado por cualquier cliente y se ejecutará en el servidor.
            DrawingZoneManager.Instance.RequestUndoServerRpc(currentIndex);
        }
        else
        {
            Debug.LogError("[BarController] No se encuentra la instancia de DrawingZoneManager.");
        }
    }

    private void UpdateActiveZone()
    {
        if (replicaZones == null || replicaZones.Count == 0) return;
        
        for (int i = 0; i < replicaZones.Count; i++)
        {
            if (replicaZones[i] != null)
            {
                replicaZones[i].SetActive(i == currentIndex);
            }
        }
    }
}