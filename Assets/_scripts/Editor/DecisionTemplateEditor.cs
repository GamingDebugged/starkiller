using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using StarkillerBaseCommand.Narrative;

namespace StarkillerBaseCommand.EditorTools
{
    public class DecisionTemplateEditor : EditorWindow
    {
        private Vector2 scrollPos;
        private List<DecisionTemplate> templates = new List<DecisionTemplate>();
        private DecisionTemplate selectedTemplate;
        private bool showCreateSection = false;
        private string newTemplateName = "NewDecisionTemplate";
        
        [MenuItem("Starkiller/Narrative/Decision Template Manager")]
        public static void ShowWindow()
        {
            GetWindow<DecisionTemplateEditor>("Decision Templates");
        }
        
        private void OnEnable()
        {
            RefreshTemplateList();
        }
        
        private void RefreshTemplateList()
        {
            templates.Clear();
            
            // Find all DecisionTemplate assets in the project
            string[] guids = AssetDatabase.FindAssets("t:DecisionTemplate");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DecisionTemplate template = AssetDatabase.LoadAssetAtPath<DecisionTemplate>(path);
                if (template != null)
                {
                    templates.Add(template);
                }
            }
        }
        
        private void OnGUI()
        {
            // Main UI code here...
        }
        
        // Quick create methods here...
    }
}