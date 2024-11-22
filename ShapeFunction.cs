using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class ShapeFunction : MonoBehaviour {
    private Dropdown shapeSelector;
    
    // Start is called before the first frame update
    void Start() {
        shapeSelector = GameObject.Find("ShapeSelector").GetComponent<Dropdown>();

        shapeSelector.onValueChanged.AddListener(delegate {
            Shape(shapeSelector.value);
        });

        if (shapeSelector.value != 0) Shape(shapeSelector.value);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            SelectSingleObject();
            shapeSelector.value = 0;
        }
    }

    private void Shape(int selectedShapeIndex) {
        if (selectedShapeIndex == 0) return; // If "Select" is chosen, do nothing.
        
        // Handling the "Original" option
        if (selectedShapeIndex == shapeSelector.options.Count - 1) {
            if (selectedObject != null) {
                OriginalMeshHolder meshHolder = selectedObject.GetComponent<OriginalMeshHolder>();
                if (meshHolder != null && meshHolder.originalMesh != null) {
                    selectedObject.GetComponent<MeshFilter>().mesh = meshHolder.originalMesh;
                }
            }
            return;
        }
        
        selectedShapeIndex--; // Adjust for "Select" option
        
        List<string> shapes = new List<string> { "Cube.fbx", "Sphere.fbx", "Cylinder.fbx", "Capsule.fbx" };

        if (selectedShapeIndex >= 0 && selectedShapeIndex < shapes.Count)
            selectedObject.GetComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>(shapes[selectedShapeIndex]);
    }
}