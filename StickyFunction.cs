using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class StickyFunction : MonoBehaviour {
    private Toggle stickyToggle;

    void Start() {
        stickyToggle = GameObject.Find("StickyToggle").GetComponent<Toggle>();
        stickyToggle.GetComponentInChildren<Text>().text = "Sticky";
        stickyToggle.isOn = false; // Ensure that drawing mode is initially off
        stickyToggle.onValueChanged.AddListener(delegate { ToggleStickySelection(); });
    }

    void ToggleStickySelection() {
        stickySelection = stickyToggle.isOn;
    }
}