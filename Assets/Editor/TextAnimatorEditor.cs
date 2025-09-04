#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextAnimator))]
public class TextAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw default inspector
        DrawDefaultInspector();
        
        TextAnimator animator = (TextAnimator)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Testing", EditorStyles.boldLabel);
        
        // Add buttons to test animations
        if (GUILayout.Button("Preview Animation"))
        {
            animator.Animate();
        }
        
        if (GUILayout.Button("Stop Animation"))
        {
            animator.StopAnimation();
        }
        
        // Add number counter test (only if animation type is NumberCounter)
        if (animator.animationType == TextAnimator.AnimationType.NumberCounter)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Number Counter Test", EditorStyles.boldLabel);
            
            float testNumber = EditorGUILayout.FloatField("Test Number", 100f);
            string format = EditorGUILayout.TextField("Number Format", "0");
            
            if (GUILayout.Button("Test Number Animation"))
            {
                animator.AnimateNumber(testNumber, format);
            }
        }
    }
}
#endif