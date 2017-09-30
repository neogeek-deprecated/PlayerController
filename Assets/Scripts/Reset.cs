using UnityEngine;

public class Reset : MonoBehaviour {

    private Vector3 originalPosition;

    void Awake() {

        originalPosition = gameObject.transform.position;

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.DownArrow)) {

            ResetGameObject();

        }

    }

    void ResetGameObject() {

        gameObject.transform.position = originalPosition;

    }

}
