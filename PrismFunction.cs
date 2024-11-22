using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class PrismFunction : MonoBehaviour
{
    public Button prismAddButton;
    public InputField prismVertexInput;
    public InputField prismSideInput;
    public InputField prismHeightInput;
    public Material prismMaterial; // Assign a material for the prism through the Inspector

    private List<GameObject> prisms = new List<GameObject>();

    void Start()
    {
        prismAddButton = GameObject.Find("PrismAddButton").GetComponent<Button>();
        prismAddButton.GetComponentInChildren<Text>().text = "Add Prism";
        prismAddButton.onClick.AddListener(AddPrism);

        prismVertexInput = GameObject.Find("PrismVertexInput").GetComponent<InputField>();
        prismSideInput = GameObject.Find("PrismSideInput").GetComponent<InputField>();
        prismHeightInput = GameObject.Find("PrismHeightInput").GetComponent<InputField>();
    }

    void AddPrism()
    {
        int vertex = Mathf.Max(3, ValidateAndParseInputInt(prismVertexInput.text, 6)); // Default to 6 vertex if input is invalid
        float radius = ValidateAndParseInput(prismSideInput.text, 0.5f); // Default radius is 0.5 if input is invalid
        float height = ValidateAndParseInput(prismHeightInput.text, 1.0f); // Default height is 1.0 if input is invalid

        GameObject prism = new GameObject("Prism");
        MeshFilter meshFilter = prism.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = prism.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = prism.AddComponent<MeshCollider>();
        OriginalMeshHolder meshHolder = prism.AddComponent<OriginalMeshHolder>();

        meshRenderer.material = prismMaterial;

        Mesh mesh = GeneratePrismMesh(vertex, radius, height);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshHolder.originalMesh = mesh;
        meshCollider.convex = true;

        prism.transform.position = Vector3.zero;
        prism.transform.localScale = Vector3.one;
        prism.transform.rotation = Quaternion.identity;

        prisms.Add(prism); // Add to the list to keep track of the prisms
        dynamicObjects.Add(prism);
    }

    Mesh GeneratePrismMesh(int vertex, float radius, float height)
    {
        Vector3[] vertices = new Vector3[vertex * 2];
        int[] triangles = new int[vertex * 12]; // 3 vertices per triangle, 2 triangles per face, 2 faces (top and bottom) + 2 triangles per side face

        float angleStep = 360.0f / vertex;
        for (int i = 0; i < vertex; i++)
        {
            float angleRad = Mathf.Deg2Rad * angleStep * i;
            vertices[i] = new Vector3(radius * Mathf.Cos(angleRad), 0, radius * Mathf.Sin(angleRad)); // Bottom vertices
            vertices[i + vertex] = new Vector3(radius * Mathf.Cos(angleRad), height, radius * Mathf.Sin(angleRad)); // Top vertices
        }

        // Create bottom and top faces
        for (int i = 0, j = 0; i < vertex; i++)
        {
            int next = (i + 1) % vertex;
            // Bottom face
            triangles[j++] = i;
            triangles[j++] = next;
            triangles[j++] = 0;
            // Top face
            triangles[j++] = i + vertex;
            triangles[j++] = vertex;
            triangles[j++] = next + vertex;
        }

        // Create side faces
        for (int i = 0, j = vertex * 6; i < vertex; i++)
        {
            int next = (i + 1) % vertex;
            // First triangle of side face
            triangles[j++] = i;
            triangles[j++] = i + vertex;
            triangles[j++] = next;
            // Second triangle of side face
            triangles[j++] = next;
            triangles[j++] = i + vertex;
            triangles[j++] = next + vertex;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    float ValidateAndParseInput(string inputText, float defaultValue)
    {
        if (float.TryParse(inputText, out float result))
        {
            return result;
        }
        return defaultValue;
    }

    int ValidateAndParseInputInt(string inputText, int defaultValue)
    {
        if (int.TryParse(inputText, out int result))
        {
            return result;
        }
        return defaultValue;
    }
}