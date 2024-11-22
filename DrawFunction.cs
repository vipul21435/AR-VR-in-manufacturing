using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class DrawFunction : MonoBehaviour {
    public Material lineMaterial;
    public Material solidMaterial; // New material for the closed figure
    public float lineDrawWidth = 0.1f;
    public float depth = 0.2f; // Depth of the extruded shape

    private readonly List<GameObject> lines = new();
    private LineRenderer currentLineRenderer;
    private readonly List<Vector3> currentLinePoints = new();
    private bool drawMode = false;
    private Toggle drawToggle, addToggle, threadToggle;

    void Start() {
        drawToggle = GameObject.Find("DrawToggle").GetComponent<Toggle>();
        drawToggle.GetComponentInChildren<Text>().text = "Draw";
        drawToggle.isOn = false; // Ensure that drawing mode is initially off
        drawToggle.onValueChanged.AddListener(delegate { DrawModeChange(); }); // Add listener for value change event

        addToggle = GameObject.Find("AddToggle").GetComponent<Toggle>();
        threadToggle = GameObject.Find("ThreadToggle").GetComponent<Toggle>();
    }

    void DrawModeChange() {
        drawMode = drawToggle.isOn; // Update drawMode based on toggle state
        addToggle.isOn = false;
        threadToggle.isOn = false;
    }

    void Update() {
        // Check if right button is clicked
        if (drawMode && Input.GetMouseButton(1)) AddPointToLine();

        // Check if left button is clicked
        if (drawMode && Input.GetMouseButton(0)) SaveCurrentLine();
    }

    void AddPointToLine() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10.0f; // Set the distance from the camera

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // If there is no current line, create a new one
        if (currentLineRenderer == null) CreateNewLine();

        // Resize the array to accommodate the new point
        int numberOfPoints = currentLinePoints.Count;
        currentLineRenderer.positionCount = numberOfPoints + 1;

        // Set the new point in the line renderer
        currentLineRenderer.SetPosition(numberOfPoints, worldPos);

        // Save the point to the current line
        currentLinePoints.Add(worldPos);
    }

    void CreateNewLine() {
        // Create a new GameObject for the line
        GameObject lineObject = new("Line");
        lineObject.transform.SetParent(transform); // Set the current object as the parent
        currentLineRenderer = lineObject.AddComponent<LineRenderer>();
        currentLineRenderer.material = lineMaterial;
        currentLineRenderer.startWidth = lineDrawWidth;
        currentLineRenderer.endWidth = lineDrawWidth;
        currentLineRenderer.useWorldSpace = true;

        // Add the new line object to the list
        lines.Add(lineObject);
    }

    void SaveCurrentLine() {
        if (currentLineRenderer != null && currentLinePoints.Count > 2) {
            List<Vector3> backFacePoints = new();
            foreach (Vector3 point in currentLinePoints)
                backFacePoints.Add(new Vector3(point.x, point.y, point.z - depth));

            List<Vector3> vertices = new();
            List<int> triangles = new();

            AddPolygonFace(vertices, triangles, currentLinePoints, true);

            int offset = vertices.Count;
            AddPolygonFace(vertices, triangles, backFacePoints, false, offset);

            AddSideFaces(vertices, triangles, currentLinePoints, backFacePoints);

            // Calculate the centroid
            Vector3 centroid = CalculateCentroid(vertices);

            // Translate vertices to make the centroid the origin
            for (int i = 0; i < vertices.Count; i++) vertices[i] -= centroid;

            Mesh mesh = new Mesh {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };
            mesh.RecalculateNormals();

            GameObject solidObject = new GameObject("DrawObject");
            dynamicObjects.Add(solidObject);

            solidObject.transform.position = centroid;

            MeshFilter meshFilter = solidObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            // Add OriginalMeshHolder component and store the original mesh
            OriginalMeshHolder meshHolder = solidObject.AddComponent<OriginalMeshHolder>();
            meshHolder.originalMesh = mesh;

            MeshRenderer meshRenderer = solidObject.AddComponent<MeshRenderer>();
            meshRenderer.material = solidMaterial;

            MeshCollider meshCollider = solidObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            Destroy(currentLineRenderer.gameObject);
            currentLineRenderer = null;
            currentLinePoints.Clear();
        }
    }

    // Method to add a polygon face (either front or back)
    void AddPolygonFace(List<Vector3> vertices, List<int> triangles, List<Vector3> facePoints, bool frontFace, int vertexOffset = 0) {
        vertices.AddRange(facePoints);
        for (int i = 2; i < facePoints.Count; i++) {
            if (frontFace) {
                triangles.Add(vertexOffset);
                triangles.Add(vertexOffset + i - 1);
                triangles.Add(vertexOffset + i);
            }
            else {
                triangles.Add(vertexOffset + i);
                triangles.Add(vertexOffset + i - 1);
                triangles.Add(vertexOffset);
            }
        }
    }

    // Method to add side faces between front and back
    void AddSideFaces(List<Vector3> vertices, List<int> triangles, List<Vector3> frontPoints, List<Vector3> backPoints) {
        int count = frontPoints.Count;
        for (int i = 0; i < count; i++) {
            int nextIndex = (i + 1) % count;
            int currentVertCount = vertices.Count;

            // Front to back vertices for the current side
            vertices.Add(frontPoints[i]);
            vertices.Add(backPoints[i]);
            vertices.Add(frontPoints[nextIndex]);
            vertices.Add(backPoints[nextIndex]);

            // Triangle 1
            triangles.Add(currentVertCount);
            triangles.Add(currentVertCount + 1);
            triangles.Add(currentVertCount + 2);

            // Triangle 2
            triangles.Add(currentVertCount + 2);
            triangles.Add(currentVertCount + 1);
            triangles.Add(currentVertCount + 3);
        }
    }
}