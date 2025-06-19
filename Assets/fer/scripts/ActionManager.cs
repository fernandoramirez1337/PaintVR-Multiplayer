using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public static ActionManager Instance { get; private set; }

    private enum ActionType { Drawing, Layer }

    private struct ActionData
    {
        public GameObject obj;
        public ActionType type;
        public Material material;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;

        public ActionData(GameObject obj, ActionType type, Material mat = null)
        {
            this.obj = obj;
            this.type = type;
            this.material = mat;
            this.position = obj.transform.position;
            this.rotation = obj.transform.rotation;
            this.scale = obj.transform.localScale;
        }
    }

    private Stack<ActionData> undoStack = new();
    private Stack<ActionData> redoStack = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void RecordDraw(GameObject line)
    {
        var tube = line.GetComponent<TubeRenderer>();
        var mat = tube != null ? tube.material : null;

        undoStack.Push(new ActionData(line, ActionType.Drawing, mat));
        redoStack.Clear();
    }

    public void RecordLayer(GameObject cube)
    {
        undoStack.Push(new ActionData(cube, ActionType.Layer));
        redoStack.Clear();
    }

    public void Undo()
    {
        if (undoStack.Count == 0) return;

        ActionData action = undoStack.Pop();

        switch (action.type)
        {
            case ActionType.Drawing:
                action.obj.SetActive(false);
                break;

            case ActionType.Layer:
                action.obj.SetActive(false);
                CubeManager.Instance.RemoveCube(action.obj);
                if (DrawingZoneManager.Instance.GetCurrentZone() == action.obj.GetComponent<BoxCollider>())
                    DrawingZoneManager.Instance.ClearActiveZone();
                break;
        }

        redoStack.Push(action);
    }

    public void Redo()
    {
        if (redoStack.Count == 0) return;

        ActionData action = redoStack.Pop();

        switch (action.type)
        {
            case ActionType.Drawing:
                action.obj.SetActive(true);

                // Reparentar al cubo activo por si fue destruido antes
                var currentCube = CubeManager.Instance.GetCurrentCube();
                if (currentCube != null)
                    action.obj.transform.SetParent(currentCube.transform);

                var tube = action.obj.GetComponent<TubeRenderer>();
                if (tube != null && action.material != null)
                    tube.material = new Material(action.material);
                break;

            case ActionType.Layer:
                GameObject newCube = Instantiate(action.obj, action.position, action.rotation);
                newCube.transform.localScale = action.scale;

                CubeManager.Instance.AddCube(newCube);
                CubeManager.Instance.SetActiveCube(newCube);

                if (newCube.TryGetComponent(out BoxCollider col))
                    DrawingZoneManager.Instance.SetActiveZone(col);
                break;
        }

        undoStack.Push(action);
    }

    public void ClearAll()
    {
        undoStack.Clear();
        redoStack.Clear();
    }
}