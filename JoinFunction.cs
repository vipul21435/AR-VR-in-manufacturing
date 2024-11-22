using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class JoinFunction : MonoBehaviour {
    private Button joinButton;
    private Toggle stickyToggle;

    void Start() {
        joinButton = GameObject.Find("JoinButton").GetComponent<Button>();
        joinButton.GetComponentInChildren<Text>().text = "Join";
        joinButton.onClick.AddListener(JoinShapes);

        stickyToggle = GameObject.Find("StickyToggle").GetComponent<Toggle>();
    }

    void Update() {
        if (stickySelection && Input.GetMouseButtonDown(0)) SelectMultipleObjects();
    }

    void JoinShapes() {
        if (selectedObjects.Count < 2) return; // Need at least two objects to join

        CombineInstance[] combine = new CombineInstance[selectedObjects.Count];
        List<Vector3> allVertices = new List<Vector3>(); // To calculate the centroid
        for (int i = 0; i < selectedObjects.Count; i++) {
            if (selectedObjects[i].TryGetComponent<MeshFilter>(out MeshFilter meshFilter)) {
                combine[i].mesh = meshFilter.sharedMesh;
                combine[i].transform = selectedObjects[i].transform.localToWorldMatrix;

                // Add vertices to allVertices for centroid calculation
                foreach (Vector3 vertex in meshFilter.sharedMesh.vertices) {
                    allVertices.Add(selectedObjects[i].transform.TransformPoint(vertex));
                }
            }
        }

        // Create a new GameObject to hold the combined mesh
        GameObject combinedObject = new GameObject("JoinObject");
        combinedObject.transform.position = Vector3.zero; // Initialize at origin for correct centroid calculation
        combinedObject.AddComponent<MeshFilter>().mesh = new Mesh();
        combinedObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);

        // Calculate centroid and adjust vertices
        Vector3 centroid = CalculateCentroid(allVertices);
        Vector3[] adjustedVertices = new Vector3[combinedObject.GetComponent<MeshFilter>().mesh.vertexCount];
        for (int i = 0; i < adjustedVertices.Length; i++) {
            adjustedVertices[i] = combinedObject.GetComponent<MeshFilter>().mesh.vertices[i] - centroid;
        }

        combinedObject.GetComponent<MeshFilter>().mesh.vertices = adjustedVertices;
        combinedObject.GetComponent<MeshFilter>().mesh.RecalculateBounds(); // Important to adjust collider

        combinedObject.AddComponent<MeshRenderer>().material = selectedObjects[0].GetComponent<MeshRenderer>().material; // Use the material of the first object
        MeshCollider meshCollider = combinedObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = combinedObject.GetComponent<MeshFilter>().mesh; // Update collider with the adjusted mesh

        // Adjust the object's position to its original centroid
        combinedObject.transform.position = centroid;

        // store the original combined mesh in OriginalMeshHolder
        OriginalMeshHolder meshHolder = combinedObject.AddComponent<OriginalMeshHolder>();
        meshHolder.originalMesh = Instantiate(combinedObject.GetComponent<MeshFilter>().mesh);

        dynamicObjects.Add(combinedObject);

        // Destroy the original objects
        foreach (GameObject obj in selectedObjects) {
            Destroy(obj);
        }

        // Clear the list of selected objects and reset selection mode
        selectedObjects.Clear();
        stickySelection = false;
        stickyToggle.isOn = false;
    }
}