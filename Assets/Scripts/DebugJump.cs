using System.Collections.Generic;
using UnityEngine;

public class DebugJump : MonoBehaviour {

    private readonly float debugLineWidth = 2.0f;

    [SerializeField]
    private List<Vector3> fallingPositions = new List<Vector3>();

    private Vector3 previousPosition = Vector3.zero;

    private bool falling = false;

    void Update() {

        foreach (Vector3 position in fallingPositions) {

            Debug.DrawLine(position + Vector3.left * debugLineWidth / 2, position + Vector3.right * debugLineWidth / 2, Color.red);

        }

        Vector3 currentPosition = gameObject.transform.position;

        if (previousPosition.y > currentPosition.y && !falling) {

            falling = true;

            fallingPositions.Add(currentPosition);

        } else if (previousPosition.y < currentPosition.y) {

            falling = false;

        }

        previousPosition = currentPosition;

    }

    private void OnDrawGizmos() {

        Bounds colliderBounds = gameObject.GetComponent<BoxCollider2D>().bounds;

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector2(colliderBounds.min.x - colliderBounds.extents.x, colliderBounds.center.y), colliderBounds.size * 0.95f);
        Gizmos.DrawCube(new Vector2(colliderBounds.max.x + colliderBounds.extents.x, colliderBounds.center.y), colliderBounds.size * 0.95f);
        Gizmos.DrawCube(new Vector2(colliderBounds.center.x, colliderBounds.max.y + colliderBounds.extents.y), colliderBounds.size * 0.95f);
        Gizmos.DrawCube(new Vector2(colliderBounds.center.x, colliderBounds.min.y - colliderBounds.extents.y), colliderBounds.size * 0.95f);

    }

}
