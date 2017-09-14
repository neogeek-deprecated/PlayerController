﻿using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform platformTrigger;
    public LayerMask platformLayerMask;

    public Transform wallTrigger;
    public LayerMask wallLayerMask;

    public enum STATE {
        PLAYER_IDLE,
        PLAYER_RUNNING,
        PLAYER_FALLING,
        PLAYER_JUMPING,
        PLAYER_WALL_SLIDE,
        PLAYER_WALL_JUMP,
        PLAYER_WALL_DISMOUNT
    }

    [SerializeField]
    private STATE _state = STATE.PLAYER_IDLE;

    public STATE state {
        get {
            return _state;
        }
        set {

            Debug.Log(string.Format("Switched from {0} to {1}.", _state, value));

            _state = value;

        }
    }

    private readonly float gravity = 20.0f;
    private readonly float wallSlideSpeed = -2.0f;
    private readonly float horizontalSpeed = 6.0f;
    private readonly float jumpSpeed = 14.0f;
    private readonly int maxAvalibleJumps = 2;
    private readonly float raycastDistance = 1.0f;
    private readonly WaitForSeconds horizontalMovementDelay = new WaitForSeconds(0.5f);

    private Vector2 velocity = Vector2.zero;
    private int horizontalDirection = 1;

    private float inputHorizontal = 0;
    private bool inputHorizontalEnabled = true;
    private int inputJumpsAvalible = 0;

    private Vector2 hitLeft;
    private Vector2 hitRight;
    private Vector2 hitBottom;

    private bool _inputJump = false;
    private bool inputJump {

        get {

            if (_inputJump) {

                _inputJump = false;

                return true;

            }

            return false;

        }

        set {

            if (value) {

                _inputJump = true;

            }

        }

    }

    void Update() {

        if (inputHorizontalEnabled) {

            inputHorizontal = Input.GetAxisRaw("Horizontal");

        }

        inputJump = Input.GetKeyDown(KeyCode.Space);

    }

    void FixedUpdate() {

        UpdateHitVectors();

        switch (state) {

            case STATE.PLAYER_IDLE:
                Idle();
                break;

            case STATE.PLAYER_RUNNING:
                Running();
                break;

            case STATE.PLAYER_FALLING:
                Falling();
                break;

            case STATE.PLAYER_JUMPING:
                Jumping();
                break;

            case STATE.PLAYER_WALL_SLIDE:
                WallSlide();
                break;

            case STATE.PLAYER_WALL_JUMP:
                WallJump();
                break;

            case STATE.PLAYER_WALL_DISMOUNT:
                WallDismount();
                break;

            default:
                break;

        }

        ResetInputVariables();

    }

    void Idle() {

        inputJumpsAvalible = maxAvalibleJumps;

        velocity = Vector2.zero;

        Vector2 platformPoint = GetNextPlatformPoint();
        Vector2 wallPoint = GetNextWallPoint();

        if (Mathf.Abs(inputHorizontal) > 0 && inputHorizontal != horizontalDirection) {

            Flip();

        }

        if ((inputHorizontal == 1 && wallPoint.x > gameObject.transform.position.x) ||
            (inputHorizontal == -1 && wallPoint.x < gameObject.transform.position.x)) {

            state = STATE.PLAYER_RUNNING;

            return;

        }

        if (platformPoint.y < platformTrigger.position.y) {

            state = STATE.PLAYER_FALLING;

            return;

        }

        if (inputJump) {

            inputJumpsAvalible -= 1;

            state = STATE.PLAYER_JUMPING;

            return;

        }

    }

    void Running() {

        inputJumpsAvalible = maxAvalibleJumps;

        velocity.x = inputHorizontal * horizontalSpeed;
        velocity.y = 0;

        if (Mathf.Abs(inputHorizontal) > 0 && inputHorizontal != horizontalDirection) {

            Flip();

        }

        Vector2 platformPoint = GetNextPlatformPoint();
        Vector2 wallPoint = GetNextWallPoint();

        gameObject.transform.position = Move();

        if (gameObject.transform.position.x == wallPoint.x) {

            state = STATE.PLAYER_IDLE;

            return;

        }

        if (platformPoint.y < platformTrigger.position.y) {

            state = STATE.PLAYER_FALLING;

            return;

        }

        if (inputJump) {

            inputJumpsAvalible -= 1;

            state = STATE.PLAYER_JUMPING;

            return;

        }

        if (inputHorizontal == 0) {

            state = STATE.PLAYER_IDLE;

            return;

        }

    }

    void Falling() {

        if (Mathf.Abs(inputHorizontal) > 0) {

            velocity.x = inputHorizontal * horizontalSpeed;

        }

        if (Mathf.Abs(inputHorizontal) > 0 && inputHorizontal != horizontalDirection) {

            Flip();

        }

        velocity.y -= gravity * Time.deltaTime;

        Vector2 platformPoint = GetNextPlatformPoint();
        Vector2 wallPoint = GetNextWallPoint();

        gameObject.transform.position = Move();

        if (inputJumpsAvalible > 0 && inputJump) {

            inputJumpsAvalible -= 1;

            velocity.y = 0;

            state = STATE.PLAYER_JUMPING;

            return;

        }

        if (gameObject.transform.position.x == wallPoint.x) {

            state = STATE.PLAYER_WALL_SLIDE;

            return;

        }

        if (gameObject.transform.position.y == platformPoint.y) {

            state = STATE.PLAYER_RUNNING;

            return;

        }

    }

    void Jumping() {

        if (Mathf.Abs(inputHorizontal) > 0) {

            velocity.x = inputHorizontal * horizontalSpeed;

        }

        if (Mathf.Abs(inputHorizontal) > 0 && inputHorizontal != horizontalDirection) {

            Flip();

        }

        if (velocity.y == 0) {

            velocity.y = jumpSpeed;

        } else {

            velocity.y -= gravity * Time.deltaTime;

        }

        Vector2 platformPoint = GetNextPlatformPoint();
        Vector2 wallPoint = GetNextWallPoint();

        gameObject.transform.position = Move();

        if (inputJumpsAvalible > 0 && inputJump) {

            inputJumpsAvalible -= 1;

            velocity.y = 0;

            state = STATE.PLAYER_JUMPING;

            return;

        }

        if (gameObject.transform.position.x == wallPoint.x) {

            state = STATE.PLAYER_WALL_SLIDE;

            return;

        }

        if (velocity.y <= 0) {

            state = STATE.PLAYER_FALLING;

            return;

        }

    }

    void WallSlide() {

        inputJumpsAvalible = maxAvalibleJumps;

        velocity.x = 0;

        if (velocity.y > 0) {

            velocity.y -= gravity * Time.deltaTime;

        } else {

            velocity.y = wallSlideSpeed;

        }

        Vector2 platformPoint = GetNextPlatformPoint();
        Vector2 wallPoint = GetNextWallPoint();

        gameObject.transform.position = Move();

        if (inputJump) {

            state = STATE.PLAYER_WALL_JUMP;

            return;

        }

        if (Mathf.Abs(inputHorizontal) > 0 && inputHorizontal != horizontalDirection) {

            state = STATE.PLAYER_WALL_DISMOUNT;

            return;

        }

        if (gameObject.transform.position.y == platformPoint.y) {

            state = STATE.PLAYER_IDLE;

            return;

        }

    }

    void WallJump() {

        Flip();

        StartCoroutine(DisallowHorizontalMovement());

        velocity.x = horizontalDirection * horizontalSpeed;
        velocity.y = jumpSpeed;

        inputJumpsAvalible -= 1;

        state = STATE.PLAYER_JUMPING;

    }

    void WallDismount() {

        Flip();

        velocity.x = inputHorizontal * horizontalSpeed;
        velocity.y = 0;

        state = STATE.PLAYER_FALLING;

    }

    void Flip() {

        Vector3 scale = gameObject.transform.localScale;
        horizontalDirection *= -1;
        scale.x *= -1;
        gameObject.transform.localScale = scale;

    }

    void UpdateHitVectors() {

        Bounds colliderBounds = gameObject.GetComponent<BoxCollider2D>().bounds;

        RaycastHit2D hitLeftRay = Physics2D.Raycast(new Vector2(colliderBounds.min.x, colliderBounds.center.y), Vector2.left, raycastDistance, wallLayerMask);
        RaycastHit2D hitRightRay = Physics2D.Raycast(new Vector2(colliderBounds.max.x, colliderBounds.center.y), Vector2.right, raycastDistance, wallLayerMask);
        RaycastHit2D hitBottomRay = Physics2D.Raycast(new Vector2(colliderBounds.center.x, colliderBounds.min.y), Vector2.down, raycastDistance, platformLayerMask);

        Vector2 position = gameObject.transform.position;

        if (hitLeftRay) {

            hitLeft = new Vector2(hitLeftRay.collider.bounds.max.x + colliderBounds.extents.x, 0);

        } else {

            hitLeft = Vector2.zero;

        }

        if (hitRightRay) {

            hitRight = new Vector2(hitRightRay.collider.bounds.min.x - colliderBounds.extents.x, 0);

        } else {

            hitRight = Vector2.zero;

        }

        if (hitBottomRay) {

            hitBottom = new Vector2(0, hitBottomRay.collider.bounds.max.y + colliderBounds.extents.y);

        } else {

            hitBottom = Vector2.zero;

        }

    }

    Vector2 Move() {

        Vector2 position = gameObject.transform.position;

        position += velocity * Time.deltaTime;

        if (hitLeft.x != 0) {

            position.x = Mathf.Max(position.x, hitLeft.x);

        }

        if (hitRight.x != 0) {

            position.x = Mathf.Min(position.x, hitRight.x);

        }

        if (hitBottom.y != 0) {

            position.y = Mathf.Max(position.y, hitBottom.y);

        }

        return position;

    }

    void ResetInputVariables() {

        _inputJump = false;

    }

    Vector2 GetNextPlatformPoint() {

        RaycastHit2D[] platforms = Physics2D.RaycastAll(platformTrigger.position, -gameObject.transform.up, raycastDistance, platformLayerMask);

        foreach (RaycastHit2D platform in platforms) {

            Vector2 platformPoint = platform.point - (Vector2)(platformTrigger.localPosition * gameObject.transform.localScale.y);

            if (RoundFloat(platform.point.y) == RoundFloat(platform.collider.bounds.max.y)) {

                Debug.DrawLine(gameObject.transform.position, platformPoint, Color.green);

                return RoundVector2(platformPoint);

            }

        }

        return gameObject.transform.position + Vector3.down;

    }

    Vector2 GetNextWallPoint() {

        RaycastHit2D[] walls = Physics2D.RaycastAll(wallTrigger.position, wallTrigger.right * horizontalDirection, raycastDistance, wallLayerMask);

        foreach (RaycastHit2D wall in walls) {

            Vector2 wallPoint = wall.point - (Vector2)(wallTrigger.localPosition * gameObject.transform.localScale.x);

            if (RoundFloat(wall.point.x) == RoundFloat(wall.collider.bounds.min.x) ||
                RoundFloat(wall.point.x) == RoundFloat(wall.collider.bounds.max.x)) {

                Debug.DrawLine(gameObject.transform.position, wallPoint, Color.green);

                return RoundVector2(wallPoint);

            }

        }

        return gameObject.transform.position + Vector3.right * horizontalDirection;

    }

    Vector2 RoundVector2(Vector2 vector, int digits = 2) {

        vector.x = RoundFloat(vector.x, digits);
        vector.y = RoundFloat(vector.y, digits);

        return vector;

    }

    float RoundFloat(float value, int digits = 2) {

        return (float) Math.Round((double) value, digits);

    }

    IEnumerator DisallowHorizontalMovement() {

        inputHorizontal = 0;

        inputHorizontalEnabled = false;

        yield return horizontalMovementDelay;

        inputHorizontalEnabled = true;

    }

}
