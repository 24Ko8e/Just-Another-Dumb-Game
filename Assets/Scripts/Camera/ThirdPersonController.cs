using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    [SerializeField] Transform playerTransform;

    [Header("Settings")]
    [SerializeField] float sensitivity = 1f;
    [SerializeField] float lerpSpeed = 10f;
    [SerializeField] float angleLimit = 90f;
    [SerializeField] bool invert;
    [SerializeField] Vector3 pivotTransformOffset;

    [Header("Yaw, Pitch, Roll, Zoom")]
    [SerializeField] float Yaw;
    float currentYaw;
    [SerializeField] float Pitch;
    float currentPitch;
    [SerializeField] float Roll;
    float currentRoll;
    [SerializeField] float zoomLevel;

    [Header("Camera Collision")]
    [SerializeField] Camera cam;
    [SerializeField] LayerMask collisionLayer;
    [SerializeField] float collisionSpace = 3.41f;
    bool isColliding = false;
    Vector3[] desiredCamereaClipPoints;
    Vector3[] adjustedCamereaClipPoints;


    void Start()
    {
        transform.SetParent(null);
        Initialize();
    }

    void Update()
    {
        Yaw += Input.GetAxisRaw("Mouse X") * sensitivity * Time.deltaTime;
        currentYaw = Mathf.Lerp(currentYaw, Yaw, lerpSpeed);
        Pitch += invert ? Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime : -1 * Input.GetAxisRaw("Mouse Y") * sensitivity * Time.deltaTime;
        Pitch = Mathf.Clamp(Pitch, -angleLimit, angleLimit);
        currentPitch = Mathf.Lerp(currentPitch, Pitch, lerpSpeed);
        currentRoll = Mathf.Lerp(currentRoll, Roll, lerpSpeed);

        transform.position = playerTransform.position + pivotTransformOffset;
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, currentRoll);
        transform.Translate(new Vector3(0, 0, -zoomLevel));

        CheckCollision(transform.position);
        Debug.Log(GetAdjustedDistance(transform.position));
    }

    void Initialize()
    {
        adjustedCamereaClipPoints = new Vector3[5];
        desiredCamereaClipPoints = new Vector3[5];
    }

    bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
    {
        for (int i = 0; i < clipPoints.Length; i++)
        {
            Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
            float dist = Vector3.Distance(clipPoints[i], fromPosition);
            if (Physics.Raycast(ray, dist, collisionLayer))
            {
                return true;
            }
        }
        return false;
    }

    void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion AtRotation, ref Vector3[] intoArray)
    {
        if (!cam)
            return;

        intoArray = new Vector3[5];
        float z = cam.nearClipPlane;
        float x = Mathf.Tan(cam.fieldOfView / collisionSpace) * z;
        float y = x / cam.aspect;

        intoArray[0] = (AtRotation * new Vector3(-x, y, z)) + cameraPosition;
        intoArray[1] = (AtRotation * new Vector3(x, -y, z)) + cameraPosition;
        intoArray[2] = (AtRotation * new Vector3(-x, -y, z)) + cameraPosition;
        intoArray[3] = (AtRotation * new Vector3(x, y, z)) + cameraPosition;
        intoArray[4] = cameraPosition - cam.transform.forward;
    }

    float GetAdjustedDistance(Vector3 from)
    {
        float distance = -1;

        for (int i = 0; i < desiredCamereaClipPoints.Length; i++)
        {
            Ray ray = new Ray(from, desiredCamereaClipPoints[i] - from);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (distance == -1)
                    distance = hit.distance;
                else
                {
                    if (hit.distance < distance)
                        distance = hit.distance;
                }
            }
        }

        if (distance == -1)
            return 0;

        else
            return distance;
    }

    void CheckCollision(Vector3 targetPos)
    {
        isColliding = CollisionDetectedAtClipPoints(desiredCamereaClipPoints, targetPos);
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
