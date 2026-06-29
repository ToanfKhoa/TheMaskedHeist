using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DarkRoom))]
public class DarkRoomEditor : Editor
{
    void OnSceneGUI()
    {
        DarkRoom room = (DarkRoom)target;
        serializedObject.Update();

        SerializedProperty pathProp = serializedObject.FindProperty("localPath");
        if (pathProp == null || pathProp.arraySize < 2)
        {
            serializedObject.ApplyModifiedProperties();
            return;
        }

        Transform t = room.transform;

        for (int i = 0; i < pathProp.arraySize; i++)
        {
            SerializedProperty elem = pathProp.GetArrayElementAtIndex(i);
            Vector3 world = t.TransformPoint(elem.vector3Value);

            EditorGUI.BeginChangeCheck();
            world = Handles.PositionHandle(world, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(room, "Move Dark Room Waypoint");
                Vector3 local = t.InverseTransformPoint(world);
                local.z = 0f;
                elem.vector3Value = local;
                serializedObject.ApplyModifiedProperties();
            }
        }

        Handles.color = new Color(0f, 1f, 1f, 0.85f);
        for (int i = 0; i < pathProp.arraySize; i++)
        {
            Vector3 a = t.TransformPoint(pathProp.GetArrayElementAtIndex(i).vector3Value);
            Vector3 b = t.TransformPoint(pathProp.GetArrayElementAtIndex((i + 1) % pathProp.arraySize).vector3Value);
            Handles.DrawAAPolyLine(3f, a, b);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
