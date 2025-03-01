using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform; 
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Vector2 originalPos;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
        ChatAPTBehaviour.instance.AttachmentModeActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        ChatAPTBehaviour.instance.AttachmentModeActive(false);

        if (ChatAPTBehaviour.instance.InAttachmentArea)
        {
            ChatAPTBehaviour.instance.AttachObject(this);
            rectTransform.anchoredPosition = originalPos;
        }

        if (!Tools.IsMouseOnScreen()) rectTransform.anchoredPosition = originalPos;
    }
}
