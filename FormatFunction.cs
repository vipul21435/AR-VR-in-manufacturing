using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class FormatFunction : MonoBehaviour {
    private Material copiedMaterial;
    private Button formatButton;
    private bool readyToFormat = false;

    void Start() {
        formatButton = GameObject.Find("FormatButton").GetComponent<Button>();
        formatButton.GetComponentInChildren<Text>().text = "Format";
        formatButton.onClick.AddListener(PrepareFormat);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (!readyToFormat) {
                // Select the object to copy the material from
                SelectSingleObject();
                if (selectedObject != null)
                    CopyMaterialFromSelectedObject();
            } else {
                // Apply the copied material to the next selected object
                SelectSingleObject();
                if (selectedObject != null && copiedMaterial != null) {
                    ApplyMaterialToSelectedObject();
                    readyToFormat = false; // Reset the flag after formatting
                }
            }
        }
    }

    private void PrepareFormat() {
        if (copiedMaterial != null) {
            // Set the flag that indicates we're ready to apply the copied material
            readyToFormat = true;
        }
    }

    private void CopyMaterialFromSelectedObject() {
        Renderer renderer = selectedObject.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null) {
            copiedMaterial = renderer.sharedMaterial;
        }
    }

    private void ApplyMaterialToSelectedObject() {
        Renderer renderer = selectedObject.GetComponent<Renderer>();
        if (renderer != null) {
            renderer.material = copiedMaterial;
        }
    }
}