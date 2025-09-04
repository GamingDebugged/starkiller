using UnityEngine;
using UnityEngine.UI;

public class TestButtonHandler : MonoBehaviour
{
    public Button testButton;
    public CredentialChecker credentialChecker;
    public ShipEncounterSystem shipSystem;
    
    // Add reference to coordinator
    public ShipGeneratorCoordinator shipCoordinator;

    void Awake()
    {
        // Try to find coordinator if not assigned
        if (shipCoordinator == null)
            shipCoordinator = FindFirstObjectByType<ShipGeneratorCoordinator>();
    }

    void Start()
    {
        testButton.onClick.AddListener(TestShipGeneration);
    }

    void TestShipGeneration()
    {
        // Try to use the coordinator if available
        if (shipCoordinator != null)
        {
            shipCoordinator.DisplayTestShip(true);
            return;
        }
        
        // Fall back to the original method if coordinator isn't available
        if (shipSystem != null && credentialChecker != null)
        {
            ShipEncounter testShip = shipSystem.CreateTestShip();
            credentialChecker.DisplayEncounter(testShip);
        }
        else
        {
            Debug.LogError("TestButtonHandler: Missing required references!");
        }
    }
}