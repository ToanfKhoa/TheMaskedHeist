using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TeleportDoor))]
public class TeleportDoorEditor : Editor
{
    // This function draws tools inside the Scene View
    private void OnSceneGUI()
    {
        // 1. Get reference to the script
        TeleportDoor door = (TeleportDoor)target;

        // 2. If no destination is set, stop here
        if (door.destinationPoint == null) return;

        // 3. Create a handle (the move tool gizmo) at the destination point
        EditorGUI.BeginChangeCheck();

        Vector3 newPosition = Handles.PositionHandle(
            door.destinationPoint.position,
            Quaternion.identity
        );

        // 4. If the user moved the handle, update the transform
        if (EditorGUI.EndChangeCheck())
        {
            // Allow Undo (Ctrl+Z) functionality
            Undo.RecordObject(door.destinationPoint, "Move Door Exit");
            door.destinationPoint.position = newPosition;
        }

        // 5. Draw a visual line connecting Door -> Exit
        Handles.color = Color.green;
        Handles.DrawDottedLine(door.transform.position, door.destinationPoint.position, 4f);

        // 6. Draw a Label so you can see it clearly
        Handles.Label(door.destinationPoint.position + Vector3.up * 0.5f, "Exit Point");
    }

    // This function customizes the Inspector window (where variables are)
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (variables)
        DrawDefaultInspector();

        TeleportDoor door = (TeleportDoor)target;

        // Add a helpful button to create the exit point automatically
        if (door.destinationPoint == null)
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Create Exit Point"))
            {
                CreateExitPoint(door);
            }
        }
    }

    private void CreateExitPoint(TeleportDoor door)
    {
        // Create a new empty GameObject
        GameObject exitPoint = new GameObject(door.name + "_Exit");

        // Position it slightly to the right of the door
        exitPoint.transform.position = door.transform.position + Vector3.right * 2;

        // Assign it to the script
        door.destinationPoint = exitPoint.transform;

        // Register Undo so you can Ctrl+Z the creation
        Undo.RegisterCreatedObjectUndo(exitPoint, "Create Door Exit");
    }
}