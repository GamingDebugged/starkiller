using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

namespace Starkiller.Core.Managers
{
    /// <summary>
    /// Manages save/load operations and player preferences
    /// Extracted from GameManager for focused save system responsibility
    /// </summary>
    public class SaveGameManager : MonoBehaviour
    {
        [Header("Save Settings")]
        [SerializeField] private bool enableAutoSave = true;
        [SerializeField] private float autoSaveInterval = 300f; // 5 minutes
        [SerializeField] private int maxSaveSlots = 10;
        [SerializeField] private bool compressSaves = true;
        
        [Header("Save Locations")]
        [SerializeField] private string saveDirectoryName = "SaveData";
        [SerializeField] private string saveFileExtension = ".sav";
        [SerializeField] private string preferencesFileName = "preferences.json";
        
        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;
        [SerializeField] private bool enableSaveValidation = true;
        
        // Save system state
        private string _saveDirectory;
        private float _lastAutoSaveTime;
        private bool _isSaving = false;
        private bool _isLoading = false;
        private GameSaveData _currentSaveData;
        
        // Manager references for save data collection
        private CreditsManager _creditsManager;
        private DecisionTracker _decisionTracker;
        private DayProgressionManager _dayManager;
        private GameStateManager _gameStateManager;
        
        // Player preferences
        private PlayerPreferences _preferences;
        
        // Events
        public static event Action<string> OnSaveStarted;
        public static event Action<string, bool> OnSaveCompleted; // (fileName, success)
        public static event Action<string> OnLoadStarted;
        public static event Action<string, bool> OnLoadCompleted; // (fileName, success)
        public static event Action<PlayerPreferences> OnPreferencesChanged;
        
        // Public properties
        public bool IsSaving => _isSaving;
        public bool IsLoading => _isLoading;
        public bool AutoSaveEnabled => enableAutoSave;
        public PlayerPreferences Preferences => _preferences;
        public string SaveDirectory => _saveDirectory;
        
        private void Awake()
        {
            // Register with service locator
            ServiceLocator.Register<SaveGameManager>(this);
            
            // Initialize save directory
            InitializeSaveDirectory();
            
            if (enableDebugLogs)
                Debug.Log("[SaveGameManager] Initialized and registered with ServiceLocator");
        }
        
        private void Start()
        {
            // Get manager references
            _creditsManager = ServiceLocator.Get<CreditsManager>();
            _decisionTracker = ServiceLocator.Get<DecisionTracker>();
            _dayManager = ServiceLocator.Get<DayProgressionManager>();
            _gameStateManager = ServiceLocator.Get<GameStateManager>();
            
            // Subscribe to events
            GameEvents.OnSaveRequested += () => SaveGame();
            GameEvents.OnLoadRequested += () => LoadGame();
            
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayEnded += OnDayEnded;
            }
            
            // Load preferences
            LoadPreferences();
            
            // Initialize auto-save timer
            _lastAutoSaveTime = Time.time;
            
            if (enableDebugLogs)
                Debug.Log($"[SaveGameManager] Save system ready. Directory: {_saveDirectory}");
        }
        
        private void Update()
        {
            // Handle auto-save
            if (enableAutoSave && !_isSaving && !_isLoading)
            {
                if (Time.time - _lastAutoSaveTime >= autoSaveInterval)
                {
                    AutoSave();
                }
            }
        }
        
        /// <summary>
        /// Initialize the save directory
        /// </summary>
        private void InitializeSaveDirectory()
        {
            _saveDirectory = Path.Combine(Application.persistentDataPath, saveDirectoryName);
            
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
                
                if (enableDebugLogs)
                    Debug.Log($"[SaveGameManager] Created save directory: {_saveDirectory}");
            }
        }
        
        /// <summary>
        /// Save the current game state
        /// </summary>
        public void SaveGame(string fileName = "")
        {
            if (_isSaving)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[SaveGameManager] Save already in progress");
                return;
            }
            
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"quicksave_{DateTime.Now:yyyyMMdd_HHmmss}";
            }
            
            StartCoroutine(SaveGameCoroutine(fileName));
        }
        
        /// <summary>
        /// Auto-save functionality
        /// </summary>
        private void AutoSave()
        {
            string fileName = $"autosave_{DateTime.Now:yyyyMMdd_HHmmss}";
            SaveGame(fileName);
            _lastAutoSaveTime = Time.time;
            
            // Clean up old auto-saves
            CleanupOldAutoSaves();
        }
        
        /// <summary>
        /// Save game coroutine
        /// </summary>
        private System.Collections.IEnumerator SaveGameCoroutine(string fileName)
        {
            _isSaving = true;
            bool success = false;
            
            if (enableDebugLogs)
                Debug.Log($"[SaveGameManager] Starting save: {fileName}");
            
            OnSaveStarted?.Invoke(fileName);
            GameEvents.TriggerSaveRequested();
            
            // Collect save data from all managers
            _currentSaveData = CollectSaveData();
            
            // Add metadata
            _currentSaveData.SaveTime = DateTime.Now;
            _currentSaveData.GameVersion = Application.version;
            _currentSaveData.FileName = fileName;
            
            yield return null; // Allow frame update
            
            try
            {
                // Serialize and save
                string json = JsonUtility.ToJson(_currentSaveData, true);
                string filePath = Path.Combine(_saveDirectory, fileName + saveFileExtension);
                
                File.WriteAllText(filePath, json);
                
                if (enableSaveValidation)
                {
                    // Validate the save by trying to read it back
                    string testRead = File.ReadAllText(filePath);
                    GameSaveData testData = JsonUtility.FromJson<GameSaveData>(testRead);
                    
                    if (testData != null && testData.GameVersion == Application.version)
                    {
                        success = true;
                    }
                }
                else
                {
                    success = true;
                }
                
                if (enableDebugLogs)
                {
                    if (success)
                        Debug.Log($"[SaveGameManager] Save completed successfully: {fileName}");
                    else
                        Debug.LogError($"[SaveGameManager] Save validation failed: {fileName}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveGameManager] Save failed: {e.Message}");
                success = false;
            }
            
            _isSaving = false;
            OnSaveCompleted?.Invoke(fileName, success);
            GameEvents.TriggerSaveCompleted();
        }
        
        /// <summary>
        /// Load a saved game
        /// </summary>
        public void LoadGame(string fileName = "")
        {
            if (_isLoading)
            {
                if (enableDebugLogs)
                    Debug.LogWarning("[SaveGameManager] Load already in progress");
                return;
            }
            
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = GetMostRecentSave();
                if (string.IsNullOrEmpty(fileName))
                {
                    Debug.LogWarning("[SaveGameManager] No save files found");
                    return;
                }
            }
            
            StartCoroutine(LoadGameCoroutine(fileName));
        }
        
        /// <summary>
        /// Load game coroutine
        /// </summary>
        private System.Collections.IEnumerator LoadGameCoroutine(string fileName)
        {
            _isLoading = true;
            bool success = false;
            GameSaveData saveData = null;
            
            if (enableDebugLogs)
                Debug.Log($"[SaveGameManager] Starting load: {fileName}");
            
            OnLoadStarted?.Invoke(fileName);
            GameEvents.TriggerLoadRequested();
            
            try
            {
                string filePath = Path.Combine(_saveDirectory, fileName + saveFileExtension);
                
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"[SaveGameManager] Save file not found: {filePath}");
                    _isLoading = false;
                    OnLoadCompleted?.Invoke(fileName, false);
                    GameEvents.TriggerLoadCompleted();
                    yield break;
                }
                
                string json = File.ReadAllText(filePath);
                saveData = JsonUtility.FromJson<GameSaveData>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveGameManager] Load failed: {e.Message}");
                success = false;
            }
            
            yield return null; // Allow frame update
            
            if (saveData != null)
            {
                try
                {
                    // Apply save data to managers
                    ApplySaveData(saveData);
                    success = true;
                    
                    if (enableDebugLogs)
                        Debug.Log($"[SaveGameManager] Load completed successfully: {fileName}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[SaveGameManager] Failed to apply save data: {e.Message}");
                    success = false;
                }
            }
            else
            {
                Debug.LogError($"[SaveGameManager] Failed to deserialize save data: {fileName}");
            }
            
            _isLoading = false;
            OnLoadCompleted?.Invoke(fileName, success);
            GameEvents.TriggerLoadCompleted();
        }
        
        /// <summary>
        /// Collect save data from all managers
        /// </summary>
        private GameSaveData CollectSaveData()
        {
            var saveData = new GameSaveData();
            
            // Collect from CreditsManager
            if (_creditsManager != null)
            {
                saveData.Credits = _creditsManager.CurrentCredits;
            }
            
            // Collect from DecisionTracker
            if (_decisionTracker != null)
            {
                saveData.CorrectDecisions = _decisionTracker.CorrectDecisions;
                saveData.WrongDecisions = _decisionTracker.WrongDecisions;
                saveData.CurrentStrikes = _decisionTracker.CurrentStrikes;
            }
            
            // Collect from DayProgressionManager
            if (_dayManager != null)
            {
                saveData.CurrentDay = _dayManager.CurrentDay;
                saveData.ShipsProcessedToday = _dayManager.ShipsProcessedToday;
                saveData.RemainingTime = _dayManager.RemainingTime;
                saveData.IsShiftActive = _dayManager.IsShiftActive;
            }
            
            // Collect from GameStateManager
            if (_gameStateManager != null)
            {
                saveData.CurrentGameState = _gameStateManager.CurrentState.ToString();
            }
            
            return saveData;
        }
        
        /// <summary>
        /// Apply save data to all managers
        /// </summary>
        private void ApplySaveData(GameSaveData saveData)
        {
            // Apply to CreditsManager
            if (_creditsManager != null)
            {
                _creditsManager.SetCredits(saveData.Credits);
            }
            
            // Apply to DecisionTracker
            if (_decisionTracker != null)
            {
                _decisionTracker.SetDecisionCounts(saveData.CorrectDecisions, saveData.WrongDecisions, saveData.CurrentStrikes);
            }
            
            // Apply to DayProgressionManager
            if (_dayManager != null)
            {
                _dayManager.SetDayValues(saveData.CurrentDay, saveData.ShipsProcessedToday, saveData.RemainingTime);
            }
            
            // Apply to GameStateManager
            if (_gameStateManager != null && Enum.TryParse<GameState>(saveData.CurrentGameState, out GameState state))
            {
                _gameStateManager.ChangeState(state);
            }
        }
        
        /// <summary>
        /// Get list of all save files
        /// </summary>
        public List<SaveFileInfo> GetSaveFiles()
        {
            var saveFiles = new List<SaveFileInfo>();
            
            if (!Directory.Exists(_saveDirectory))
                return saveFiles;
            
            string[] files = Directory.GetFiles(_saveDirectory, "*" + saveFileExtension);
            
            foreach (string file in files)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(file);
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    
                    var saveFileInfo = new SaveFileInfo
                    {
                        FileName = fileName,
                        FilePath = file,
                        CreatedTime = fileInfo.CreationTime,
                        ModifiedTime = fileInfo.LastWriteTime,
                        FileSize = fileInfo.Length
                    };
                    
                    // Try to read basic info from save
                    try
                    {
                        string json = File.ReadAllText(file);
                        GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
                        if (saveData != null)
                        {
                            saveFileInfo.SaveTime = saveData.SaveTime;
                            saveFileInfo.GameVersion = saveData.GameVersion;
                            saveFileInfo.CurrentDay = saveData.CurrentDay;
                        }
                    }
                    catch
                    {
                        // If we can't read the save data, just use file info
                    }
                    
                    saveFiles.Add(saveFileInfo);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SaveGameManager] Error reading save file {file}: {e.Message}");
                }
            }
            
            // Sort by creation time (newest first)
            saveFiles.Sort((a, b) => b.CreatedTime.CompareTo(a.CreatedTime));
            
            return saveFiles;
        }
        
        /// <summary>
        /// Get the most recent save file
        /// </summary>
        public string GetMostRecentSave()
        {
            var saves = GetSaveFiles();
            return saves.Count > 0 ? saves[0].FileName : "";
        }
        
        /// <summary>
        /// Delete a save file
        /// </summary>
        public bool DeleteSave(string fileName)
        {
            try
            {
                string filePath = Path.Combine(_saveDirectory, fileName + saveFileExtension);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    
                    if (enableDebugLogs)
                        Debug.Log($"[SaveGameManager] Deleted save: {fileName}");
                    
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveGameManager] Failed to delete save {fileName}: {e.Message}");
            }
            
            return false;
        }
        
        /// <summary>
        /// Clean up old auto-saves
        /// </summary>
        private void CleanupOldAutoSaves()
        {
            var saves = GetSaveFiles();
            var autoSaves = saves.FindAll(s => s.FileName.StartsWith("autosave_"));
            
            // Keep only the 5 most recent auto-saves
            if (autoSaves.Count > 5)
            {
                for (int i = 5; i < autoSaves.Count; i++)
                {
                    DeleteSave(autoSaves[i].FileName);
                }
            }
        }
        
        /// <summary>
        /// Save player preferences
        /// </summary>
        public void SavePreferences()
        {
            try
            {
                string json = JsonUtility.ToJson(_preferences, true);
                string filePath = Path.Combine(_saveDirectory, preferencesFileName);
                File.WriteAllText(filePath, json);
                
                if (enableDebugLogs)
                    Debug.Log("[SaveGameManager] Preferences saved");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveGameManager] Failed to save preferences: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load player preferences
        /// </summary>
        public void LoadPreferences()
        {
            try
            {
                string filePath = Path.Combine(_saveDirectory, preferencesFileName);
                
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    _preferences = JsonUtility.FromJson<PlayerPreferences>(json);
                    
                    if (enableDebugLogs)
                        Debug.Log("[SaveGameManager] Preferences loaded");
                }
                else
                {
                    // Create default preferences
                    _preferences = new PlayerPreferences();
                    SavePreferences();
                    
                    if (enableDebugLogs)
                        Debug.Log("[SaveGameManager] Created default preferences");
                }
                
                OnPreferencesChanged?.Invoke(_preferences);
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveGameManager] Failed to load preferences: {e.Message}");
                _preferences = new PlayerPreferences();
            }
        }
        
        /// <summary>
        /// Update a preference setting
        /// </summary>
        public void SetPreference<T>(string key, T value)
        {
            if (_preferences == null)
                _preferences = new PlayerPreferences();
            
            _preferences.SetValue(key, value);
            OnPreferencesChanged?.Invoke(_preferences);
            
            if (enableDebugLogs)
                Debug.Log($"[SaveGameManager] Preference updated: {key} = {value}");
        }
        
        /// <summary>
        /// Get a preference setting
        /// </summary>
        public T GetPreference<T>(string key, T defaultValue = default(T))
        {
            if (_preferences == null)
                return defaultValue;
            
            return _preferences.GetValue(key, defaultValue);
        }
        
        // Event handlers
        private void OnDayEnded(int day)
        {
            if (enableAutoSave)
            {
                // Auto-save at end of each day
                string fileName = $"day_{day}_end";
                SaveGame(fileName);
            }
        }
        
        private void OnDestroy()
        {
            // Save preferences on exit
            SavePreferences();
            
            // Unsubscribe from events
            GameEvents.OnSaveRequested -= () => SaveGame();
            GameEvents.OnLoadRequested -= () => LoadGame();
            
            if (_dayManager != null)
            {
                DayProgressionManager.OnDayEnded -= OnDayEnded;
            }
            
            // Clear event subscriptions
            OnSaveStarted = null;
            OnSaveCompleted = null;
            OnLoadStarted = null;
            OnLoadCompleted = null;
            OnPreferencesChanged = null;
        }
        
        // Debug methods for testing
        [ContextMenu("Test: Quick Save")]
        private void TestQuickSave() => SaveGame("test_save");
        
        [ContextMenu("Test: Load Recent Save")]
        private void TestLoadRecentSave() => LoadGame();
        
        [ContextMenu("Test: Auto Save")]
        private void TestAutoSave() => AutoSave();
        
        [ContextMenu("Show Save Files")]
        private void ShowSaveFiles()
        {
            var saves = GetSaveFiles();
            Debug.Log($"=== SAVE FILES ({saves.Count}) ===");
            foreach (var save in saves)
            {
                Debug.Log($"{save.FileName} - Day {save.CurrentDay} - {save.CreatedTime}");
            }
            Debug.Log("=== END SAVE FILES ===");
        }
        
        [ContextMenu("Show Preferences")]
        private void ShowPreferences()
        {
            if (_preferences != null)
            {
                Debug.Log($"=== PREFERENCES ===");
                Debug.Log($"Audio Volume: {_preferences.audioVolume}");
                Debug.Log($"Graphics Quality: {_preferences.graphicsQuality}");
                Debug.Log($"Auto Save: {_preferences.autoSaveEnabled}");
                Debug.Log("=== END PREFERENCES ===");
            }
        }
    }
    
    // Supporting data structures
    [System.Serializable]
    public class GameSaveData
    {
        // Game state
        public DateTime SaveTime;
        public string GameVersion;
        public string FileName;
        
        // Manager data
        public int Credits;
        public int CorrectDecisions;
        public int WrongDecisions;
        public int CurrentStrikes;
        public int CurrentDay;
        public int ShipsProcessedToday;
        public float RemainingTime;
        public bool IsShiftActive;
        public string CurrentGameState;
        
        // Additional game data can be added here
    }
    
    [System.Serializable]
    public class SaveFileInfo
    {
        public string FileName;
        public string FilePath;
        public DateTime CreatedTime;
        public DateTime ModifiedTime;
        public DateTime SaveTime;
        public long FileSize;
        public string GameVersion;
        public int CurrentDay;
    }
    
    [System.Serializable]
    public class PlayerPreferences
    {
        public float audioVolume = 1.0f;
        public int graphicsQuality = 2;
        public bool autoSaveEnabled = true;
        public Dictionary<string, string> customSettings = new Dictionary<string, string>();
        
        public void SetValue<T>(string key, T value)
        {
            customSettings[key] = value.ToString();
        }
        
        public T GetValue<T>(string key, T defaultValue = default(T))
        {
            if (customSettings.TryGetValue(key, out string stringValue))
            {
                try
                {
                    return (T)System.Convert.ChangeType(stringValue, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }
}