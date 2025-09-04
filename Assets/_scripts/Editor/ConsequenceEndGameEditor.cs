using UnityEngine;
using UnityEditor;
using StarkillerBaseCommand;
using System.Collections.Generic;

/// <summary>
/// Editor utility to help populate endGameMessage fields for Consequence ScriptableObjects
/// </summary>
public class ConsequenceEndGameEditor : EditorWindow
{
    private Vector2 scrollPosition;
    private List<Consequence> allConsequences = new List<Consequence>();
    private Dictionary<Consequence, string> suggestedMessages = new Dictionary<Consequence, string>();
    
    [MenuItem("Starkiller/Consequence EndGame Messages")]
    public static void ShowWindow()
    {
        var window = GetWindow<ConsequenceEndGameEditor>("Consequence EndGame Messages");
        window.minSize = new Vector2(600, 400);
        window.LoadConsequences();
    }
    
    private void LoadConsequences()
    {
        allConsequences.Clear();
        suggestedMessages.Clear();
        
        // Find all Consequence ScriptableObjects in the project
        string[] guids = AssetDatabase.FindAssets("t:Consequence");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Consequence consequence = AssetDatabase.LoadAssetAtPath<Consequence>(path);
            
            if (consequence != null)
            {
                allConsequences.Add(consequence);
                
                // Generate suggested message if none exists
                if (string.IsNullOrEmpty(consequence.endGameMessage))
                {
                    suggestedMessages[consequence] = GenerateSuggestedMessage(consequence);
                }
            }
        }
        
        Debug.Log($"Found {allConsequences.Count} Consequence ScriptableObjects");
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("Consequence EndGame Messages", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Refresh Consequences"))
        {
            LoadConsequences();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox($"Found {allConsequences.Count} Consequence ScriptableObjects. Edit the EndGame Messages below:", MessageType.Info);
        
        if (GUILayout.Button("Apply All Suggested Messages"))
        {
            ApplyAllSuggestedMessages();
        }
        
        EditorGUILayout.Space();
        
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        foreach (var consequence in allConsequences)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            // Title and severity
            EditorGUILayout.LabelField($"{consequence.title} ({consequence.severity})", EditorStyles.boldLabel);
            
            // Show type and other info
            EditorGUILayout.LabelField($"Type: {consequence.type} | Affects Family: {consequence.affectsFamily}");
            
            // Current endGameMessage
            EditorGUI.BeginChangeCheck();
            string newMessage = EditorGUILayout.TextArea(consequence.endGameMessage, GUILayout.MinHeight(40));
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(consequence, "Update EndGame Message");
                consequence.endGameMessage = newMessage;
                EditorUtility.SetDirty(consequence);
            }
            
            // Show suggested message if empty
            if (string.IsNullOrEmpty(consequence.endGameMessage) && suggestedMessages.ContainsKey(consequence))
            {
                EditorGUILayout.HelpBox($"Suggested: {suggestedMessages[consequence]}", MessageType.None);
                
                if (GUILayout.Button("Use Suggested", GUILayout.Width(100)))
                {
                    Undo.RecordObject(consequence, "Apply Suggested EndGame Message");
                    consequence.endGameMessage = suggestedMessages[consequence];
                    EditorUtility.SetDirty(consequence);
                }
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private string GenerateSuggestedMessage(Consequence consequence)
    {
        // Generate context-appropriate messages based on type and severity
        string message = "";
        
        switch (consequence.type)
        {
            case Consequence.ConsequenceType.Security:
                switch (consequence.severity)
                {
                    case Consequence.SeverityLevel.Critical:
                        message = $"Caused critical security failure resulting in {consequence.imperiumCasualties} casualties";
                        break;
                    case Consequence.SeverityLevel.Severe:
                        message = $"Security breach compromised checkpoint integrity";
                        break;
                    case Consequence.SeverityLevel.Moderate:
                        message = $"Violated security protocols during checkpoint duty";
                        break;
                    default:
                        message = $"Minor security lapse recorded in service record";
                        break;
                }
                break;
                
            case Consequence.ConsequenceType.Family:
                if (consequence.title == "Family Reassignment")
                {
                    message = "Family relocated to monitoring quarters after security concerns";
                }
                else if (consequence.affectsFamily)
                {
                    message = consequence.severity >= Consequence.SeverityLevel.Severe ?
                        "Decisions severely impacted family security status" :
                        "Family affected by checkpoint decisions";
                }
                else
                {
                    message = "Family welfare compromised by professional choices";
                }
                break;
                
            case Consequence.ConsequenceType.Financial:
                if (consequence.creditPenalty > 30)
                {
                    message = $"Incurred major financial penalties totaling {consequence.creditPenalty} credits";
                }
                else
                {
                    message = $"Financial infractions resulted in credit penalties";
                }
                break;
                
            case Consequence.ConsequenceType.Reputation:
                message = consequence.severity >= Consequence.SeverityLevel.Severe ?
                    "Professional reputation severely damaged by failures" :
                    "Service record tarnished by questionable decisions";
                break;
                
            case Consequence.ConsequenceType.Story:
                message = $"Made pivotal story decision: {consequence.title}";
                break;
                
            case Consequence.ConsequenceType.Special:
                message = $"Triggered special event: {consequence.title}";
                break;
                
            default:
                message = $"Consequence recorded: {consequence.title}";
                break;
        }
        
        // Add loyalty context if significant
        if (consequence.imperialLoyaltyChange <= -2)
        {
            message += " (undermined Imperial authority)";
        }
        else if (consequence.rebellionSympathyChange >= 2)
        {
            message += " (increased rebel sympathies)";
        }
        
        return message;
    }
    
    private void ApplyAllSuggestedMessages()
    {
        int applied = 0;
        
        foreach (var consequence in allConsequences)
        {
            if (string.IsNullOrEmpty(consequence.endGameMessage) && suggestedMessages.ContainsKey(consequence))
            {
                Undo.RecordObject(consequence, "Apply All Suggested EndGame Messages");
                consequence.endGameMessage = suggestedMessages[consequence];
                EditorUtility.SetDirty(consequence);
                applied++;
            }
        }
        
        Debug.Log($"Applied {applied} suggested messages");
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Starkiller/Quick Actions/List Consequences Without EndGame Messages")]
    public static void ListMissingMessages()
    {
        string[] guids = AssetDatabase.FindAssets("t:Consequence");
        int missing = 0;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Consequence consequence = AssetDatabase.LoadAssetAtPath<Consequence>(path);
            
            if (consequence != null && string.IsNullOrEmpty(consequence.endGameMessage))
            {
                Debug.LogWarning($"Missing EndGame Message: {consequence.title} at {path}", consequence);
                missing++;
            }
        }
        
        Debug.Log($"Total: {missing} consequences missing EndGame messages out of {guids.Length}");
    }
}