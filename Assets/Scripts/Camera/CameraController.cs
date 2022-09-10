using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public enum Perspective
    {
        thirdPerson,
    }
    public Perspective currentPerspective;

    [SerializeField] Transform thirdPersonTransform;

    void Start()
    {
        transform.SetParent(null);
    }

    void Update()
    {
        SetCameraPositionAndRotation();
    }

    void SetCameraPositionAndRotation()
    {
        switch (currentPerspective)
        {
            default:
            case Perspective.thirdPerson:
                transform.position = thirdPersonTransform.position; 
                transform.rotation = thirdPersonTransform.rotation; 
                break;
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
            return;

        SetCameraPositionAndRotation();
    }
#endif
}
