using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerController))]
public class PlayerSetup : Editor
{ 
    bool ShowMovementInfo = false;
    bool ShowCameraInfo = false;

    public override void OnInspectorGUI()
    {
        PlayerController playerController = target as PlayerController;

        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(playerController, "Change Player Settings");

        ShowMovementInfo = EditorGUILayout.Foldout(ShowMovementInfo, "Movement Settings", true);
        if (ShowMovementInfo)
        {
            EditorGUI.indentLevel++;

            // Edit movement speed
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Walk Speed", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.WalkSpeed = EditorGUILayout.FloatField(playerController.WalkSpeed);
            GUILayout.EndHorizontal();

            // Edit running speed
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Run Speed", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.runSpeed = EditorGUILayout.FloatField(playerController.runSpeed);
            GUILayout.EndHorizontal();

            // Edit acceleration
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Acceleration", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.acceleration = EditorGUILayout.FloatField(playerController.acceleration);
            GUILayout.EndHorizontal();

            // Edit acceleration
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Deceleration", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.deceleration = EditorGUILayout.FloatField(playerController.deceleration);
            GUILayout.EndHorizontal();

            // Set movement smoothing
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Move Smooth Time", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.moveSmoothTime = EditorGUILayout.FloatField(playerController.moveSmoothTime);
            GUILayout.EndHorizontal();
            
            // Edit jump force
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Jump Force", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.jumpForce = EditorGUILayout.FloatField(playerController.jumpForce);
            GUILayout.EndHorizontal();

            // Edit gravity force
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gravity", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.gravity = EditorGUILayout.FloatField(playerController.gravity);
            GUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
        }

        ShowCameraInfo = EditorGUILayout.Foldout(ShowCameraInfo, "Camera Settings", true);
        if (ShowCameraInfo)
        {
            EditorGUI.indentLevel++;

            // Get camera reference
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Camera", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            if (playerController.m_camera != null)
            {
                playerController.m_camera = (Camera)EditorGUILayout.ObjectField(playerController.m_camera, typeof(Camera), true);
            }
            GUILayout.EndHorizontal();
            
            // Set camera type
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Camera Type", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
            playerController.cameraType = (PlayerController.CameraType)EditorGUILayout.EnumPopup(playerController.cameraType);
            GUILayout.EndHorizontal();

            if (playerController.cameraType == PlayerController.CameraType.FirstPerson)
            {
                // Set FP camera offset
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Offset", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
                playerController.FPcameraOffset = EditorGUILayout.Vector3Field("", playerController.FPcameraOffset);
                GUILayout.EndHorizontal();

                // Set mouse Sensitivity
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Mouse Sensitivity", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
                playerController.mouseSensitivity = EditorGUILayout.FloatField(playerController.mouseSensitivity);
                GUILayout.EndHorizontal();

                // Set verticlal angle limit
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Vertical Angle Limit", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
                playerController.verticalAngleLimit = EditorGUILayout.FloatField(playerController.verticalAngleLimit);
                GUILayout.EndHorizontal();

                // Set mouse smoothing
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Mouse Smooth Time", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
                playerController.mouseSmoothTime = EditorGUILayout.FloatField(playerController.mouseSmoothTime);
                GUILayout.EndHorizontal();

                // Toggle cursor lock
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Lock Cursor On Start", GUILayout.MaxWidth(125), GUILayout.MinWidth(0));
                playerController.lockCursor = EditorGUILayout.Toggle(playerController.lockCursor);
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Apply Settings", GUILayout.Width(125), GUILayout.Height(20)))
        {
            playerController.SetPlayer();
        }

        if (EditorGUI.EndChangeCheck())
        {
            playerController.SetPlayer();
        }
    }
}
