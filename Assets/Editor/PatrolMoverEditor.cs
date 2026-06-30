using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolMover))]
public class PatrolMoverEditor : Editor
{
    void OnSceneGUI()
    {
        PatrolMover mover = (PatrolMover)target;
        serializedObject.Update();

        SerializedProperty waypointsProp = serializedObject.FindProperty("waypoints");
        if (waypointsProp == null || waypointsProp.arraySize < 2)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        Transform t = mover.transform;

        for (int i = 0; i < waypointsProp.arraySize; i++)
        {
            SerializedProperty elem = waypointsProp.GetArrayElementAtIndex(i);
            Vector3 world = t.TransformPoint(elem.vector3Value);

            EditorGUI.BeginChangeCheck();
            world = Handles.PositionHandle(world, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(mover, "Move Waypoint");
                Vector3 local = t.InverseTransformPoint(world);
                local.z = 0f;
                elem.vector3Value = local;
                serializedObject.ApplyModifiedProperties();
            }
        }

        Handles.color = new Color(0f, 1f, 1f, 0.85f);
        for (int i = 0; i < waypointsProp.arraySize; i++)
        {
            Vector3 a = t.TransformPoint(waypointsProp.GetArrayElementAtIndex(i).vector3Value);
            Vector3 b = t.TransformPoint(waypointsProp.GetArrayElementAtIndex((i + 1) % waypointsProp.arraySize).vector3Value);
            Handles.DrawAAPolyLine(3f, a, b);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
