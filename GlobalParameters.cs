using System.Collections.Generic;
using UnityEngine;

public static class GlobalParameters {
    public static List<GameObject> dynamicObjects = new List<GameObject>();
    public static float moveFactor = 0.4f;
    public static float angle = 10f;
    public static float scaleFactor = 0.1f;
    public static GameObject selectedObject;
    public static List<GameObject> selectedObjects = new List<GameObject>();
    public static bool stickySelection = false;
    public static float timeLag = 0.001f;
    public static Transform targetTransform;
    
    public static void SelectSingleObject() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            selectedObject = hit.collider.gameObject;
            targetTransform = GetTargetTransform(selectedObject);
        }
    }

    public static void SelectMultipleObjects() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (!selectedObjects.Contains(hit.collider.gameObject)) {
                selectedObjects.Add(hit.collider.gameObject);
            }
        }
    }

    public static Vector3 CalculateCentroid(List<Vector3> vertices) {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 vert in vertices) sum += vert;
        return sum / vertices.Count;
    }

    public static Transform GetTargetTransform(GameObject obj) {
        if (obj.transform.parent != null) return obj.transform.parent;
        return obj.transform;
    }
}