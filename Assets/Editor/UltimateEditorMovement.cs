using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterMovement))]
public class UltimateEditorMovement : Editor
{
    GUIStyle headerStyle;
    GUIStyle foldoutStyle;

    void OnEnable()
    {
        headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter
        };

        foldoutStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold
        };
    }

    public override void OnInspectorGUI()
    {
        CharacterMovement ctrl = (CharacterMovement)target;

        DrawHeader("CHARACTER MOVEMENT");

        EditorGUILayout.Space();

        // REFERENCES
        DrawSection("References", ref ctrl._editorFoldoutReferences, () =>
        {
            ctrl.cameraTransform = (Transform)EditorGUILayout.ObjectField(
                "Camera",
                ctrl.cameraTransform,
                typeof(Transform),
                true
            );

            if (ctrl.cameraTransform == null)
            {
                EditorGUILayout.HelpBox(
                    "Camera is required when 'Move Relative To Camera' is enabled.",
                    MessageType.Error
                );

                if (GUILayout.Button("Auto Assign Main Camera"))
                {
                    if (Camera.main != null)
                        ctrl.cameraTransform = Camera.main.transform;
                }
            }
        });

        // MOVEMENT BASIC
        DrawSection("Movement - Basic", ref ctrl._editorFoldoutMovement, () =>
        {
            ctrl.AllowMovement = EditorGUILayout.Toggle("Allow Movement", ctrl.AllowMovement);
            ctrl.walkSpeed = EditorGUILayout.FloatField("Walk Speed", ctrl.walkSpeed);
            ctrl.allowRunning = EditorGUILayout.Toggle("Allow Running", ctrl.allowRunning);
            ctrl.runSpeed = EditorGUILayout.FloatField("Run Speed", ctrl.runSpeed);
        });

        // MOVEMENT ADVANCED
        DrawSection("Movement - Advanced", ref ctrl._editorFoldoutMovementAdvanced, () =>
        {
            ctrl.acceleration = EditorGUILayout.FloatField("Acceleration", ctrl.acceleration);
            ctrl.airControlMultiplier = EditorGUILayout.Slider(
                "Air Control",
                ctrl.airControlMultiplier,
                0f,
                1f
            );
            ctrl.moveRelativeToCamera =
                EditorGUILayout.Toggle("Move Relative To Camera", ctrl.moveRelativeToCamera);

            ctrl.rotateTowardsMovement =
                EditorGUILayout.Toggle("Rotate Towards Movement", ctrl.rotateTowardsMovement);

            ctrl.rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", ctrl.rotationSpeed);
        });

        // JUMP BASIC
        DrawSection("Jump - Basic", ref ctrl._editorFoldoutJump, () =>
        {
            ctrl.allowJump = EditorGUILayout.Toggle("Allow Jump", ctrl.allowJump);
            ctrl.jumpForce = EditorGUILayout.FloatField("Jump Force", ctrl.jumpForce);
        });

        // JUMP ADVANCED
        DrawSection("Jump - Advanced", ref ctrl._editorFoldoutJumpAdvanced, () =>
        {
            ctrl.gravity = EditorGUILayout.FloatField("Gravity", ctrl.gravity);
            ctrl.coyoteTime = EditorGUILayout.FloatField("Coyote Time", ctrl.coyoteTime);
            ctrl.jumpBufferTime = EditorGUILayout.FloatField("Jump Buffer Time", ctrl.jumpBufferTime);
        });


        DrawSection("Slope Adaptation", ref ctrl._editorFoldoutSlope, () =>
        {
            ctrl.adaptToSlope =
                EditorGUILayout.Toggle("Adapt To Slope", ctrl.adaptToSlope);

            if (ctrl.adaptToSlope)
            {
                ctrl.visual = (Transform)EditorGUILayout.ObjectField(
                    "Visual Root",
                    ctrl.visual,
                    typeof(Transform),
                    true
                );

                ctrl.slopeRotationSpeed =
                    EditorGUILayout.FloatField("Rotation Speed", ctrl.slopeRotationSpeed);

                ctrl.maxSlopeAngle =
                    EditorGUILayout.Slider("Max Slope Angle", ctrl.maxSlopeAngle, 0f, 80f);

                if (ctrl.visual == null)
                {
                    EditorGUILayout.HelpBox(
                        "You must assign a Visual Root (child object) when Adapt To Slope is enabled.",
                        MessageType.Error
                    );
                }
            }
        });

        if (GUI.changed)
            EditorUtility.SetDirty(ctrl);
    }

    void DrawHeader(string title)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 34);
        EditorGUI.DrawRect(rect, new Color(0.365f, 0.075f, 0.502f, 1.000f));
        GUI.Label(rect, title, headerStyle);
    }

    void DrawSection(string title, ref bool foldout, System.Action content)
    {
        EditorGUILayout.BeginVertical("box");
        foldout = EditorGUILayout.Foldout(foldout, title, true, foldoutStyle);

        if (foldout)
        {
            EditorGUILayout.Space(4);
            content.Invoke();
        }

        EditorGUILayout.EndVertical();
    }
}
