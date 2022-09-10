using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] CharacterController characterController;

    [Header("Controls")]
    float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;

    [Header("Locomotion")]
    Vector3 moveVector;
    Vector3 currentMoveVector;
    [SerializeField] Transform cameraT;
    float verticalVelocity = 0f;
    [SerializeField] float gravity;
    bool isGrounded = true;

    [Header("Slope Handling")]
    Vector3 slopeNormal;
    [SerializeField] float maxSlopeAngle;
    bool isStandingOnSlope = false;

    void Start()
    {
        currentMoveVector = moveVector;
        verticalVelocity = transform.localPosition.y;
        moveSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        CheckPlayerGround();
        UpdatePlayerMovement();
    }

    void UpdatePlayerMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveVector = new Vector3(h, 0, v);
        float inputMagnitude = Mathf.Clamp01(moveVector.magnitude);
        float speed = inputMagnitude * moveSpeed;
        moveVector = Quaternion.AngleAxis(cameraT.rotation.eulerAngles.y, Vector3.up) * moveVector;
        moveVector.Normalize();

        if (isGrounded)
            verticalVelocity = 0;
        else
            verticalVelocity += gravity * Time.deltaTime;

        moveVector *= speed;
        if (isStandingOnSlope)
            moveVector = Vector3.ProjectOnPlane(moveVector, slopeNormal).normalized;
        currentMoveVector.y = verticalVelocity;
        currentMoveVector = Vector3.Lerp(currentMoveVector, moveVector, 100f * Time.deltaTime);
        characterController.Move(currentMoveVector * Time.deltaTime);

        if (moveVector.x != 0 || moveVector.z != 0)
            transform.Rotate(Vector3.up * (Vector3.SignedAngle(transform.forward, moveVector, Vector3.up) * 10f * Time.deltaTime), Space.World);
    }

    void CheckPlayerGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, Vector3.down);
            if (hit.distance < 0.1f)
            {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                isStandingOnSlope = angle < maxSlopeAngle && angle != 0;
                slopeNormal = hit.normal;
                isGrounded = true;
            }
            else
            {
                isStandingOnSlope = false;
                isGrounded = false;
            }
        }
        else
        {
            isStandingOnSlope = false;
            isGrounded = false;
        }
    }
}
