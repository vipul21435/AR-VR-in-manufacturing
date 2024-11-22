using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class ColorFunction : MonoBehaviour {
    private Material shapeMaterial;
    private Slider redColorSlider, greenColorSlider, blueColorSlider;
    private Button applyButton;

    void Start() {
        redColorSlider = InitializeSlider("RedColorSlider");
        greenColorSlider = InitializeSlider("GreenColorSlider");
        blueColorSlider = InitializeSlider("BlueColorSlider");

        // Initialize the Apply button and add a listener
        applyButton = GameObject.Find("ApplyButton").GetComponent<Button>();
        applyButton.GetComponentInChildren<Text>().text = "Apply";
        applyButton.onClick.AddListener(ApplyColor);
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) SelectSingleObject();
        if (selectedObject != null) shapeMaterial = selectedObject.GetComponent<Renderer>().material;
    }

    Slider InitializeSlider(string sliderName) {
        Slider slider = GameObject.Find(sliderName).GetComponent<Slider>();
        slider.value = 1f;
        return slider;
    }

    private void ApplyColor() {
        // Change color only if an object is selected and its material is not null
        if (selectedObject != null && shapeMaterial != null) {
            Color newColor = new Color(redColorSlider.value, greenColorSlider.value, blueColorSlider.value);
            shapeMaterial.color = newColor;
        }
    }
}