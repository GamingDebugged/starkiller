using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Base class for annoying popups that distract the player
/// </summary>
public class AnnoyingPopup : MonoBehaviour
{
    [Header("Popup Settings")]
    [SerializeField] protected string[] possibleTitles;
    [SerializeField] protected string[] possibleMessages;
    [SerializeField] protected Sprite[] possibleIcons;
    
    [Header("UI References")]
    [SerializeField] protected TMP_Text titleText;
    [SerializeField] protected TMP_Text messageText;
    [SerializeField] protected Image iconImage;
    [SerializeField] protected Button closeButton;
    [SerializeField] protected Button actionButton;
    [SerializeField] protected TMP_Text actionButtonText;
    
    [Header("Behavior")]
    [SerializeField] protected bool autoClose = true;
    [SerializeField] protected float autoCloseDelay = 30f;
    [SerializeField] protected bool shakeOnSpawn = true;
    [SerializeField] protected bool flashOnSpawn = true;
    
    protected virtual void Start()
    {
        SetupContent();
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() => ClosePopup());
        }
        
        if (actionButton != null)
        {
            actionButton.onClick.AddListener(() => OnActionButtonClicked());
        }
        
        if (shakeOnSpawn)
        {
            StartCoroutine(ShakeAnimation());
        }
        
        if (flashOnSpawn)
        {
            StartCoroutine(FlashAnimation());
        }
        
        if (autoClose)
        {
            Invoke(nameof(ClosePopup), autoCloseDelay);
        }
    }
    
    protected virtual void SetupContent()
    {
        // Set random title
        if (titleText != null && possibleTitles != null && possibleTitles.Length > 0)
        {
            titleText.text = possibleTitles[Random.Range(0, possibleTitles.Length)];
        }
        
        // Set random message
        if (messageText != null && possibleMessages != null && possibleMessages.Length > 0)
        {
            messageText.text = possibleMessages[Random.Range(0, possibleMessages.Length)];
        }
        
        // Set random icon
        if (iconImage != null && possibleIcons != null && possibleIcons.Length > 0)
        {
            iconImage.sprite = possibleIcons[Random.Range(0, possibleIcons.Length)];
        }
    }
    
    protected virtual void OnActionButtonClicked()
    {
        // Override in derived classes
        ClosePopup();
    }
    
    protected virtual void ClosePopup()
    {
        // Could add close animation here
        Destroy(gameObject);
    }
    
    protected IEnumerator ShakeAnimation()
    {
        RectTransform rt = GetComponent<RectTransform>();
        Vector3 originalPos = rt.localPosition;
        float duration = 0.5f;
        float elapsed = 0f;
        float magnitude = 10f;
        
        while (elapsed < duration)
        {
            float x = Random.Range(-magnitude, magnitude);
            float y = Random.Range(-magnitude, magnitude);
            rt.localPosition = originalPos + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            magnitude *= 0.9f; // Decay
            yield return null;
        }
        
        rt.localPosition = originalPos;
    }
    
    protected IEnumerator FlashAnimation()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg == null) cg = gameObject.AddComponent<CanvasGroup>();
        
        for (int i = 0; i < 3; i++)
        {
            cg.alpha = 0.5f;
            yield return new WaitForSeconds(0.1f);
            cg.alpha = 1f;
            yield return new WaitForSeconds(0.1f);
        }
    }
}

/// <summary>
/// Spam email popup
/// </summary>
public class SpamEmailPopup : AnnoyingPopup
{
    protected override void Start()
    {
        possibleTitles = new string[] {
            "URGENT: You've Won!",
            "Re: Your Recent Purchase",
            "FINAL NOTICE",
            "Claim Your Prize NOW!",
            "Important Security Update"
        };
        
        possibleMessages = new string[] {
            "Congratulations! You've been selected for a special offer. Click here to claim 1000 credits!",
            "Your Starkiller Base parking pass is about to expire. Renew now for only 50 credits!",
            "Hot singles in your sector want to meet you!",
            "Imperium Prince needs your help transferring 1,000,000 credits...",
            "Your computer may be infected! Download our security scanner now!"
        };
        
        if (actionButtonText != null)
        {
            actionButtonText.text = "Claim Now!";
        }
        
        base.Start();
    }
    
    protected override void OnActionButtonClicked()
    {
        // Spawn another popup as punishment for clicking spam
        if (DraggablePanelManager.Instance != null)
        {
            messageText.text = "Processing... ERROR: Invalid credentials. Please try again.";
            actionButton.interactable = false;
        }
    }
}

/// <summary>
/// System notification popup
/// </summary>
public class SystemNotificationPopup : AnnoyingPopup
{
    protected override void Start()
    {
        possibleTitles = new string[] {
            "System Update Available",
            "Printer Error",
            "Low Disk Space",
            "Network Connection Lost",
            "Backup Reminder"
        };
        
        possibleMessages = new string[] {
            "A new update is available for your workstation. Install now? (Estimated time: 3 hours)",
            "Printer in Sector C is out of toner. Again.",
            "Your personal storage is 98% full. Please delete unnecessary files.",
            "Connection to Imperium network temporarily lost. Retrying...",
            "You haven't backed up your files in 47 days!"
        };
        
        if (actionButtonText != null)
        {
            actionButtonText.text = "Remind Later";
        }
        
        base.Start();
    }
}

/// <summary>
/// Social message popup from colleagues
/// </summary>
public class ColleagueMessagePopup : AnnoyingPopup
{
    protected override void Start()
    {
        possibleTitles = new string[] {
            "Message from TK-421",
            "Lunch Plans?",
            "Quick Question",
            "FW: FW: FW: Funny",
            "Meeting in 5!"
        };
        
        possibleMessages = new string[] {
            "Hey, did you see the game last night? Can't believe they lost!",
            "Want to grab lunch at the cantina? They have blue milk on special.",
            "Do you know where the TPS reports go? I can never remember.",
            "Check out this hilarious holovid! [ATTACHMENT: DancingEwok.holo]",
            "Don't forget about the mandatory fun committee meeting!"
        };
        
        if (actionButtonText != null)
        {
            actionButtonText.text = "Reply";
        }
        
        base.Start();
    }
    
    protected override void OnActionButtonClicked()
    {
        messageText.text = "Reply sent: 'Sorry, busy with inspections right now!'";
        actionButton.interactable = false;
    }
}

/// <summary>
/// Advertisement popup
/// </summary>
public class AdvertisementPopup : AnnoyingPopup
{
    [Header("Ad Specific")]
    [SerializeField] private float animationSpeed = 1f;
    [SerializeField] private bool useBannerAnimation = true;
    
    protected override void Start()
    {
        possibleTitles = new string[] {
            "Special Offer!",
            "Limited Time Deal",
            "Exclusive for You",
            "Don't Miss Out!",
            "Today Only!"
        };
        
        possibleMessages = new string[] {
            "New Speeder Model X-34! Now with 20% more thrust! Finance available.",
            "Tired of bland rations? Try new FLAVORBLAST meal supplements!",
            "Join the Imperium Rewards Program and earn points on every purchase!",
            "Stormtrooper armor polish - Keep your gear inspection-ready!",
            "Visit scenic Tatooine! Two suns for the price of one!"
        };
        
        if (actionButtonText != null)
        {
            actionButtonText.text = "Learn More";
        }
        
        base.Start();
        
        if (useBannerAnimation)
        {
            StartCoroutine(BannerAnimation());
        }
    }
    
    private IEnumerator BannerAnimation()
    {
        Transform banner = transform.Find("Banner");
        if (banner == null) banner = transform;
        
        while (true)
        {
            banner.localScale = Vector3.one * (1f + Mathf.Sin(Time.time * animationSpeed) * 0.1f);
            yield return null;
        }
    }
}

/// <summary>
/// Error/Warning popup
/// </summary>
public class ErrorPopup : AnnoyingPopup
{
    [Header("Error Specific")]
    [SerializeField] private AudioClip errorSound;
    
    protected override void Start()
    {
        possibleTitles = new string[] {
            "WARNING",
            "ERROR",
            "SYSTEM ALERT",
            "CRITICAL ERROR",
            "ATTENTION REQUIRED"
        };
        
        possibleMessages = new string[] {
            "Error 404: Motivation not found.",
            "Warning: Coffee levels critically low.",
            "System32 has stopped responding. This is probably fine.",
            "Unspecified error in unspecified module. Please contact IT.",
            "Task failed successfully."
        };
        
        if (actionButtonText != null)
        {
            actionButtonText.text = "OK";
        }
        
        // Make it red
        Image panelImage = GetComponent<Image>();
        if (panelImage != null)
        {
            panelImage.color = new Color(1f, 0.8f, 0.8f);
        }
        
        base.Start();
        
        // Play error sound
        if (errorSound != null)
        {
            AudioSource.PlayClipAtPoint(errorSound, transform.position);
        }
    }
}