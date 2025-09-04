using System;
using UnityEngine;

namespace Starkiller.Core
{
    /// <summary>
    /// Central event system for game-wide events
    /// Replaces direct references between systems
    /// </summary>
    public static class GameEvents
    {
        // Game State Events
        public static event Action<GameState> OnGameStateChanged;
        public static event Action OnGameStarted;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action OnGameEnded;

        // Day/Time Events
        public static event Action<int> OnDayChanged;
        public static event Action<float> OnTimeOfDayChanged;
        public static event Action OnShiftStarted;
        public static event Action OnShiftEnded;

        // Encounter Events
        public static event Action<IEncounter> OnEncounterGenerated;
        public static event Action<IEncounter> OnEncounterDisplayed;
        public static event Action<IEncounter> OnEncounterCompleted;
        public static event Action OnEncounterQueueEmpty;

        // Decision Events
        public static event Action<DecisionType, IEncounter> OnDecisionMade;
        public static event Action<IEncounter> OnShipApproved;
        public static event Action<IEncounter> OnShipDenied;
        public static event Action<IEncounter> OnShipHoldingPattern;
        public static event Action<IEncounter> OnShipTractorBeamed;

        // Resource Events
        public static event Action<int> OnCreditsChanged;
        public static event Action<int> OnStrikesChanged;
        public static event Action<ResourceType, int> OnResourceChanged;

        // UI Events
        public static event Action<string> OnUINotification;
        public static event Action<string, float> OnUITimedNotification;
        public static event Action OnUIRefreshRequested;

        // Audio Events
        public static event Action<string> OnPlaySound;
        public static event Action<string> OnPlayMusic;
        public static event Action OnStopAllSounds;

        // Save/Load Events
        public static event Action OnSaveRequested;
        public static event Action OnLoadRequested;
        public static event Action OnSaveCompleted;
        public static event Action OnLoadCompleted;

        // Invoke Methods (with null-safe pattern)
        public static void TriggerGameStateChanged(GameState newState)
        {
            OnGameStateChanged?.Invoke(newState);
        }

        public static void TriggerGameStarted()
        {
            OnGameStarted?.Invoke();
        }

        public static void TriggerGamePaused()
        {
            OnGamePaused?.Invoke();
        }

        public static void TriggerGameResumed()
        {
            OnGameResumed?.Invoke();
        }

        public static void TriggerGameEnded()
        {
            OnGameEnded?.Invoke();
        }

        public static void TriggerDayChanged(int newDay)
        {
            OnDayChanged?.Invoke(newDay);
        }

        public static void TriggerEncounterGenerated(IEncounter encounter)
        {
            OnEncounterGenerated?.Invoke(encounter);
        }

        public static void TriggerDecisionMade(DecisionType decision, IEncounter encounter)
        {
            OnDecisionMade?.Invoke(decision, encounter);
            
            // Also trigger specific decision events
            switch (decision)
            {
                case DecisionType.Approve:
                    OnShipApproved?.Invoke(encounter);
                    break;
                case DecisionType.Deny:
                    OnShipDenied?.Invoke(encounter);
                    break;
                case DecisionType.HoldingPattern:
                    OnShipHoldingPattern?.Invoke(encounter);
                    break;
                case DecisionType.TractorBeam:
                    OnShipTractorBeamed?.Invoke(encounter);
                    break;
            }
        }

        public static void TriggerCreditsChanged(int newAmount)
        {
            OnCreditsChanged?.Invoke(newAmount);
        }

        public static void TriggerStrikesChanged(int newAmount)
        {
            OnStrikesChanged?.Invoke(newAmount);
        }

        public static void TriggerUINotification(string message)
        {
            OnUINotification?.Invoke(message);
        }

        public static void TriggerSaveRequested()
        {
            OnSaveRequested?.Invoke();
        }

        public static void TriggerLoadRequested()
        {
            OnLoadRequested?.Invoke();
        }

        public static void TriggerSaveCompleted()
        {
            OnSaveCompleted?.Invoke();
        }

        public static void TriggerLoadCompleted()
        {
            OnLoadCompleted?.Invoke();
        }

        public static void TriggerEncounterDisplayed(IEncounter encounter)
        {
            OnEncounterDisplayed?.Invoke(encounter);
        }

        public static void TriggerEncounterCompleted(IEncounter encounter)
        {
            OnEncounterCompleted?.Invoke(encounter);
        }

        // Clear all event subscriptions (useful for scene changes)
        public static void ClearAllSubscriptions()
        {
            OnGameStateChanged = null;
            OnGameStarted = null;
            OnGamePaused = null;
            OnGameResumed = null;
            OnGameEnded = null;
            OnDayChanged = null;
            OnTimeOfDayChanged = null;
            OnShiftStarted = null;
            OnShiftEnded = null;
            OnEncounterGenerated = null;
            OnEncounterDisplayed = null;
            OnEncounterCompleted = null;
            OnEncounterQueueEmpty = null;
            OnDecisionMade = null;
            OnShipApproved = null;
            OnShipDenied = null;
            OnShipHoldingPattern = null;
            OnShipTractorBeamed = null;
            OnCreditsChanged = null;
            OnStrikesChanged = null;
            OnResourceChanged = null;
            OnUINotification = null;
            OnUITimedNotification = null;
            OnUIRefreshRequested = null;
            OnPlaySound = null;
            OnPlayMusic = null;
            OnStopAllSounds = null;
            OnSaveRequested = null;
            OnLoadRequested = null;
            OnSaveCompleted = null;
            OnLoadCompleted = null;
            
            Debug.Log("[GameEvents] All event subscriptions cleared");
        }
    }

    // Supporting Enums and Interfaces
    public enum GameState
    {
        MainMenu,
        DailyBriefing,
        Gameplay,
        Paused,
        DayReport,
        GameOver
    }

    public enum DecisionType
    {
        Approve,
        Deny,
        HoldingPattern,
        TractorBeam
    }

    public enum ResourceType
    {
        Credits,
        Strikes,
        Reputation,
        Energy
    }

    public interface IEncounter
    {
        string ShipType { get; }
        string CaptainName { get; }
        string AccessCode { get; }
        bool IsValid { get; }
    }
}