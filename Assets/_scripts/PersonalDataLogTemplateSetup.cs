using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One-time setup script to ensure PersonalDataLog templates are properly configured
/// This ensures NewsEntryTemplate, FamilyActionTemplate, and VideoEntryTemplate are visually distinct
/// </summary>
public class PersonalDataLogTemplateSetup : MonoBehaviour
{
    [Header("Template References")]
    [SerializeField] private GameObject newsEntryTemplate;
    [SerializeField] private GameObject familyActionTemplate;
    [SerializeField] private GameObject videoEntryTemplate;
    
    [Header("Visual Settings")]
    [SerializeField] private Color newsBackgroundColor = new Color(0.1f, 0.1f, 0.3f, 0.9f); // Dark blue
    [SerializeField] private Color familyBackgroundColor = new Color(0.2f, 0.3f, 0.1f, 0.9f); // Dark green
    [SerializeField] private Color videoBackgroundColor = new Color(0.3f, 0.1f, 0.1f, 0.9f); // Dark red
    
    [ContextMenu("Setup Templates")]
    public void SetupTemplates()
    {
        Debug.Log("[PersonalDataLogTemplateSetup] Setting up templates...");
        
        // Setup News Entry Template
        if (newsEntryTemplate != null)
        {
            SetupNewsTemplate();
        }
        else
        {
            Debug.LogWarning("[PersonalDataLogTemplateSetup] NewsEntryTemplate not assigned!");
        }
        
        // Setup Family Action Template
        if (familyActionTemplate != null)
        {
            SetupFamilyTemplate();
        }
        else
        {
            Debug.LogWarning("[PersonalDataLogTemplateSetup] FamilyActionTemplate not assigned!");
        }
        
        // Setup Video Entry Template
        if (videoEntryTemplate != null)
        {
            SetupVideoTemplate();
        }
        else
        {
            Debug.LogWarning("[PersonalDataLogTemplateSetup] VideoEntryTemplate not assigned!");
        }
        
        Debug.Log("[PersonalDataLogTemplateSetup] Template setup complete!");
    }
    
    private void SetupNewsTemplate()
    {
        Debug.Log("[PersonalDataLogTemplateSetup] Setting up NewsEntryTemplate...");
        
        // Set background color
        Image background = newsEntryTemplate.GetComponent<Image>();
        if (background == null)
        {
            background = newsEntryTemplate.AddComponent<Image>();
        }
        background.color = newsBackgroundColor;
        
        // Ensure it has proper layout components
        LayoutElement layoutElement = newsEntryTemplate.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = newsEntryTemplate.AddComponent<LayoutElement>();
        }
        layoutElement.minHeight = 100f;
        layoutElement.preferredHeight = 150f;
        
        // Add padding
        VerticalLayoutGroup vlg = newsEntryTemplate.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = newsEntryTemplate.AddComponent<VerticalLayoutGroup>();
        }
        vlg.padding = new RectOffset(15, 15, 10, 10);
        vlg.spacing = 5f;
        
        // Ensure text components exist
        EnsureTextComponent(newsEntryTemplate, "Headline", "NEWS HEADLINE", 18, FontStyles.Bold);
        EnsureTextComponent(newsEntryTemplate, "Content", "News content goes here...", 14, FontStyles.Normal);
        
        // Hide any non-video buttons (news entries are read-only except for video play)
        Button[] buttons = newsEntryTemplate.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            // Don't hide PlayButton as it's needed for video entries
            if (!button.name.Contains("PlayButton"))
            {
                button.gameObject.SetActive(false);
            }
        }
        
        // Ensure Play button exists for video entries
        Button playButton = EnsureButton(newsEntryTemplate, "PlayButton", "▶ Play Video");
        if (playButton != null)
        {
            // Style the play button
            Image buttonImage = playButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.2f, 0.7f, 0.3f, 1f); // Green
            }
            
            // Position the button appropriately
            LayoutElement playButtonLayout = playButton.GetComponent<LayoutElement>();
            if (playButtonLayout == null)
            {
                playButtonLayout = playButton.gameObject.AddComponent<LayoutElement>();
            }
            playButtonLayout.minHeight = 30f;
            playButtonLayout.preferredHeight = 35f;
            
            // Initially hide it - will be shown only for video entries
            playButton.gameObject.SetActive(false);
        }
        
        Debug.Log("[PersonalDataLogTemplateSetup] NewsEntryTemplate configured!");
    }
    
    private void SetupFamilyTemplate()
    {
        Debug.Log("[PersonalDataLogTemplateSetup] Setting up FamilyActionTemplate...");
        
        // Set background color
        Image background = familyActionTemplate.GetComponent<Image>();
        if (background == null)
        {
            background = familyActionTemplate.AddComponent<Image>();
        }
        background.color = familyBackgroundColor;
        
        // Ensure it has proper layout components
        LayoutElement layoutElement = familyActionTemplate.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = familyActionTemplate.AddComponent<LayoutElement>();
        }
        layoutElement.minHeight = 120f;
        layoutElement.preferredHeight = 180f;
        
        // Add padding
        VerticalLayoutGroup vlg = familyActionTemplate.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = familyActionTemplate.AddComponent<VerticalLayoutGroup>();
        }
        vlg.padding = new RectOffset(15, 15, 10, 10);
        vlg.spacing = 8f;
        
        // Ensure text components exist
        EnsureTextComponent(familyActionTemplate, "Headline", "FAMILY UPDATE", 18, FontStyles.Bold);
        EnsureTextComponent(familyActionTemplate, "Content", "Family message content...", 14, FontStyles.Normal);
        
        // Ensure action button exists
        Button actionButton = EnsureButton(familyActionTemplate, "ActionButton", "Take Action (50 Credits)");
        if (actionButton != null)
        {
            // Style the button
            Image buttonImage = actionButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.4f, 0.6f, 0.2f, 1f); // Green tint
            }
        }
        
        Debug.Log("[PersonalDataLogTemplateSetup] FamilyActionTemplate configured!");
    }
    
    private void SetupVideoTemplate()
    {
        Debug.Log("[PersonalDataLogTemplateSetup] Setting up VideoEntryTemplate...");
        
        // Set background color
        Image background = videoEntryTemplate.GetComponent<Image>();
        if (background == null)
        {
            background = videoEntryTemplate.AddComponent<Image>();
        }
        background.color = videoBackgroundColor;
        
        // Ensure it has proper layout components
        LayoutElement layoutElement = videoEntryTemplate.GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = videoEntryTemplate.AddComponent<LayoutElement>();
        }
        layoutElement.minHeight = 200f;
        layoutElement.preferredHeight = 300f;
        
        // Add padding
        VerticalLayoutGroup vlg = videoEntryTemplate.GetComponent<VerticalLayoutGroup>();
        if (vlg == null)
        {
            vlg = videoEntryTemplate.AddComponent<VerticalLayoutGroup>();
        }
        vlg.padding = new RectOffset(15, 15, 10, 10);
        vlg.spacing = 10f;
        
        // Ensure text components exist
        EnsureTextComponent(videoEntryTemplate, "Headline", "VIDEO REPORT", 18, FontStyles.Bold);
        EnsureTextComponent(videoEntryTemplate, "Content", "Video description...", 14, FontStyles.Normal);
        
        // Ensure video display exists
        GameObject videoDisplay = videoEntryTemplate.transform.Find("VideoDisplay")?.gameObject;
        if (videoDisplay == null)
        {
            videoDisplay = new GameObject("VideoDisplay");
            videoDisplay.transform.SetParent(videoEntryTemplate.transform);
            
            RawImage rawImage = videoDisplay.AddComponent<RawImage>();
            rawImage.color = Color.white;
            
            LayoutElement videoLayout = videoDisplay.AddComponent<LayoutElement>();
            videoLayout.minHeight = 120f;
            videoLayout.preferredHeight = 150f;
        }
        
        // Ensure Play button exists
        Button playButton = EnsureButton(videoEntryTemplate, "PlayButton", "▶ Play Video");
        if (playButton != null)
        {
            // Style the play button
            Image buttonImage = playButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = new Color(0.2f, 0.7f, 0.3f, 1f); // Green
            }
            
            // Position the button appropriately
            LayoutElement playButtonLayout = playButton.GetComponent<LayoutElement>();
            if (playButtonLayout == null)
            {
                playButtonLayout = playButton.gameObject.AddComponent<LayoutElement>();
            }
            playButtonLayout.minHeight = 30f;
            playButtonLayout.preferredHeight = 35f;
        }
        
        Debug.Log("[PersonalDataLogTemplateSetup] VideoEntryTemplate configured!");
    }
    
    private TMP_Text EnsureTextComponent(GameObject parent, string name, string defaultText, int fontSize, FontStyles fontStyle)
    {
        Transform existing = parent.transform.Find(name);
        GameObject textObj;
        
        if (existing != null)
        {
            textObj = existing.gameObject;
        }
        else
        {
            textObj = new GameObject(name);
            textObj.transform.SetParent(parent.transform);
        }
        
        TMP_Text text = textObj.GetComponent<TMP_Text>();
        if (text == null)
        {
            text = textObj.AddComponent<TextMeshProUGUI>();
        }
        
        text.text = defaultText;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = Color.white;
        text.alignment = TextAlignmentOptions.Left;
        
        return text;
    }
    
    private Button EnsureButton(GameObject parent, string name, string buttonText)
    {
        Transform existing = parent.transform.Find(name);
        GameObject buttonObj;
        
        if (existing != null)
        {
            buttonObj = existing.gameObject;
        }
        else
        {
            buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent.transform);
            
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
            
            Button button = buttonObj.AddComponent<Button>();
            
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform);
            
            TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = buttonText;
            text.fontSize = 16;
            text.alignment = TextAlignmentOptions.Center;
            text.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
        }
        
        return buttonObj.GetComponent<Button>();
    }
    
    [ContextMenu("Find Templates Automatically")]
    public void FindTemplatesAutomatically()
    {
        GameObject templates = GameObject.Find("Templates");
        if (templates != null)
        {
            newsEntryTemplate = templates.transform.Find("NewsEntryTemplate")?.gameObject;
            familyActionTemplate = templates.transform.Find("FamilyActionTemplate")?.gameObject;
            videoEntryTemplate = templates.transform.Find("VideoEntryTemplate")?.gameObject;
            
            Debug.Log("[PersonalDataLogTemplateSetup] Templates found automatically!");
        }
        else
        {
            Debug.LogError("[PersonalDataLogTemplateSetup] Could not find Templates GameObject!");
        }
    }
}