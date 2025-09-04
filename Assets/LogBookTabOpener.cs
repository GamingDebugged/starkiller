using UnityEngine;
using UnityEngine.UI;
using StarkillerBaseCommand;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LogBookTabOpener : MonoBehaviour
{
    [Tooltip("Reference to the LogBook manager")]
    public StarkkillerLogBookManager logBookManager;
    
    [Tooltip("Which tab to open")]
    public StarkkillerLogBookManager.TabType tabToOpen = StarkkillerLogBookManager.TabType.Rules;
    
    private Button button;
    
    void Start()
    {
        // Find references if not assigned
        if (logBookManager == null)
            logBookManager = FindFirstObjectByType<StarkkillerLogBookManager>();
            
        // Get the button component and add a listener
        button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OpenLogBookTab);
    }
    
    public void OpenLogBookTab()
    {
        if (logBookManager != null)
        {
            logBookManager.OpenLogBook();
            logBookManager.SwitchTab(tabToOpen);
        }
        else
        {
            Debug.LogWarning("LogBookTabOpener: Cannot open LogBook - manager reference is missing!");
        }
    }
}