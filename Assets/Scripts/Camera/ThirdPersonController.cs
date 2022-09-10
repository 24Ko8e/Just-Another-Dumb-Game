using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    [SerializeField] float sensitivity = 1f;
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] float angleLimit = 90f;
    [SerializeField] bool invert;
    [SerializeField] Vector3 pivotTransformOffset;

    [SerializeField] float Yaw;
    float currentYaw;
    [SerializeField] float Pitch;
    float currentPitch;
    [SerializeField] float Roll;
    float currentRoll;

    [SerializeField] float zoomLevel;

    void Start()
    {
        transform.SetParent(null);
    }

    void Update()
    {
        Yaw += Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        currentYaw = Mathf.Lerp(currentYaw, Yaw, lerpSpeed);
        Pitch += invert ? Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime : -1 * Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;
        currentPitch = Mathf.Lerp(currentPitch, Pitch, lerpSpeed);
        currentRoll = Mathf.Lerp(currentRoll, Roll, lerpSpeed);
        Pitch = Mathf.Clamp(Pitch, -angleLimit, angleLimit);

        transform.position = playerTransform.position + pivotTransformOffset;
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, currentRoll);
        transform.Translate(new Vector3(0, 0, -zoomLevel));
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        currentPitch = Pitch;
        currentYaw = Yaw;
        currentRoll = Roll;

        transform.position = playerTransform.position + pivotTransformOffset;
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, currentRoll);
        transform.Translate(new Vector3(0, 0, -zoomLevel));
    }
#endif
}
