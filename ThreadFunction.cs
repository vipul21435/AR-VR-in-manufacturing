using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class ThreadFunction : MonoBehaviour {
    public Material cylinderMaterial;
    public float cylinderRadius = 0.1f;

    private List<GameObject> cylinders = new List<GameObject>();
    private List<Vector3> linePoints = new List<Vector3>();

    private bool threadMode = false;
    private Toggle threadToggle, addToggle, drawToggle;

    void Start() {
        threadToggle = GameObject.Find("ThreadToggle").GetComponent<Toggle>();
        threadToggle.GetComponentInChildren<Text>().text = "Thread";
        threadToggle.isOn = false; // Ensure that threading mode is initially off
        threadToggle.onValueChanged.AddListener(delegate { ThreadModeChange(); }); // Add listener for value change event
        
        addToggle = GameObject.Find("AddToggle").GetComponent<Toggle>();
        drawToggle = GameObject.Find("DrawToggle").GetComponent<Toggle>();
    }

    void ThreadModeChange() {
        threadMode = threadToggle.isOn; // Update threadMode based on toggle state
        addToggle.isOn = false;
        drawToggle.isOn = false;
    }

    void Update() {
        // Check if right mouse button is clicked
        if (threadMode && Input.GetMouseButtonDown(1)) AddPointToLine();

        // Check if left mouse button is clicked
        if (threadMode && Input.GetMouseButtonDown(0)) {
            MergeCylindersIntoThread();
            ClearLine();
        }
    }

    void AddPointToLine() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10.0f; // Set the distance from the camera

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Add the new point to the line
        linePoints.Add(worldPos);

        // If there are at least two points, create cylinders between them
        if (linePoints.Count >= 2) {
            Vector3 lastPoint = linePoints[linePoints.Count - 2];
            Vector3 currentPoint = linePoints[linePoints.Count - 1];

            CreateCylinderBetweenPoints(lastPoint, currentPoint);
        }
    }

    void CreateCylinderBetweenPoints(Vector3 start, Vector3 end) {
        // Calculate the distance and direction between the two points
        float distance = Vector3.Distance(start, end);
        Vector3 direction = (end - start).normalized;

        // Create a new cylinder
        GameObject cylinderObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);

        // Calculate position, rotation, and scale of the cylinder
        cylinderObject.transform.position = start + direction * distance * 0.5f;
        cylinderObject.transform.up = direction;
        cylinderObject.transform.localScale = new Vector3(cylinderRadius * 2, distance * 0.5f, cylinderRadius * 2);

        // Assign material to the cylinder
        cylinderObject.GetComponent<Renderer>().material = cylinderMaterial;

        // Add the cylinder to the list
        cylinders.Add(cylinderObject);
    }

    void MergeCylindersIntoThread() {
        if (cylinders.Count == 0) return;

        // Create a new GameObject to hold the merged mesh
        GameObject threadObject = new GameObject("Thread");

        // Combine all cylinder meshes into a single mesh
        CombineInstance[] combine = new CombineInstance[cylinders.Count];
        for (int i = 0; i < cylinders.Count; i++) {
            MeshFilter filter = cylinders[i].GetComponent<MeshFilter>();
            combine[i].mesh = filter.sharedMesh;
            combine[i].transform = filter.transform.localToWorldMatrix;
            // Destroy the individual cylinder GameObjects
            Destroy(cylinders[i]);
        }

        // Create a new MeshFilter and MeshRenderer for the thread object
        MeshFilter meshFilter = threadObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine, true);

        // Add OriginalMeshHolder component and store the original mesh
        OriginalMeshHolder meshHolder = threadObject.AddComponent<OriginalMeshHolder>();
        meshHolder.originalMesh = meshFilter.mesh;

        MeshRenderer meshRenderer = threadObject.AddComponent<MeshRenderer>();
        meshRenderer.material = cylinderMaterial;

        // Add a mesh collider to enable selection
        MeshCollider meshCollider = threadObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = meshFilter.mesh;

        // Detach the thread object from its parent
        threadObject.transform.parent = null;

        // Clear the cylinders list
        cylinders.Clear();
        dynamicObjects.Add(threadObject);
    }

    void ClearLine() {
        // Clear the line points
        linePoints.Clear();
    }
}