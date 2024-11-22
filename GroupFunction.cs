using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GlobalParameters;

public class GroupFunction : MonoBehaviour {
    private Button groupButton;
    private Button ungroupButton;
    private Toggle stickyToggle;

    void Start() {
        groupButton = GameObject.Find("GroupButton").GetComponent<Button>();
        groupButton.GetComponentInChildren<Text>().text = "Group";
        groupButton.onClick.AddListener(GroupShapes);

        ungroupButton = GameObject.Find("UngroupButton").GetComponent<Button>();
        ungroupButton.GetComponentInChildren<Text>().text = "Ungrp";
        ungroupButton.onClick.AddListener(UngroupShapes);

        stickyToggle = GameObject.Find("StickyToggle").GetComponent<Toggle>();
    }

    void Update() {
        if (stickySelection && Input.GetMouseButtonDown(0)) SelectMultipleObjects();
        else if (Input.GetMouseButtonDown(0)) SelectSingleObject();
    }

    Vector3 CalculateCenter(List<GameObject> objects) {
        if (objects == null || objects.Count == 0) return Vector3.zero;

        Vector3 sum = Vector3.zero;
        foreach (GameObject obj in objects) {
            sum += obj.transform.position;
        }
        return sum / objects.Count;
    }

    void GroupShapes() {
        if (selectedObjects.Count < 2) return; // Need at least two objects to group together

        // Creating a new GameObject to serve as the group parent
        GameObject groupContainer = new GameObject("GroupObject");

        // Optional: Calculate the center of all selected objects
        Vector3 center = CalculateCenter(selectedObjects);

        // Set the position of the group container to the calculated center
        groupContainer.transform.position = center;

        // Assign each selected object to the new group container
        foreach (GameObject obj in selectedObjects) {
            obj.transform.SetParent(groupContainer.transform, true);
        }

        dynamicObjects.Add(groupContainer);

        // Clear the list of selected objects and reset selection mode
        selectedObjects.Clear();
        selectedObject = null;
        stickySelection = false;
        stickyToggle.isOn = false;
    }

    void UngroupShapes() {
        // Check if a single object is selected
        if (selectedObject == null) return;

        // Check if the selected object is part of a group (has a parent)
        Transform parentGroup = selectedObject.transform.parent;
        if (parentGroup != null) {
            // Temporary list to store all siblings for safe iteration
            List<GameObject> siblings = new List<GameObject>();
            foreach (Transform sibling in parentGroup) {
                siblings.Add(sibling.gameObject);
            }

            // Detach each sibling from the parent group
            foreach (GameObject sibling in siblings) {
                sibling.transform.SetParent(null);
            }

            // Destroy the empty parent group
            Destroy(parentGroup.gameObject);
        }

        // Reset the selected object
        selectedObject = null;
    }
}