using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TestCombineMeshes))]
public class TestCombineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        TestCombineMeshes myScript = (TestCombineMeshes)target;
        if (GUILayout.Button("Combine meshes"))
        {
            myScript.Combine();
        }

        if (GUILayout.Button("Hide original meshes"))
        {
            myScript.HideMeshes();
        }

        if (GUILayout.Button("Show original meshes"))
        {
            myScript.ShowMeshes();
        }

        if (GUILayout.Button("Save combined mesh"))
        {
            myScript.SaveAsset();
        }

        if (GUILayout.Button("Turn Off Children Colliders"))
        {
            myScript.RemoveChildrenMeshesColliders();
        }

        if (GUILayout.Button("Turn On Children Colliders"))
        {
            myScript.TurnOnChildrenMeshesColliders();
        }


        if (GUILayout.Button("Destroy Children Meshes (can't be undone)"))
        {
            myScript.DestroyMeshes();
        }
    }
}

#endif