using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The absolute simplest drag implementation
/// Just moves the UI element by the mouse delta
/// </summary>
public class UltraSimpleDrag : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Bring to front
        transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // Simply add the delta movement to current position
        rectTransform.anchoredPosition += eventData.delta;
    }
}