using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using StarkillerBaseCommand;

namespace StarkillerBaseCommand
{
    /// <summary>
    /// Emergency helper script to fix buttons during runtime if they're not working
    /// Attach this to a GameObject in the scene for emergency recovery
    /// </summary>
    public class ButtonFixHelper : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CredentialChecker credentialChecker;
        [SerializeField] private MasterShipGenerator shipGenerator;
        
        [Header("Buttons")]
        [SerializeField] private Button approveButton;
        [SerializeField] private Button denyButton;
        [SerializeField] private Button holdingPatternButton;
        [SerializeField] private Button tractorBeamButton;
        [SerializeField] private Button acceptBribeButton;
        
        [Header("Settings")]
        [SerializeField] private bool findButtonsAutomatically = true;
        [SerializeField] private bool addListenersOnStart = true;
        
        void Start()
        {
            if (findButtonsAutomatically)
            {
                FindReferences();
            }
            
            if (addListenersOnStart)
            {
                AddButtonListeners();
            }
        }
        
        /// <summary>
        /// Find references to needed components
        /// </summary>
        public void FindReferences()
        {
            // Find credential checker
            if (credentialChecker == null)
                credentialChecker = FindFirstObjectByType<CredentialChecker>();
                
            // Find ship generator
            if (shipGenerator == null)
                shipGenerator = FindFirstObjectByType<MasterShipGenerator>();
                
            // Find buttons if not assigned
            if (approveButton == null)
                approveButton = GameObject.Find("ApproveButton")?.GetComponent<Button>();
                
            if (denyButton == null)
                denyButton = GameObject.Find("DenyButton")?.GetComponent<Button>();
                
            if (holdingPatternButton == null)
                holdingPatternButton = GameObject.Find("HoldingPatternButton")?.GetComponent<Button>();
                
            if (tractorBeamButton == null)
                tractorBeamButton = GameObject.Find("TractorBeamButton")?.GetComponent<Button>();
                
            if (acceptBribeButton == null)
                acceptBribeButton = GameObject.Find("AcceptBribeButton")?.GetComponent<Button>();
                
            LogFoundReferences();
        }
        
        /// <summary>
        /// Log found references
        /// </summary>
        private void LogFoundReferences()
        {
            Debug.Log("ButtonFixHelper found references:");
            Debug.Log($"CredentialChecker: {credentialChecker != null}");
            Debug.Log($"MasterShipGenerator: {shipGenerator != null}");
            Debug.Log($"ApproveButton: {approveButton != null}");
            Debug.Log($"DenyButton: {denyButton != null}");
            Debug.Log($"HoldingPatternButton: {holdingPatternButton != null}");
            Debug.Log($"TractorBeamButton: {tractorBeamButton != null}");
            Debug.Log($"AcceptBribeButton: {acceptBribeButton != null}");
        }
        
        /// <summary>
        /// Add listeners to buttons
        /// </summary>
        public void AddButtonListeners()
        {
            if (credentialChecker == null)
            {
                Debug.LogError("ButtonFixHelper: Cannot add listeners - CredentialChecker is null");
                return;
            }
            
            // First remove any existing listeners to avoid duplicates
            if (approveButton != null)
            {
                approveButton.onClick.RemoveAllListeners();
                approveButton.onClick.AddListener(OnApproveClicked);
                Debug.Log("Added listener to ApproveButton");
            }
            
            if (denyButton != null)
            {
                denyButton.onClick.RemoveAllListeners();
                denyButton.onClick.AddListener(OnDenyClicked);
                Debug.Log("Added listener to DenyButton");
            }
            
            if (holdingPatternButton != null)
            {
                holdingPatternButton.onClick.RemoveAllListeners();
                holdingPatternButton.onClick.AddListener(OnHoldingPatternClicked);
                Debug.Log("Added listener to HoldingPatternButton");
            }
            
            if (tractorBeamButton != null)
            {
                tractorBeamButton.onClick.RemoveAllListeners();
                tractorBeamButton.onClick.AddListener(OnTractorBeamClicked);
                Debug.Log("Added listener to TractorBeamButton");
            }
            
            if (acceptBribeButton != null)
            {
                acceptBribeButton.onClick.RemoveAllListeners();
                acceptBribeButton.onClick.AddListener(OnAcceptBribeClicked);
                Debug.Log("Added listener to AcceptBribeButton");
            }
        }
        
        /// <summary>
        /// Handle approve button click
        /// </summary>
        public void OnApproveClicked()
        {
            Debug.Log("ButtonFixHelper: Approve button clicked");
            if (credentialChecker != null)
            {
                credentialChecker.OnApproveClicked();
            }
            else
            {
                Debug.LogError("ButtonFixHelper: Cannot handle approve - CredentialChecker is null");
            }
        }
        
        /// <summary>
        /// Handle deny button click
        /// </summary>
        public void OnDenyClicked()
        {
            Debug.Log("ButtonFixHelper: Deny button clicked");
            if (credentialChecker != null)
            {
                credentialChecker.OnDenyClicked();
            }
            else
            {
                Debug.LogError("ButtonFixHelper: Cannot handle deny - CredentialChecker is null");
            }
        }
        
        /// <summary>
        /// Handle holding pattern button click
        /// </summary>
        public void OnHoldingPatternClicked()
        {
            Debug.Log("ButtonFixHelper: Holding pattern button clicked");
            if (credentialChecker != null)
            {
                // Use reflection to call the method since it might be private
                var method = credentialChecker.GetType().GetMethod("OnHoldingPatternClicked", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    
                if (method != null)
                {
                    method.Invoke(credentialChecker, null);
                }
                else
                {
                    // Direct processing if method not found
                    Debug.LogWarning("ButtonFixHelper: OnHoldingPatternClicked method not found, trying direct processing");
                    DirectHoldingPatternProcess();
                }
            }
            else
            {
                Debug.LogError("ButtonFixHelper: Cannot handle holding pattern - CredentialChecker is null");
            }
        }
        
        /// <summary>
        /// Handle tractor beam button click
        /// </summary>
        public void OnTractorBeamClicked()
        {
            Debug.Log("ButtonFixHelper: Tractor beam button clicked");
            if (credentialChecker != null)
            {
                // Use reflection to call the method since it might be private
                var method = credentialChecker.GetType().GetMethod("OnTractorBeamClicked", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    
                if (method != null)
                {
                    method.Invoke(credentialChecker, null);
                }
                else
                {
                    // Direct processing if method not found
                    Debug.LogWarning("ButtonFixHelper: OnTractorBeamClicked method not found, trying direct processing");
                    DirectTractorBeamProcess();
                }
            }
            else
            {
                Debug.LogError("ButtonFixHelper: Cannot handle tractor beam - CredentialChecker is null");
            }
        }
        
        /// <summary>
        /// Handle accept bribe button click
        /// </summary>
        public void OnAcceptBribeClicked()
        {
            Debug.Log("ButtonFixHelper: Accept bribe button clicked");
            if (credentialChecker != null)
            {
                // Use reflection to call the method since it might be private
                var method = credentialChecker.GetType().GetMethod("OnAcceptBribeClicked", 
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
                    
                if (method != null)
                {
                    method.Invoke(credentialChecker, null);
                }
                else
                {
                    // Direct processing if method not found
                    Debug.LogWarning("ButtonFixHelper: OnAcceptBribeClicked method not found, trying direct processing");
                    DirectBribeProcess();
                }
            }
            else
            {
                Debug.LogError("ButtonFixHelper: Cannot handle accept bribe - CredentialChecker is null");
            }
        }
        
        /// <summary>
        /// Direct processing for holding pattern if the method can't be found
        /// </summary>
        private void DirectHoldingPatternProcess()
        {
            if (shipGenerator == null)
            {
                Debug.LogError("ButtonFixHelper: Cannot process holding pattern - MasterShipGenerator is null");
                return;
            }
            
            // Try to find holding pattern processor
            HoldingPatternProcessor processor = FindFirstObjectByType<HoldingPatternProcessor>();
            if (processor != null)
            {
                Debug.Log("ButtonFixHelper: Using HoldingPatternProcessor directly");
                // processor.StartHoldingPattern();
            }
            else
            {
                Debug.LogError("ButtonFixHelper: HoldingPatternProcessor not found");
            }
        }
        
        /// <summary>
        /// Direct processing for tractor beam if the method can't be found
        /// </summary>
        private void DirectTractorBeamProcess()
        {
            if (shipGenerator == null)
            {
                Debug.LogError("ButtonFixHelper: Cannot process tractor beam - MasterShipGenerator is null");
                return;
            }
            
            Debug.Log("ButtonFixHelper: Direct processing of tractor beam not implemented");
        }
        
        /// <summary>
        /// Direct processing for bribe if the method can't be found
        /// </summary>
        private void DirectBribeProcess()
        {
            if (shipGenerator == null)
            {
                Debug.LogError("ButtonFixHelper: Cannot process bribe - MasterShipGenerator is null");
                return;
            }
            
            Debug.Log("ButtonFixHelper: Direct processing of bribe not implemented");
        }
    }
}