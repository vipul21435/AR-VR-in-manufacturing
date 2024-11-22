using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class DuplicateFunction : MonoBehaviour {
    private Button duplicateButton;

    // Start is called before the first frame update
    void Start() {
        duplicateButton = GameObject.Find("DuplicateButton").GetComponent<Button>();
        duplicateButton.GetComponentInChildren<Text>().text = "+ Dup";
        duplicateButton.onClick.AddListener(DuplicateSelectedObject);
    }

    // Update is called once per frame
    void Update() {
        // Check for mouse click to select an object
        if (Input.GetMouseButtonDown(0)) SelectSingleObject();
    }

    void DuplicateSelectedObject() {
        if (selectedObject != null) {
            // Duplicate the selected object by instantiating a new one at the same position with the same rotation and scale
            GameObject clonedObject = Instantiate(selectedObject, selectedObject.transform.position, selectedObject.transform.rotation);

            // Check if the original object has a Renderer component
            Renderer originalRenderer = selectedObject.GetComponent<Renderer>();
            if (originalRenderer != null) {
                // Get the Renderer component of the cloned object
                Renderer clonedRenderer = clonedObject.GetComponent<Renderer>();
                if (clonedRenderer != null) {
                    // Create a new material instance for the cloned object based on the original's material
                    clonedRenderer.material = new Material(originalRenderer.material);
                }
            }

            dynamicObjects.Add(clonedObject);
        }
    }
}