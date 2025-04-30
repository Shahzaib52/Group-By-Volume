using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class GroupByVolume : EditorWindow
{
    [SerializeField] private Vector3 m_PositionOffset;
    [SerializeField] private Vector3 m_ScaleOffset;


    [MenuItem("Tools/FYG/Group By Volume")]
    private static void ShowWindow()
    {
        GetWindow<GroupByVolume>("Group By Volume");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        GUILayout.Label("Volume", EditorStyles.boldLabel);
        m_PositionOffset = EditorGUILayout.Vector3Field("Position", m_PositionOffset);
        m_ScaleOffset = EditorGUILayout.Vector3Field("Scale", m_ScaleOffset);

        if (GUILayout.Button("Select Overlapping Objects"))
        {
            if (!Selection.activeGameObject) return;
            var renderer = Selection.activeGameObject.GetComponent<MeshRenderer>();

            if (!renderer) return;
            Vector3 size = renderer.bounds.size + m_ScaleOffset;
            Vector3 center = renderer.bounds.center + m_PositionOffset;

            var results = Physics.OverlapBox(center, size / 2);
            GameObject[] gameObjects = new GameObject[results.Length];
            for (int i = 0; i < gameObjects.Length; i++) gameObjects[i] = results[i].gameObject;

            Selection.objects = gameObjects;
        }

        if (GUILayout.Button("Group Overlapping Objects"))
        {
            GroupObjects();
        }
    }

    private void OnSceneGUI(SceneView view)
    {
        var selection = Selection.gameObjects;
        foreach (var gameObject in selection)
        {
            if (!gameObject.activeInHierarchy) continue;
            var renderer = gameObject.GetComponent<MeshRenderer>();
            if (renderer)
            {
                Handles.color = Color.blue;
                Vector3 size = renderer.bounds.size + m_ScaleOffset;
                Vector3 center = renderer.bounds.center + m_PositionOffset;
                Handles.DrawWireCube(center, size);

                var results = Physics.OverlapBox(center, size / 2);
                for (int i = 0; i < results.Length; i++)
                {
                    if (results[i].gameObject == renderer.gameObject) continue;

                    var tempRenderer = results[i].gameObject.GetComponent<MeshRenderer>();
                    if (tempRenderer)
                    {
                        Handles.color = Color.green;
                        Handles.DrawWireCube(tempRenderer.bounds.center, tempRenderer.bounds.size);
                    }
                }
            }
        }
    }

    private void GroupObjects()
    {
        var selection = Selection.gameObjects;
        foreach (var gameObject in selection)
        {
            if (!gameObject.activeInHierarchy) continue;
            
            Collider[] results = null;
            Vector3 size = Vector3.zero;
            Vector3 center = Vector3.zero;
            MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer)
            {
                size = meshRenderer.bounds.size + m_ScaleOffset;
                center = meshRenderer.bounds.center + m_PositionOffset;
                results = Physics.OverlapBox(center, size / 2);

                foreach (var col in results)
                {
                    if (col.gameObject.GetInstanceID() == gameObject.GetInstanceID()) continue;
                    if (col.transform.parent.GetInstanceID() == gameObject.transform.GetInstanceID()) continue;

                    var isSmaller = col.bounds.size.x < size.x && col.bounds.size.y < size.y && col.bounds.size.z < size.z;

                    if (isSmaller)
                    {
                        Selection.activeObject = col.gameObject;
                        Undo.RecordObject(col.gameObject, $"duplicate_{col.gameObject.GetInstanceID()}");
                        var newInstance = GameObjectUtility.DuplicateGameObject(col.gameObject);
                        Undo.RecordObject(col.gameObject, $"select_{newInstance.GetInstanceID()}");
                        Selection.activeObject = newInstance;
                        Debug.Log($"Name: {col.gameObject.name}");
                        var newTransform = newInstance.transform;
                        Undo.RecordObject(newInstance, $"setPosition_{newInstance.GetInstanceID()}");
                        newTransform.position = col.transform.position;
                        Undo.RecordObject(newInstance, $"setRotation_{newInstance.GetInstanceID()}");
                        newTransform.rotation = col.transform.rotation;
                        Undo.RecordObject(newInstance, $"setParent_{newInstance.GetInstanceID()}");
                        newTransform.transform.SetParent(gameObject.transform);
                        Undo.RecordObject(col.gameObject, $"destroy_{col.gameObject.GetInstanceID()}");
                        // DestroyImmediate(col.gameObject);
                        col.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {

    }
}
