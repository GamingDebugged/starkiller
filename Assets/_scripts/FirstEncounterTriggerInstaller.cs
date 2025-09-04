using UnityEngine;

/// <summary>
/// Quick patch to ensure FirstEncounterTrigger is added to the scene
/// Add this to any GameObject in your scene
/// </summary>
public class FirstEncounterTriggerInstaller : MonoBehaviour
{
    void Awake()
    {
        // Check if FirstEncounterTrigger exists
        FirstEncounterTrigger trigger = FindFirstObjectByType<FirstEncounterTrigger>();
        
        if (trigger == null)
        {
            // Create a new GameObject with the trigger
            GameObject triggerObject = new GameObject("FirstEncounterTrigger");
            trigger = triggerObject.AddComponent<FirstEncounterTrigger>();
            
            // Make it persist across scenes
            DontDestroyOnLoad(triggerObject);
            
            Debug.Log("FirstEncounterTriggerInstaller: Created FirstEncounterTrigger");
        }
        else
        {
            Debug.Log("FirstEncounterTriggerInstaller: FirstEncounterTrigger already exists");
        }
    }
}