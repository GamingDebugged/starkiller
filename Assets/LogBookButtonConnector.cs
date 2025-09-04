using UnityEngine;
using UnityEngine.UI;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Simple connector script to link UI buttons to the StarkkillerLogBookManager.
    /// Add this to any GameObject with a Button that should open/close the logbook.
    /// </summary>
    public class LogBookButtonConnector : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("Reference to the Starkiller Logbook Manager")]
        public StarkkillerLogBookManager logBookManager;
        
        [Tooltip("Button that should trigger the logbook")]
        public Button button;
        
        [Header("Behavior")]
        [Tooltip("Should this button toggle, open, or close the logbook?")]
        public ActionType actionType = ActionType.Toggle;
        
        [Tooltip("Tab to switch to when opening the logbook (if applicable)")]
        public TabType targetTab = TabType.Rules;
        
        /// <summary>
        /// The action this button should perform
        /// </summary>
        public enum ActionType
        {
            Toggle,
            Open,
            Close
        }
        
        /// <summary>
        /// Available tabs in the logbook
        /// </summary>
        public enum TabType
        {
            Rules,
            Ships,
            Captains,
            Destinations,
            Contraband
        }
        
        void Start()
        {
            // Find references if not assigned
            if (logBookManager == null)
                logBookManager = FindFirstObjectByType<StarkkillerLogBookManager>();
                
            if (button == null)
                button = GetComponent<Button>();
                
            if (logBookManager == null)
            {
                Debug.LogError("LogBookButtonConnector: No StarkkillerLogBookManager found in scene!");
                return;
            }
            
            if (button == null)
            {
                Debug.LogError("LogBookButtonConnector: No Button component found!");
                return;
            }
            
            // Add button listener based on action type
            button.onClick.AddListener(OnButtonClick);
        }
        
        /// <summary>
        /// Called when the button is clicked
        /// </summary>
        public void OnButtonClick()
        {
            if (logBookManager == null) return;
            
            switch (actionType)
            {
                case ActionType.Toggle:
                    logBookManager.ToggleLogBook();
                    break;
                    
                case ActionType.Open:
                    logBookManager.OpenLogBook();
                    break;
                    
                case ActionType.Close:
                    logBookManager.CloseLogBook();
                    break;
            }
        }
    }
}