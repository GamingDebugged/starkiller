using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helper class to modify UI elements through code or MCP
/// </summary>
public class UIHelper : MonoBehaviour
{
    // Singleton pattern
    private static UIHelper _instance;
    public static UIHelper Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("UIHelper");
                _instance = go.AddComponent<UIHelper>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Change the color of a UI button or image
    /// </summary>
    public bool ChangeUIColor(string objectName, Color color)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Debug.LogError($"UIHelper: Cannot find object '{objectName}'");
            return false;
        }

        Image image = obj.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
            Debug.Log($"UIHelper: Changed color of '{objectName}' to {color}");
            return true;
        }
        else
        {
            Debug.LogError($"UIHelper: Object '{objectName}' does not have an Image component");
            return false;
        }
    }

    /// <summary>
    /// Change the text of a UI text element (supports both legacy Text and TextMeshPro)
    /// </summary>
    public bool ChangeUIText(string objectName, string newText)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Debug.LogError($"UIHelper: Cannot find object '{objectName}'");
            return false;
        }

        // Try TextMeshPro first
        TMP_Text tmpText = obj.GetComponent<TMP_Text>();
        if (tmpText != null)
        {
            tmpText.text = newText;
            Debug.Log($"UIHelper: Changed TMP_Text of '{objectName}' to '{newText}'");
            return true;
        }

        // Try legacy Text if TMP isn't found
        Text legacyText = obj.GetComponent<Text>();
        if (legacyText != null)
        {
            legacyText.text = newText;
            Debug.Log($"UIHelper: Changed legacy Text of '{objectName}' to '{newText}'");
            return true;
        }

        Debug.LogError($"UIHelper: Object '{objectName}' has no text component");
        return false;
    }

    /// <summary>
    /// Toggle the active state of a UI element
    /// </summary>
    public bool ToggleUIElement(string objectName, bool? state = null)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Debug.LogError($"UIHelper: Cannot find object '{objectName}'");
            return false;
        }

        if (state.HasValue)
        {
            obj.SetActive(state.Value);
            Debug.Log($"UIHelper: Set active state of '{objectName}' to {state.Value}");
        }
        else
        {
            obj.SetActive(!obj.activeSelf);
            Debug.Log($"UIHelper: Toggled active state of '{objectName}' to {obj.activeSelf}");
        }
        
        return true;
    }

    /// <summary>
    /// Change the interactable state of a UI button
    /// </summary>
    public bool SetButtonInteractable(string objectName, bool interactable)
    {
        GameObject obj = GameObject.Find(objectName);
        if (obj == null)
        {
            Debug.LogError($"UIHelper: Cannot find object '{objectName}'");
            return false;
        }

        Button button = obj.GetComponent<Button>();
        if (button != null)
        {
            button.interactable = interactable;
            Debug.Log($"UIHelper: Set interactable state of button '{objectName}' to {interactable}");
            return true;
        }
        else
        {
            Debug.LogError($"UIHelper: Object '{objectName}' does not have a Button component");
            return false;
        }
    }
    
    // Static convenience methods
    public static bool SetColor(string objectName, Color color)
    {
        return Instance.ChangeUIColor(objectName, color);
    }
    
    public static bool SetText(string objectName, string text)
    {
        return Instance.ChangeUIText(objectName, text);
    }
    
    public static bool Toggle(string objectName, bool? state = null)
    {
        return Instance.ToggleUIElement(objectName, state);
    }
    
    public static bool SetInteractable(string objectName, bool interactable)
    {
        return Instance.SetButtonInteractable(objectName, interactable);
    }
}
