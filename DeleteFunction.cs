using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class DeleteFunction : MonoBehaviour {
    private Button deleteButton;
    private Button ClearButton;
    private Toggle stickyToggle;

    // Start is called before the first frame update
    void Start() {
        deleteButton = GameObject.Find("DeleteButton").GetComponent<Button>();
        deleteButton.GetComponentInChildren<Text>().text = "- Del";
        deleteButton.onClick.AddListener(DeleteSelectedObjects);

        ClearButton = GameObject.Find("ClearButton").GetComponent<Button>();
        ClearButton.GetComponentInChildren<Text>().text = "- Clear";
        ClearButton.onClick.AddListener(ClearScreen);

        stickyToggle = GameObject.Find("StickyToggle").GetComponent<Toggle>();
    }

    // Update is called once per frame
    void Update() {
        // Select multiple objects if in sticky selection mode
        if (stickySelection && Input.GetMouseButtonDown(0)) SelectMultipleObjects();
        // Select a signle object if not in sticky selection mode
        else if (Input.GetMouseButtonDown(0)) SelectSingleObject();
    }

    void DeleteSelectedObjects() {
        // delete multiple objects
        if (selectedObjects.Count > 0) {
            // Delete each selected object
            foreach (GameObject obj in selectedObjects) {
                if (obj.transform.parent != null) Destroy(obj.transform.parent.gameObject);
                else Destroy(obj);
            }
            stickySelection = false;
            stickyToggle.isOn = false;
        }
        // delete single object
        else if (selectedObject != null) {
            if (selectedObject.transform.parent != null) Destroy(selectedObject.transform.parent.gameObject);
            else Destroy(selectedObject);
        }
    }

    void ClearScreen() {
        // Iterate through the list of dynamically created objects and destroy them
        foreach (GameObject obj in dynamicObjects) {
            Destroy(obj);
        }
        // Clear the list after destroying the objects
        dynamicObjects.Clear();
    }
}