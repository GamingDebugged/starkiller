using UnityEngine;

public class UITester : MonoBehaviour
{
    // This method can be called from the inspector or other scripts
    public void MakeTestButtonBlue()
    {
        UIHelper.SetColor("TestButton", new Color(0, 0, 0.6f, 1f));
        Debug.Log("Changed TestButton to dark blue color");
    }
}
