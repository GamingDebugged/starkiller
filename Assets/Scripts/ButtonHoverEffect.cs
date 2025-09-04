using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Button Sprites")]
    public Sprite normalSprite;
    public Sprite hoverSprite;
    
    [Header("Audio")]
    public AudioSource hoverSound;
    public AudioSource clickSound;
    
    private Image buttonImage;
    
    void Awake()
    {
        buttonImage = GetComponent<Image>();
        
        // Set initial sprite
        if (normalSprite != null && buttonImage != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSprite != null && buttonImage != null)
        {
            buttonImage.sprite = hoverSprite;
            
            if (hoverSound != null)
            {
                hoverSound.Play();
            }
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (normalSprite != null && buttonImage != null)
        {
            buttonImage.sprite = normalSprite;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            clickSound.Play();
        }
    }
}