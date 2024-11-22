using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class BendFunction : MonoBehaviour {
    private Button bendButton;
    private Vector3 hitPoint; // Store the hit point when an object is selected

    void Start() {
        bendButton = GameObject.Find("BendButton").GetComponent<Button>();
        bendButton.GetComponentInChildren<Text>().text = "Bend";
        // Add a parameterless method as the listener
        bendButton.onClick.AddListener(OnBendButtonClick);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) SelectObject();
    }

    void SelectObject() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            selectedObject = hit.collider.gameObject;
            hitPoint = hit.point; // Update hitPoint with the current hit position
        }
    }

    // This method is called when the bend button is clicked
    void OnBendButtonClick() {
        if (selectedObject != null) {
            BendSelectedObject(hitPoint); // Now you can use the hitPoint
        }
    }

    // Adjusted to be called with a specific hit point
    void BendSelectedObject(Vector3 hitPoint) {
        Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;
        
        // Convert vertices array to List<Vector3>
        List<Vector3> vertices = mesh.vertices.ToList();

        // Calculate the centroid
        Vector3 centroid = CalculateCentroid(vertices);

        float maxBendDistance = 1.0f; // Max distance from hit point to apply deformation
        float bendStrength = 0.5f; // Adjust to control the strength of the bend

        for (int i = 0; i < vertices.Count; i++) { // Use Count for List
            // Convert vertex back to local space for processing
            Vector3 worldVertexPosition = selectedObject.transform.TransformPoint(vertices[i]);
            float distance = (worldVertexPosition - hitPoint).magnitude;
            if (distance < maxBendDistance) {
                float bendFactor = (1 - (distance / maxBendDistance)) * bendStrength;
                // Calculate direction towards the centroid
                Vector3 directionToCentroid = (centroid - worldVertexPosition).normalized;
                // Apply bend towards centroid
                worldVertexPosition += directionToCentroid * bendFactor;
                // Convert the vertex back to local space after modification
                vertices[i] = selectedObject.transform.InverseTransformPoint(worldVertexPosition);
            }
        }

        // Convert List<Vector3> back to Vector3[] to assign to mesh.vertices
        mesh.vertices = vertices.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}