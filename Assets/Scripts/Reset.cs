using UnityEngine;

public class Reset : MonoBehaviour {

    private Vector3 originalPosition;

    private PlayerController playerController;

    void Awake() {

        originalPosition = gameObject.transform.position;

        playerController = gameObject.GetComponent<PlayerController>();

    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.R)) {

            ResetGameObject();

        }

    }

    void ResetGameObject() {

        playerController.position = originalPosition;

    }

}
