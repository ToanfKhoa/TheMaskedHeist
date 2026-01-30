using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TeleportDoor))]
public class TeleportDoorEditor : Editor
{
    private void OnSceneGUI()
    {
        TeleportDoor door = (TeleportDoor)target;

        if (door.destinationPoint == null) return;

        EditorGUI.BeginChangeCheck();

        // Draw a handle at the destination
        // Since the destination is now the OTHER door, moving this handle 
        // moves the other door entirely!
        Vector3 newPosition = Handles.PositionHandle(
            door.destinationPoint.position,
            Quaternion.identity
        );

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(door.destinationPoint, "Move Connected Door");
            door.destinationPoint.position = newPosition;
        }

        // Draw line
        Handles.color = Color.cyan; // Changed color to indicate a door connection
        Handles.DrawDottedLine(door.transform.position, door.destinationPoint.position, 4f);

        // Draw Label
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.cyan;
        style.alignment = TextAnchor.MiddleCenter;
        Handles.Label(door.destinationPoint.position + Vector3.up * 1.0f, "Connected Door", style);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TeleportDoor door = (TeleportDoor)target;

        // Only show button if there is no destination linked
        if (door.destinationPoint == null)
        {
            EditorGUILayout.Space();
            // Changed button function
            if (GUILayout.Button("Create Linked Door"))
            {
                CreateLinkedDoor(door);
            }
        }
    }
    private void CreateLinkedDoor(TeleportDoor sourceDoor)
    {
        // 1. Create the new door
        GameObject newDoorObj = Instantiate(sourceDoor.gameObject);
        newDoorObj.name = sourceDoor.name + " (Linked)";
        newDoorObj.transform.position = sourceDoor.transform.position + Vector3.right * 4;
        newDoorObj.transform.parent = sourceDoor.transform.parent;

        // 2. Register Undo for the creation of the NEW object
        Undo.RegisterCreatedObjectUndo(newDoorObj, "Create Linked Door");

        // 3. IMPORTANT: Record Undo for the SOURCE door
        // We must do this BEFORE changing variables on the source door
        Undo.RecordObject(sourceDoor, "Link Door Reference");

        // 4. Link Source -> New
        sourceDoor.destinationPoint = newDoorObj.transform;

        // 5. Link New -> Source
        TeleportDoor newDoorScript = newDoorObj.GetComponent<TeleportDoor>();
        if (newDoorScript != null)
        {
            // We don't need Undo here because newDoorObj is fresh
            newDoorScript.destinationPoint = sourceDoor.transform;
        }

        // 6. CRITICAL: Mark the source object as "Dirty"
        // This forces Unity to recognize the change and save it to the scene
        EditorUtility.SetDirty(sourceDoor);

        // 7. Select the new door
        Selection.activeGameObject = newDoorObj;

        Debug.Log("Created new door and linked them bi-directionally!");
    }
}