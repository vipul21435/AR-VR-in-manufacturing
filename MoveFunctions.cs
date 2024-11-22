using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Needed for EventTrigger
using System.Collections;
using static GlobalParameters;

public class MoveFunctions : MonoBehaviour {
    private readonly Button[] moveButtons = new Button[6];
    private Button resetButton;
    private Coroutine moveCoroutine;

    void Start() {
        resetButton = GameObject.Find("ResetPositionButton").GetComponent<Button>();
        resetButton.onClick.AddListener(ResetPosition);
        resetButton.GetComponentInChildren<Text>().text = "Reset";

        InitializeButtons();
        SetButtonCallbacks();
        SetButtonTexts();
    }

    void InitializeButtons() {
        string[] buttonNames = { "MoveLeftButton", "MoveRightButton", "MoveDownButton", "MoveUpButton", "MoveFrontButton", "MoveBackButton" };

        for (int i = 0; i < moveButtons.Length; i++)
            moveButtons[i] = GameObject.Find(buttonNames[i]).GetComponent<Button>();
    }

    void SetButtonCallbacks() {
        Vector3[] directions = {Vector3.left, Vector3.right, Vector3.down, Vector3.up, Vector3.forward, Vector3.back};

        for (int i = 0; i < moveButtons.Length; i++) AddEventTriggers(moveButtons[i], directions[i]);
    }

    void AddEventTriggers(Button button, Vector3 direction) {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>() ?? button.gameObject.AddComponent<EventTrigger>();

        var pointerDown = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        pointerDown.callback.AddListener((data) => { StartMoving(direction); });
        trigger.triggers.Add(pointerDown);

        var pointerUp = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        pointerUp.callback.AddListener((data) => { StopMoving(); });
        trigger.triggers.Add(pointerUp);
    }

    void SetButtonTexts() {
        string[] buttonTexts = { "X-", "X+", "Y-", "Y+", "Z-", "Z+" };

        for (int i = 0; i < moveButtons.Length; i++)
            moveButtons[i].GetComponentInChildren<Text>().text = buttonTexts[i];
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) SelectSingleObject();
    }

    private void StartMoving(Vector3 direction) {
        StopMoving(); // Ensure any existing movement coroutine is stopped before starting a new one
        moveCoroutine = StartCoroutine(MoveOverTime(direction));
    }

    private void StopMoving() {
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    private IEnumerator MoveOverTime(Vector3 direction) {
        while (true) {
            Move(direction);
            yield return new WaitForSeconds(timeLag);
        }
    }

    private void Move(Vector3 direction) {
        if (selectedObject != null)
            targetTransform.position += moveFactor * direction * Time.deltaTime; // Ensure smooth movement
    }

    private void ResetPosition() {
        if (selectedObject != null)
            StartCoroutine(ResetPositionOverTime(GetTargetTransform(selectedObject)));
    }

    private IEnumerator ResetPositionOverTime(Transform targetTransform) {
        Vector3 startPosition = targetTransform.position;
        Vector3 endPosition = Vector3.zero; // Target position (0, 0, 0)
        float elapsedTime = 0f;
        float duration = timeLag * 100; // Duration in seconds over which to perform the reset. Adjust as needed.

        while (elapsedTime < duration) {
            targetTransform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }

        targetTransform.position = endPosition; // Ensure the position is exactly reset at the end.
    }
}