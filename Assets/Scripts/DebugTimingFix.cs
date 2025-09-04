using UnityEngine;
using System.Reflection;

public class DebugTimingFix : MonoBehaviour
{
    void Start()
    {
        // Fix GameManager timing
        GameManager gm = FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.timeBetweenShips = 5f; // Increase from 1 to 5 seconds
            // gm.shiftTimeLimit = 180f; // 3 minutes - DISABLED: Keep default 30 seconds
            Debug.Log("Fixed GameManager timing settings (kept 30s shift limit)");
        }
        
        // Fix ShipTimingController using reflection
        ShipTimingController timingController = FindAnyObjectByType<ShipTimingController>();
        if (timingController != null)
        {
            try
            {
                var type = typeof(ShipTimingController);
                
                // Set timeBetweenShips
                var timeBetweenShipsField = type.GetField("timeBetweenShips", BindingFlags.NonPublic | BindingFlags.Instance);
                if (timeBetweenShipsField != null)
                    timeBetweenShipsField.SetValue(timingController, 5f);
                
                // Set minimumEncounterDuration
                var minDurationField = type.GetField("minimumEncounterDuration", BindingFlags.NonPublic | BindingFlags.Instance);
                if (minDurationField != null)
                    minDurationField.SetValue(timingController, 3f);
                
                Debug.Log("Fixed ShipTimingController timing settings");
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not fix ShipTimingController: {e.Message}");
            }
        }
        
        // Fix MasterShipGenerator using reflection
        MasterShipGenerator masterGen = FindAnyObjectByType<MasterShipGenerator>();
        if (masterGen != null)
        {
            try
            {
                var type = typeof(MasterShipGenerator);
                var encountersField = type.GetField("encountersPerDay", BindingFlags.NonPublic | BindingFlags.Instance);
                if (encountersField != null)
                {
                    encountersField.SetValue(masterGen, 20);
                    Debug.Log("Fixed MasterShipGenerator encounters per day");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Could not fix MasterShipGenerator: {e.Message}");
            }
        }
        
        // Ensure CredentialChecker has correct references
        CredentialChecker credChecker = FindAnyObjectByType<CredentialChecker>();
        if (credChecker != null && masterGen != null)
        {
            // Force reconnection
            FieldInfo field = typeof(CredentialChecker).GetField("masterShipGenerator", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(credChecker, masterGen);
                Debug.Log("Reconnected CredentialChecker to MasterShipGenerator");
            }
        }
        
        // Try to disable verbose logging using reflection
        DebugMonitor debugMonitor = FindAnyObjectByType<DebugMonitor>();
        if (debugMonitor != null)
        {
            try
            {
                var type = typeof(DebugMonitor);
                var verboseField = type.GetField("verboseLogging", BindingFlags.NonPublic | BindingFlags.Instance);
                if (verboseField != null)
                {
                    verboseField.SetValue(debugMonitor, false);
                    Debug.Log("Disabled verbose logging in DebugMonitor");
                }
            }
            catch { }
        }
    }
}