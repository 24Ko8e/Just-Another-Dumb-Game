using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // movement settings
    public float WalkSpeed;
    public float runSpeed;
    public float acceleration;
    public float deceleration;
    public float moveSmoothTime = 0.3f;
    public float jumpForce;
    public float gravity = -13.0f;
    public bool PhysicsCalculations;
    [SerializeField]
    Rigidbody playerRigidbody;
    [SerializeField]
    CharacterController characterController;
    [SerializeField]
    CapsuleCollider playerCollider;

    // camera settings
    public Camera m_camera;
    public enum CameraType
    {
        FirstPerson,
        ThirdPerson
    }
    public CameraType cameraType;
    public Vector3 FPcameraOffset;
    public Vector3 TPcameraOffset;
    public float mouseSensitivity;
    public float verticalAngleLimit;
    public float mouseSmoothTime = 0.03f;
    public bool lockCursor = false;

    // Runtime only variables
    Vector3 move_velocity;
    Vector3 groundNormal;
    float moveSpeed;
    float slope;
    Vector2 currDir = Vector2.zero;
    Vector2 currDirVelocity = Vector2.zero;

    Vector2 currMouseDelta = Vector2.zero;
    Vector2 currMouseDeltaVelocity= Vector2.zero;
    
    float cameraPitch = 0f;
    bool isPlayerGrounded = true;
    bool isPlayerOnSlope = false;

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        UpdateFpsMouseRotation();

        isPlayerGrounded = CheckIsPlayerGrounded();
        SetCameraPosition();
        Jump();
    }

    private void Jump()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log(isPlayerGrounded);
            if (isPlayerGrounded)
            {
                playerRigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                isPlayerGrounded = false;
                //velocityY = 0f;
            }
        }
    }

    private void UpdateFpsMouseRotation()
    {
        if (cameraType == CameraType.FirstPerson)
        {
            Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            currMouseDelta = Vector2.SmoothDamp(currMouseDelta, targetMouseDelta, ref currMouseDeltaVelocity, mouseSmoothTime);

            cameraPitch -= currMouseDelta.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -verticalAngleLimit, verticalAngleLimit);

            m_camera.transform.localEulerAngles = Vector3.right * cameraPitch;
            transform.Rotate(Vector3.up * currMouseDelta.x * mouseSensitivity);
        }
    }

    void FixedUpdate()
    {
        UpdateFpsMovement();
        playerRigidbody.AddForce(transform.up * gravity, ForceMode.Force);
    }

    private void UpdateFpsMovement()
    {
        UpdateSpeed();

        if (cameraType == CameraType.FirstPerson)
        {
            Vector2 targetDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            targetDir.Normalize();

            currDir = Vector2.SmoothDamp(currDir, targetDir, ref currDirVelocity, moveSmoothTime);

            Vector3 velocity = (transform.forward * currDir.y + transform.right * currDir.x) * moveSpeed +
                (isPlayerOnSlope && slope > 0 && targetDir != Vector2.zero ? Vector3.down * moveSpeed * 100f * Time.deltaTime : Vector3.zero);

            slope = Vector3.Dot(Vector3.Cross(velocity, Vector3.down), (Vector3.Cross(Vector3.up, groundNormal)));
            playerRigidbody.MovePosition(playerRigidbody.position + (velocity * Time.fixedDeltaTime));
        }
    }

    void UpdateSpeed()
    {
        if (Input.GetKey("left shift"))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, runSpeed, Time.deltaTime * acceleration);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, WalkSpeed, Time.deltaTime * deceleration);
        }
    }

    bool CheckIsPlayerGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, Vector3.down);

            groundNormal = hit.normal;

            if (hit.distance < (playerCollider.height / 2) + 0.1f)
            {
                if (hit.normal != Vector3.up)
                {
                    isPlayerOnSlope = true;
                }
                else
                {
                    isPlayerOnSlope = false;
                }
                return true;
            }
        }
        isPlayerOnSlope = false;
        return false;
    }

    public void SetPlayer()
    {
        AddCamera();
        SetPlayerComponents();
        SetCameraPosition();
    }

    private void AddCamera()
    {
        if (m_camera == null)
        {
            GameObject obj = new GameObject();
            obj.name = "Player Camera";
            obj.transform.SetParent(transform);
            obj.AddComponent<Camera>();
            obj.AddComponent<AudioListener>();
            m_camera = obj.GetComponent<Camera>();
        }
        else
            return;
    }

    void SetPlayerComponents()
    {
        if (playerCollider == null)
        {
            playerCollider = GetComponent<CapsuleCollider>();
            playerCollider.height = 2f;
            playerCollider.center = new Vector3(0f, 1f, 0f);
        }

        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerRigidbody.useGravity = false;
        }
    }

    private void SetCameraPosition()
    {
        if (cameraType == CameraType.FirstPerson)
        {
            m_camera.transform.position = transform.position + FPcameraOffset;
        }
    }
}
