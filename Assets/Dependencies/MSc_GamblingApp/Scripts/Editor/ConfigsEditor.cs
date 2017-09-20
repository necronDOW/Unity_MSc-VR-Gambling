using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Configs))]
public class ConfigsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUILayout.Label("In order to change these properties, open the Export Manager found at Window/Export Manager at the top of the screen. Then drag and drop this file onto the Configs box within the newly opened window.", EditorStyles.wordWrappedLabel);
    }
}
