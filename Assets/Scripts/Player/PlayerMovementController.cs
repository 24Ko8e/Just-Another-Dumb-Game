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
    [SerializeField] Animator playerAnimator;
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

    [Header("Character Controller Collider Configuration")]
    [SerializeField] Transform headBone;
    [SerializeField] float headOffset;
    [SerializeField] Transform feetBone;
    [SerializeField] float feetOffset;

    void Start()
    {
        CalculateCapsuleCollider();
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
        {
            if (isStandingOnSlope)
            {
                float slope = Vector3.Dot(Vector3.Cross(moveVector, Vector3.down), Vector3.Cross(Vector3.up, slopeNormal));

                if (slope < 0)
                    moveVector = Vector3.ProjectOnPlane(moveVector, slopeNormal).normalized;
                else if (slope > 0)
                    moveVector = (moveVector * moveSpeed + (isStandingOnSlope && slope > 0 && moveVector != Vector3.zero ? Vector3.down * moveSpeed * 90F * Time.deltaTime : Vector3.zero)).normalized;
                
                verticalVelocity = 0;
            }
            else
            {
                verticalVelocity = gravity;
            }
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
        moveVector *= speed;
        currentMoveVector.y = verticalVelocity;
        currentMoveVector = Vector3.Lerp(currentMoveVector, moveVector, 100f * Time.deltaTime);
        characterController.Move(currentMoveVector * Time.deltaTime);

        playerAnimator.SetFloat("Forward", speed / moveSpeed);

        Debug.DrawLine(transform.position, transform.position + currentMoveVector);
        if (currentMoveVector.x != 0 || currentMoveVector.z != 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(currentMoveVector.x, transform.forward.y, currentMoveVector.z), Vector3.up);
            if (lookRotation != Quaternion.Euler(Vector3.zero))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, 1000f * Time.deltaTime);
            }
        }
    }

    void CheckPlayerGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity))
        {
            slopeNormal = hit.normal;
            Debug.DrawRay(transform.position, Vector3.down);
            if (hit.distance <= 0.08f)
            {
                float angle = Vector3.Angle(Vector3.up, hit.normal);
                isStandingOnSlope = angle < maxSlopeAngle && angle != 0;
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

    void CalculateCapsuleCollider()
    {
        characterController.height = headBone.position.y + headOffset - feetBone.position.y - feetOffset;
        characterController.center = new Vector3(0, (headBone.position.y + headOffset + feetBone.position.y + feetOffset) / 2, 0);
    }
}
