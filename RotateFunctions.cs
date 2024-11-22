using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Needed for EventTrigger
using System.Collections;
using static GlobalParameters;

public class RotateFunctions : MonoBehaviour {
    private readonly Button[] rotateButtons = new Button[6];
    private Button resetButton;
    private Coroutine rotateCoroutine;

    void Start() {
        resetButton = GameObject.Find("ResetRotationButton").GetComponent<Button>();
        resetButton.onClick.AddListener(ResetRotation);
        resetButton.GetComponentInChildren<Text>().text = "Reset";

        InitializeButtons();
        SetButtonCallbacks();
        SetButtonTexts();
    }

    void InitializeButtons() {
        string[] buttonNames = { "RotateXNegButton", "RotateXPosButton", "RotateYNegButton", "RotateYPosButton", "RotateZNegButton", "RotateZPosButton" };

        for (int i = 0; i < rotateButtons.Length; i++)
            rotateButtons[i] = GameObject.Find(buttonNames[i]).GetComponent<Button>();
    }

    void SetButtonCallbacks() {
        Vector3[] directions = {Vector3.left * angle, Vector3.right * angle, Vector3.down * angle, Vector3.up * angle, Vector3.forward * angle, Vector3.back * angle};

        for (int i = 0; i < rotateButtons.Length; i++) AddEventTriggers(rotateButtons[i], directions[i]);
    }

    void AddEventTriggers(Button button, Vector3 direction) {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => { StartRotating(direction); });
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { StopRotating(); });
        trigger.triggers.Add(pointerUp);
    }

    void SetButtonTexts() {
        string[] buttonTexts = { "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

        for (int i = 0; i < rotateButtons.Length; i++)
            rotateButtons[i].GetComponentInChildren<Text>().text = buttonTexts[i];
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) SelectSingleObject();
    }

    private void StartRotating(Vector3 direction) {
        StopRotating(); // Ensure any existing rotation coroutine is stopped before starting a new one
        rotateCoroutine = StartCoroutine(RotateOverTime(direction));
    }

    private void StopRotating() {
        if (rotateCoroutine != null) {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }
    }

    private IEnumerator RotateOverTime(Vector3 direction) {
        while (true) {
            Rotate(direction);
            yield return new WaitForSeconds(timeLag);
        }
    }

    private void Rotate(Vector3 direction) {
        if (selectedObject != null)
            targetTransform.Rotate(direction * Time.deltaTime, Space.World);
    }

    private void ResetRotation() {
        if (selectedObject != null) {
            StartCoroutine(ResetRotationOverTime(GetTargetTransform(selectedObject)));
        }
    }

    private IEnumerator ResetRotationOverTime(Transform targetTransform) {
        Quaternion startRotation = targetTransform.rotation;
        Quaternion endRotation = Quaternion.identity;
        float elapsedTime = 0f;
        float duration = timeLag * 100; // Duration in seconds over which to perform the reset. Adjust as needed.

        while (elapsedTime < duration) {
            targetTransform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        targetTransform.rotation = endRotation; // Ensure the rotation is exactly reset to identity at the end.
    }
}