using UnityEngine;
using UnityEditor;

public class TestEditorWindow : EditorWindow
{
    [MenuItem("Starkiller Base/Test Editor Window")]
    public static void ShowWindow()
    {
        GetWindow<TestEditorWindow>("Test Window");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Test Editor Window", EditorStyles.boldLabel);
        
        if(GUILayout.Button("Click Me"))
        {
            Debug.Log("Button clicked!");
        }
    }
}