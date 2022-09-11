using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float maxHealth;
    float currentHealth;
    [SerializeField] float maxStamina;
    float currentStamina;

    [SerializeField] PlayerMovementController movementController;
    [SerializeField] CharacterController characterController;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] Animator playerAnimator;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    void Update()
    {

    }

    [ContextMenu("Die")]
    void Die()
    {
        rigidBody.isKinematic = false;
        movementController.enabled = false;
        characterController.enabled = false;
        playerAnimator.enabled = false;
    }


}
