using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Required for event triggers
using System.Collections;
using static GlobalParameters;

public class ScaleFunctions : MonoBehaviour {
    private readonly Button[] scaleButtons = new Button[6];
    private Button resetButton;
    private Vector3 currentScale;
    private Coroutine scaleCoroutine;

    void Start() {
        resetButton = GameObject.Find("ResetSizeButton").GetComponent<Button>();
        resetButton.onClick.AddListener(ResetSize);
        resetButton.GetComponentInChildren<Text>().text = "Reset";

        InitializeButtons();
        SetButtonCallbacks();
        SetButtonTexts();
    }

    void InitializeButtons() {
        string[] buttonNames = { "ScaleXNegButton", "ScaleXPosButton", "ScaleYNegButton", "ScaleYPosButton", "ScaleZNegButton", "ScaleZPosButton" };

        for (int i = 0; i < scaleButtons.Length; i++)
            scaleButtons[i] = GameObject.Find(buttonNames[i]).GetComponent<Button>();
    }

    void SetButtonCallbacks() {
        Vector3[] directions = { Vector3.left, Vector3.right, Vector3.down, Vector3.up, Vector3.back, Vector3.forward };

        for (int i = 0; i < scaleButtons.Length; i++) {
            int index = i; // Local copy for closure
            EventTrigger trigger = scaleButtons[i].gameObject.AddComponent<EventTrigger>();

            var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            pointerDown.callback.AddListener((data) => { StartScaling(directions[index]); });
            trigger.triggers.Add(pointerDown);

            var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            pointerUp.callback.AddListener((data) => { StopScaling(); });
            trigger.triggers.Add(pointerUp);
        }
    }

    void SetButtonTexts() {
        string[] buttonTexts = { "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

        for (int i = 0; i < scaleButtons.Length; i++)
            scaleButtons[i].GetComponentInChildren<Text>().text = buttonTexts[i];
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) SelectSingleObject();
    }

    private void StartScaling(Vector3 direction) {
        StopScaling(); // Ensure any existing scaling coroutine is stopped before starting a new one
        scaleCoroutine = StartCoroutine(ScaleOverTime(direction));
    }

    private void StopScaling() {
        if (scaleCoroutine != null) {
            StopCoroutine(scaleCoroutine);
            scaleCoroutine = null;
        }
    }

    private IEnumerator ScaleOverTime(Vector3 direction) {
        while (true) {
            Scale(direction);
            yield return new WaitForSeconds(timeLag * 100);
        }
    }

    private void Scale(Vector3 axis) {
        if (selectedObject != null) {
            currentScale = targetTransform.localScale;
            if (axis == Vector3.left && currentScale.x <= 0.1f) {
                currentScale.x = 0.01f;
            } else if (axis == Vector3.down && currentScale.y <= 0.1f) {
                currentScale.y = 0.01f;
            } else if (axis == Vector3.back && currentScale.z <= 0.1f) {
                currentScale.z = 0.01f;
            } else {
                currentScale += scaleFactor * axis;
            }
            targetTransform.localScale = currentScale;
        }
    }

    private void ResetSize() {
        if (selectedObject != null) {
            StartCoroutine(ResetSizeOverTime(GetTargetTransform(selectedObject)));
        }
    }

    private IEnumerator ResetSizeOverTime(Transform targetTransform) {
        Vector3 startScale = targetTransform.localScale;
        Vector3 endScale = new Vector3(1f, 1f, 1f); // Target scale (1, 1, 1)
        float elapsedTime = 0f;
        float duration = timeLag * 100; // Duration in seconds over which to perform the reset. Adjust as needed.

        while (elapsedTime < duration) {
            targetTransform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        targetTransform.localScale = endScale; // Ensure the scale is exactly reset at the end.
    }
}