using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;

public class RadialSelection : MonoBehaviour
{
    [Header("References")]
    public GameObject radialPartPrefab;
    public Transform radialPartCanvas;
    public Transform handTransform;
    public SetColorFromList colorProvider;

    [Header("Visual Settings")]
    public float angleBetweenParts = 10f;
    public float canvasOffsetDistance = 0.15f;
    public Vector3 selectedScale = new Vector3(1.3f, 1.3f, 1.3f);

    [Header("Events")]
    public UnityEvent<int> OnPartSelected;

    private List<GameObject> spawnedParts = new List<GameObject>();
    private int currentSelectedRadialPart = -1;
    private bool isActive = false;
    private int previousSelected = -1;

    public void GesturePerformed()
    {
        if (isActive) return;
        isActive = true;
        SpawnRadialParts();
    }

    public void GestureEnded()
    {
        if (!isActive) return;
        isActive = false;
        HideAndTriggerSelected();
    }

    void Update()
    {
        if (isActive)
        {
            GetSelectedRadialPart();
        }
    }

    private void HideAndTriggerSelected()
    {
        OnPartSelected.Invoke(currentSelectedRadialPart);
        radialPartCanvas.gameObject.SetActive(false);
    }

    private void GetSelectedRadialPart()
    {
        if (colorProvider.colors == null || colorProvider.colors.Count == 0) return;

        Vector3 centerToHand = handTransform.position - radialPartCanvas.position;
        Vector3 projected = Vector3.ProjectOnPlane(centerToHand, radialPartCanvas.forward);
        float angle = Vector3.SignedAngle(radialPartCanvas.up, projected, -radialPartCanvas.forward);
        if (angle < 0) angle += 360f;

        int count = colorProvider.colors.Count;
        int newSelected = Mathf.FloorToInt(angle * count / 360f);

        if (newSelected != currentSelectedRadialPart)
        {
            previousSelected = currentSelectedRadialPart;
            currentSelectedRadialPart = newSelected;

            // ✅ Cambia el color del material objetivo
            colorProvider.SetColor(currentSelectedRadialPart);

            // ✅ Lanza punch scale en el nuevo seleccionado
            if (currentSelectedRadialPart >= 0 && currentSelectedRadialPart < spawnedParts.Count)
            {
                StartCoroutine(PunchScale(spawnedParts[currentSelectedRadialPart].transform));
            }
        }

        for (int i = 0; i < spawnedParts.Count; i++)
        {
            if (spawnedParts[i] == null) continue;
            spawnedParts[i].transform.localScale = (i == currentSelectedRadialPart) ? selectedScale : Vector3.one;
        }
    }

    private IEnumerator PunchScale(Transform t)
    {
        Vector3 original = selectedScale;
        Vector3 punch = selectedScale * 1.2f;

        float duration = 0.1f;
        float time = 0f;

        while (time < duration)
        {
            float tVal = time / duration;
            t.localScale = Vector3.Lerp(punch, original, tVal);
            time += Time.deltaTime;
            yield return null;
        }

        t.localScale = original;
    }

    private void SpawnRadialParts()
    {
        if (colorProvider == null || colorProvider.colors == null || colorProvider.colors.Count == 0)
        {
            Debug.LogError("colorProvider no está asignado o la lista de colores está vacía.");
            return;
        }

        int numberOfRadialParts = colorProvider.colors.Count;

        Vector3 offset = -handTransform.up * canvasOffsetDistance;
        radialPartCanvas.position = handTransform.position + offset;
        radialPartCanvas.rotation = Quaternion.LookRotation(Camera.main.transform.forward, handTransform.up);
        radialPartCanvas.gameObject.SetActive(true);

        foreach (var part in spawnedParts)
        {
            Destroy(part);
        }
        spawnedParts.Clear();

        float fillPerPart = 1f / numberOfRadialParts;
        float fillGap = angleBetweenParts / 360f;
        float finalFill = Mathf.Max(0f, fillPerPart - fillGap);

        for (int i = 0; i < numberOfRadialParts; i++)
        {
            float angle = -i * 360f / numberOfRadialParts - angleBetweenParts / 2f;

            GameObject part = Instantiate(radialPartPrefab, radialPartCanvas);
            part.transform.localPosition = Vector3.zero;
            part.transform.localEulerAngles = new Vector3(0, 0, angle);
            part.transform.localScale = Vector3.one;

            var img = part.GetComponent<Image>();
            if (img != null)
            {
                Color baseColor = colorProvider.colors[i];
                baseColor.a = 1f;
                img.color = baseColor;
                img.fillAmount = finalFill;
            }

            spawnedParts.Add(part);
        }

        currentSelectedRadialPart = -1;
        previousSelected = -1;
    }
}