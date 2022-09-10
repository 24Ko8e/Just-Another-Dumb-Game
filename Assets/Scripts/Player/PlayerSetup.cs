using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    [Header("Character Controller Collider Configuration")]
    [SerializeField] CharacterController playerCharacterController;
    [SerializeField] Transform headBone;
    [SerializeField] float headOffset;
    [SerializeField] Transform feetBone;
    [SerializeField] float feetOffset;

    private void Update()
    {
        playerCharacterController.height = headBone.position.y + headOffset - feetBone.position.y - feetOffset;
        playerCharacterController.center = new Vector3(0, (headBone.position.y + headOffset + feetBone.position.y + feetOffset) / 2, 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        playerCharacterController.height = headBone.position.y + headOffset - feetBone.position.y - feetOffset;
        playerCharacterController.center = new Vector3(0, (headBone.position.y + headOffset + feetBone.position.y + feetOffset) / 2, 0);
    }
#endif
}
