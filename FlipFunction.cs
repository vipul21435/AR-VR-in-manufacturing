using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class FlipFunction : MonoBehaviour {
    private Button flipButton;

    // Start is called before the first frame update
    void Start() {
        flipButton = GameObject.Find("FlipButton").GetComponent<Button>();
        flipButton.GetComponentInChildren<Text>().text = "Flip";
        flipButton.onClick.AddListener(FlipSelectedObject);
    }

    // Update is called once per frame
    void Update() {
        // Check for mouse click to select an object
        if (Input.GetMouseButtonDown(0)) SelectSingleObject();
    }

    void FlipSelectedObject() {
        if (selectedObject != null) {
            // Get the current scale
            Vector3 scale = selectedObject.transform.localScale;
            
            // Flip the scale along the x-axis
            scale.x *= -1;
            
            // Apply the flipped scale back to the object
            selectedObject.transform.localScale = scale;
        }
    }
}