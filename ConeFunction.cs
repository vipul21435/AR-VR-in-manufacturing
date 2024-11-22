using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class ConeFunction : MonoBehaviour
{
    public Button coneAddButton;
    public InputField coneRadiusInput;
    public InputField coneHeightInput;
    public Material coneMaterial; // Assign a material for the cone through the Inspector

    private List<GameObject> cones = new List<GameObject>();

    void Start()
    {
        coneAddButton = GameObject.Find("ConeAddButton").GetComponent<Button>();
        coneAddButton.GetComponentInChildren<Text>().text = "Add";
        coneAddButton.onClick.AddListener(AddCone);

        coneRadiusInput = GameObject.Find("ConeRadiusInput").GetComponent<InputField>();
        coneHeightInput = GameObject.Find("ConeHeightInput").GetComponent<InputField>();
    }

    void AddCone()
    {
        // Validate and get radius and height from input fields
        float radius = ValidateAndParseInput(coneRadiusInput.text, 0.5f); // Default radius is 0.5 if input is invalid
        float height = ValidateAndParseInput(coneHeightInput.text, 1.0f); // Default height is 1.0 if input is invalid

        GameObject cone = new GameObject("Cone");
        MeshFilter meshFilter = cone.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = cone.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = cone.AddComponent<MeshCollider>();
        OriginalMeshHolder meshHolder = cone.AddComponent<OriginalMeshHolder>();

        meshRenderer.material = coneMaterial;

        Mesh mesh = GenerateConeMesh(20, radius, height);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;
        meshHolder.originalMesh = mesh;
        meshCollider.convex = true;

        cone.transform.position = new Vector3(0, 0, 0);
        cone.transform.localScale = new Vector3(1, 1, 1);
        cone.transform.rotation = Quaternion.identity;

        cones.Add(cone); // Add to the list to keep track of the cones
        dynamicObjects.Add(cone);
    }

    Mesh GenerateConeMesh(int segments, float radius, float height)
    {
        Vector3[] vertices = new Vector3[segments + 2];
        int[] triangles = new int[segments * 6];

        vertices[0] = Vector3.up * height;
        vertices[1] = Vector3.zero;
        float angleStep = 360.0f / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            vertices[i + 2] = new Vector3(Mathf.Sin(angle) * radius, 0f, Mathf.Cos(angle) * radius);
        }

        for (int i = 0; i < segments - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2;
            triangles[i * 3 + 2] = i + 3;
        }
        triangles[(segments - 1) * 3] = 0;
        triangles[(segments - 1) * 3 + 1] = segments + 1;
        triangles[(segments - 1) * 3 + 2] = 2;

        for (int i = 0; i < segments - 1; i++)
        {
            triangles[(segments + i) * 3] = 1;
            triangles[(segments + i) * 3 + 1] = i + 3;
            triangles[(segments + i) * 3 + 2] = i + 2;
        }
        triangles[(2 * segments - 1) * 3] = 1;
        triangles[(2 * segments - 1) * 3 + 1] = 2;
        triangles[(2 * segments - 1) * 3 + 2] = segments + 1;

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

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
}