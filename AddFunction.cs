using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class AddFunction : MonoBehaviour {
    private Toggle addToggle, drawToggle, threadToggle; // Reference to the add toggle
    private bool addMode = false; // Whether add mode is enabled

    void Start() {
        addToggle = GameObject.Find("AddToggle").GetComponent<Toggle>();
        addToggle.GetComponentInChildren<Text>().text = "Add";
        addToggle.isOn = false; // Ensure that add mode is initially off
        addToggle.onValueChanged.AddListener(delegate { AddModeChange(); });

        drawToggle = GameObject.Find("DrawToggle").GetComponent<Toggle>();
        threadToggle = GameObject.Find("ThreadToggle").GetComponent<Toggle>();
    }

    void AddModeChange() {
        addMode = addToggle.isOn; // Update drawMode based on toggle state
        drawToggle.isOn = false;
        threadToggle.isOn = false;
    }

    void Update() {
        if (addMode && Input.GetMouseButtonDown(1)) AddCubeAtMousePosition();
    }

    void AddCubeAtMousePosition() {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane + 10f; // Adjust this value as needed for your scene setup

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.y = 0;
        worldPos.z = 0;

        // Create and configure the cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = worldPos;

        cube.name = "SimpleObject"; // Set a name for the cube
        dynamicObjects.Add(cube); // Add to your dynamic objects list, assuming you have such a list
    }
}