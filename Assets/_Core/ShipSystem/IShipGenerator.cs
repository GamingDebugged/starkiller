using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StarkillerBaseCommand;

namespace StarkillerBaseCommand.ShipSystem
{
    /// <summary>
    /// Interface defining the core functionality of a ship generator in the Starkiller Base Command game.
    /// This provides a common contract for all ship generator implementations.
    /// </summary>
    public interface IShipGenerator
    {
        /// <summary>
        /// Event triggered when a new ship encounter is ready for processing.
        /// </summary>
        event MasterShipGenerator.EncounterReadyHandler OnEncounterReady;

        /// <summary>
        /// Updates the generator for a new day of gameplay.
        /// </summary>
        /// <param name="day">The current game day number</param>
        void StartNewDay(int day);

        /// <summary>
        /// Generates random encounters for a specific day.
        /// </summary>
        /// <param name="day">The day to generate encounters for</param>
        void GenerateEncountersForDay(int day);

        /// <summary>
        /// Creates a random ship encounter.
        /// </summary>
        /// <param name="forceValid">Whether to force the ship to have valid credentials</param>
        /// <returns>A new ship encounter</returns>
        MasterShipEncounter GenerateRandomEncounter(bool forceValid = false);

        /// <summary>
        /// Creates a story-based encounter for specific progression moments.
        /// </summary>
        /// <param name="storyTag">The story tag to use for encounter generation</param>
        /// <returns>A new story-based encounter</returns>
        MasterShipEncounter GenerateStoryEncounter(string storyTag);

        /// <summary>
        /// Gets the next pending encounter from the queue.
        /// Always returns a valid encounter (fallback created if needed).
        /// </summary>
        /// <returns>The next encounter to process</returns>
        MasterShipEncounter GetNextEncounter();

        /// <summary>
        /// Processes a player decision on the current encounter.
        /// </summary>
        /// <param name="approved">Whether the ship was approved</param>
        void ProcessDecision(bool approved);

        /// <summary>
        /// Processes a decision with an explicitly provided encounter.
        /// </summary>
        /// <param name="approved">Whether the ship was approved</param>
        /// <param name="encounter">The encounter to process</param>
        void ProcessDecisionWithEncounter(bool approved, MasterShipEncounter encounter);

        /// <summary>
        /// Processes a bribery acceptance from the player.
        /// </summary>
        /// <param name="encounter">The encounter offering the bribe</param>
        void ProcessBriberyAccepted(MasterShipEncounter encounter);

        /// <summary>
        /// Processes a holding pattern decision from the player.
        /// </summary>
        /// <param name="encounter">The encounter to place in holding pattern</param>
        void ProcessHoldingPattern(MasterShipEncounter encounter);

        /// <summary>
        /// Processes a ship that has completed its holding pattern and should be requeued.
        /// </summary>
        /// <param name="encounter">The encounter that completed holding pattern</param>
        void ProcessHoldingPatternCompletion(MasterShipEncounter encounter);

        /// <summary>
        /// Re-queues an encounter to be processed again later.
        /// </summary>
        /// <param name="encounter">The encounter to re-queue</param>
        void RequeueEncounter(MasterShipEncounter encounter);

        /// <summary>
        /// Processes a tractor beam capture from the player.
        /// </summary>
        /// <param name="encounter">The encounter to capture with tractor beam</param>
        void ProcessTractorBeam(MasterShipEncounter encounter);
    }
}