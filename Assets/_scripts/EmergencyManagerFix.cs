using UnityEngine;
using Starkiller.Core.Managers;
using Starkiller.Core;
using System.Reflection;
using System.Collections;

/// <summary>
/// Emergency fix script to create missing managers and fix UI references
/// This will ensure PersonalDataLog and DailyReport work properly
/// </summary>
public class EmergencyManagerFix : MonoBehaviour
{
    [Header("Emergency Fix Controls")]
    [SerializeField] private KeyCode emergencyFixKey = KeyCode.F1;
    [SerializeField] private bool autoFixOnStart = true;
    
    private void Start()
    {
        Debug.Log("[EmergencyManagerFix] ⚠️ Emergency Fix Ready. Press F1 to fix all managers.");
        
        if (autoFixOnStart)
        {
            StartCoroutine(AutoFixAfterDelay());
        }
    }
    
    private IEnumerator AutoFixAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        Debug.Log("[EmergencyManagerFix] Running automatic fix...");
        PerformEmergencyFix();
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(emergencyFixKey))
        {
            PerformEmergencyFix();
        }
        
        // Additional hotkeys
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ForceShowPersonalDataLog();
        }
    }
    
    public void PerformEmergencyFix()
    {
        Debug.Log("[EmergencyManagerFix] 🚨 STARTING EMERGENCY FIX SEQUENCE 🚨");
        
        // Step 1: Create missing PersonalDataLogManager
        CreatePersonalDataLogManager();
        
        // Step 2: Create missing RefactoredDailyReportManager
        CreateRefactoredDailyReportManager();
        
        // Step 3: Fix DayProgressionManager
        FixDayProgressionManager();
        
        // Step 4: Fix UI References
        FixUIReferences();
        
        // Step 5: Register all managers with ServiceLocator
        RegisterManagers();
        
        // Step 6: Run diagnostics
        RunDiagnostics();
        
        Debug.Log("[EmergencyManagerFix] ✅ EMERGENCY FIX COMPLETE!");
    }
    
    private void CreatePersonalDataLogManager()
    {
        var existing = FindFirstObjectByType<PersonalDataLogManager>();
        if (existing == null)
        {
            Debug.Log("[EmergencyManagerFix] Creating PersonalDataLogManager...");
            
            GameObject managerObj = new GameObject("PersonalDataLogManager");
            var manager = managerObj.AddComponent<PersonalDataLogManager>();
            DontDestroyOnLoad(managerObj);
            
            // Find and assign UI panel
            GameObject dataLogPanel = FindPersonalDataLogPanel();
            if (dataLogPanel != null)
            {
                SetPrivateField(manager, "dataLogPanel", dataLogPanel);
                Debug.Log($"[EmergencyManagerFix] ✅ PersonalDataLogManager created with panel: {dataLogPanel.name}");
                
                // Set up feed sections
                SetupPersonalDataLogSections(manager, dataLogPanel);
            }
            else
            {
                Debug.LogError("[EmergencyManagerFix] ❌ Could not find PersonalDataLogPanel!");
            }
            
            // Set up templates
            SetupPersonalDataLogTemplates(manager);
        }
        else
        {
            Debug.Log("[EmergencyManagerFix] PersonalDataLogManager already exists");
            
            // Ensure UI is connected
            var dataLogPanelField = GetPrivateField<GameObject>(existing, "dataLogPanel");
            if (dataLogPanelField == null)
            {
                GameObject dataLogPanel = FindPersonalDataLogPanel();
                if (dataLogPanel != null)
                {
                    SetPrivateField(existing, "dataLogPanel", dataLogPanel);
                    Debug.Log($"[EmergencyManagerFix] ✅ Fixed PersonalDataLogManager panel reference");
                    SetupPersonalDataLogSections(existing, dataLogPanel);
                }
            }
        }
    }
    
    private void CreateRefactoredDailyReportManager()
    {
        var existing = FindFirstObjectByType<RefactoredDailyReportManager>();
        if (existing == null)
        {
            Debug.Log("[EmergencyManagerFix] Creating RefactoredDailyReportManager...");
            
            GameObject managerObj = new GameObject("RefactoredDailyReportManager");
            var manager = managerObj.AddComponent<RefactoredDailyReportManager>();
            DontDestroyOnLoad(managerObj);
            
            // Find and assign daily report panel
            GameObject reportPanel = FindDailyReportPanel();
            if (reportPanel != null)
            {
                SetPrivateField(manager, "dailyReportPanel", reportPanel);
                Debug.Log($"[EmergencyManagerFix] ✅ RefactoredDailyReportManager created with panel: {reportPanel.name}");
            }
            
            Debug.Log("[EmergencyManagerFix] ✅ RefactoredDailyReportManager created");
        }
        else
        {
            Debug.Log("[EmergencyManagerFix] RefactoredDailyReportManager already exists");
            
            // Fix panel reference
            var panelField = GetPrivateField<GameObject>(existing, "dailyReportPanel");
            if (panelField == null)
            {
                GameObject reportPanel = FindDailyReportPanel();
                if (reportPanel != null)
                {
                    SetPrivateField(existing, "dailyReportPanel", reportPanel);
                    Debug.Log($"[EmergencyManagerFix] ✅ Fixed RefactoredDailyReportManager panel reference");
                }
            }
        }
    }
    
    private void FixDayProgressionManager()
    {
        var dayProgression = ServiceLocator.Get<DayProgressionManager>();
        if (dayProgression == null)
        {
            dayProgression = FindFirstObjectByType<DayProgressionManager>();
        }
        
        if (dayProgression != null)
        {
            // Reset any bad state
            var currentDayField = GetPrivateField<int>(dayProgression, "_currentDay");
            if (currentDayField < 1)
            {
                SetPrivateField(dayProgression, "_currentDay", 1);
                Debug.Log("[EmergencyManagerFix] ✅ Fixed DayProgressionManager current day");
            }
            
            // Clear transition flag
            SetPrivateField(dayProgression, "_isTransitioningDays", false);
            Debug.Log("[EmergencyManagerFix] ✅ Cleared day transition flag");
        }
    }
    
    private GameObject FindPersonalDataLogPanel()
    {
        // Try various names
        string[] possibleNames = {
            "PersonalDataLogPanel",
            "PersonalDataLog",
            "DataLogPanel",
            "DataLog",
            "PersonalLog",
            "HolographicInterface"
        };
        
        foreach (var name in possibleNames)
        {
            var found = GameObject.Find(name);
            if (found != null)
            {
                Debug.Log($"[EmergencyManagerFix] Found PersonalDataLog panel: {name}");
                return found;
            }
        }
        
        // Search all canvases
        var canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var canvas in canvases)
        {
            var panel = SearchInChildren(canvas.transform, "personal", "datalog", "holographic");
            if (panel != null) return panel.gameObject;
        }
        
        return null;
    }
    
    private GameObject FindDailyReportPanel()
    {
        string[] possibleNames = {
            "DailyReportPanel",
            "DailyReport",
            "Daily Report Panel",
            "DailyPerformanceReport",
            "EndOfDayPanel"
        };
        
        foreach (var name in possibleNames)
        {
            var found = GameObject.Find(name);
            if (found != null)
            {
                Debug.Log($"[EmergencyManagerFix] Found DailyReport panel: {name}");
                return found;
            }
        }
        
        return null;
    }
    
    private Transform SearchInChildren(Transform parent, params string[] keywords)
    {
        string parentNameLower = parent.name.ToLower();
        foreach (var keyword in keywords)
        {
            if (parentNameLower.Contains(keyword.ToLower()))
                return parent;
        }
        
        for (int i = 0; i < parent.childCount; i++)
        {
            var result = SearchInChildren(parent.GetChild(i), keywords);
            if (result != null) return result;
        }
        
        return null;
    }
    
    private void SetupPersonalDataLogSections(PersonalDataLogManager manager, GameObject panel)
    {
        // Find sections
        var imperiumSection = FindChildByName(panel.transform, "ImperiumNewsSection", "ImperiumNews", "Imperium");
        var familySection = FindChildByName(panel.transform, "FamilyChatSection", "FamilyChat", "Family");
        var frontierSection = FindChildByName(panel.transform, "FrontierEzineSection", "FrontierEzine", "Frontier");
        
        if (imperiumSection != null)
        {
            SetPrivateField(manager, "imperiumNewsSection", imperiumSection);
            Debug.Log("[EmergencyManagerFix] ✅ Set imperiumNewsSection");
        }
        
        if (familySection != null)
        {
            SetPrivateField(manager, "familyChatSection", familySection);
            Debug.Log("[EmergencyManagerFix] ✅ Set familyChatSection");
        }
        
        if (frontierSection != null)
        {
            SetPrivateField(manager, "frontierEzineSection", frontierSection);
            Debug.Log("[EmergencyManagerFix] ✅ Set frontierEzineSection");
        }
    }
    
    private void SetupPersonalDataLogTemplates(PersonalDataLogManager manager)
    {
        // Find templates in project
        var newsTemplate = FindPrefabByName("NewsEntryTemplate");
        var videoTemplate = FindPrefabByName("VideoEntryTemplate");
        
        if (newsTemplate != null)
        {
            SetPrivateField(manager, "newsEntryTemplate", newsTemplate);
            Debug.Log("[EmergencyManagerFix] ✅ Set newsEntryTemplate");
        }
        
        if (videoTemplate != null)
        {
            SetPrivateField(manager, "videoEntryTemplate", videoTemplate);
            SetPrivateField(manager, "familyActionTemplate", videoTemplate);
            Debug.Log("[EmergencyManagerFix] ✅ Set video/family templates");
        }
    }
    
    private GameObject FindPrefabByName(string name)
    {
        // Search in Resources
        var prefab = Resources.Load<GameObject>(name);
        if (prefab != null) return prefab;
        
        // Search all loaded objects
        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name.Contains(name) && !obj.scene.IsValid()) // Prefabs don't have valid scenes
                return obj;
        }
        
        return null;
    }
    
    private Transform FindChildByName(Transform parent, params string[] possibleNames)
    {
        foreach (var name in possibleNames)
        {
            var child = parent.Find(name);
            if (child != null) return child;
            
            // Recursive search
            var found = FindChildRecursive(parent, name);
            if (found != null) return found;
        }
        return null;
    }
    
    private Transform FindChildRecursive(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return child;
                
            var recursive = FindChildRecursive(child, name);
            if (recursive != null) return recursive;
        }
        return null;
    }
    
    private void FixUIReferences()
    {
        // Fix any other UI references that might be missing
        Debug.Log("[EmergencyManagerFix] Fixing additional UI references...");
        
        // Fix continue buttons, headers, etc.
        var personalDataLog = FindFirstObjectByType<PersonalDataLogManager>();
        if (personalDataLog != null)
        {
            var panel = GetPrivateField<GameObject>(personalDataLog, "dataLogPanel");
            if (panel != null)
            {
                // Find continue button
                var continueButton = panel.GetComponentInChildren<UnityEngine.UI.Button>();
                if (continueButton != null)
                {
                    SetPrivateField(personalDataLog, "continueButton", continueButton);
                    Debug.Log("[EmergencyManagerFix] ✅ Set continue button");
                }
            }
        }
    }
    
    private void RegisterManagers()
    {
        Debug.Log("[EmergencyManagerFix] Registering managers with ServiceLocator...");
        
        // Register PersonalDataLogManager
        var personalDataLog = FindFirstObjectByType<PersonalDataLogManager>();
        if (personalDataLog != null)
        {
            ServiceLocator.Register<PersonalDataLogManager>(personalDataLog);
            Debug.Log("[EmergencyManagerFix] ✅ Registered PersonalDataLogManager");
        }
        
        // Register RefactoredDailyReportManager
        var dailyReport = FindFirstObjectByType<RefactoredDailyReportManager>();
        if (dailyReport != null)
        {
            ServiceLocator.Register<RefactoredDailyReportManager>(dailyReport);
            Debug.Log("[EmergencyManagerFix] ✅ Registered RefactoredDailyReportManager");
        }
    }
    
    private void RunDiagnostics()
    {
        Debug.Log("[EmergencyManagerFix] === DIAGNOSTIC RESULTS ===");
        
        var personalDataLog = FindFirstObjectByType<PersonalDataLogManager>();
        Debug.Log($"PersonalDataLogManager: {(personalDataLog != null ? "✅ EXISTS" : "❌ MISSING")}");
        
        if (personalDataLog != null)
        {
            var panel = GetPrivateField<GameObject>(personalDataLog, "dataLogPanel");
            Debug.Log($"  - dataLogPanel: {(panel != null ? "✅ ASSIGNED" : "❌ NULL")}");
            
            var imperium = GetPrivateField<Transform>(personalDataLog, "imperiumNewsSection");
            Debug.Log($"  - imperiumNewsSection: {(imperium != null ? "✅ ASSIGNED" : "❌ NULL")}");
            
            var family = GetPrivateField<Transform>(personalDataLog, "familyChatSection");
            Debug.Log($"  - familyChatSection: {(family != null ? "✅ ASSIGNED" : "❌ NULL")}");
            
            var frontier = GetPrivateField<Transform>(personalDataLog, "frontierEzineSection");
            Debug.Log($"  - frontierEzineSection: {(frontier != null ? "✅ ASSIGNED" : "❌ NULL")}");
        }
        
        var dailyReport = FindFirstObjectByType<RefactoredDailyReportManager>();
        Debug.Log($"RefactoredDailyReportManager: {(dailyReport != null ? "✅ EXISTS" : "❌ MISSING")}");
        
        if (dailyReport != null)
        {
            var panel = GetPrivateField<GameObject>(dailyReport, "dailyReportPanel");
            Debug.Log($"  - dailyReportPanel: {(panel != null ? "✅ ASSIGNED" : "❌ NULL")}");
        }
        
        Debug.Log("[EmergencyManagerFix] === END DIAGNOSTIC ===");
    }
    
    private void ForceShowPersonalDataLog()
    {
        Debug.Log("[EmergencyManagerFix] Force showing PersonalDataLog...");
        
        var manager = FindFirstObjectByType<PersonalDataLogManager>();
        if (manager != null)
        {
            // Add test data
            manager.AddLogEntry(new DataLogEntry
            {
                feedType = FeedType.ImperiumNews,
                headline = "EMERGENCY TEST: System Operational",
                content = "PersonalDataLogManager has been repaired and is now functioning correctly.",
                timestamp = System.DateTime.Now,
                requiresAction = false
            });
            
            manager.AddLogEntry(new DataLogEntry
            {
                feedType = FeedType.FamilyChat,
                headline = "EMERGENCY TEST: Family Connection",
                content = "Family communication systems have been restored.",
                timestamp = System.DateTime.Now,
                requiresAction = false
            });
            
            manager.AddLogEntry(new DataLogEntry
            {
                feedType = FeedType.FrontierEzine,
                headline = "EMERGENCY TEST: Trade Update",
                content = "All systems are now operational. Trade data flowing normally.",
                timestamp = System.DateTime.Now,
                requiresAction = false
            });
            
            // Show it
            manager.ShowDataLog();
        }
        else
        {
            Debug.LogError("[EmergencyManagerFix] PersonalDataLogManager not found! Run F1 first!");
        }
    }
    
    // Reflection helpers
    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, 
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogWarning($"[EmergencyManagerFix] Could not find field '{fieldName}' in {target.GetType().Name}");
        }
    }
    
    private T GetPrivateField<T>(object target, string fieldName)
    {
        var field = target.GetType().GetField(fieldName, 
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        if (field != null)
        {
            return (T)field.GetValue(target);
        }
        return default(T);
    }
}