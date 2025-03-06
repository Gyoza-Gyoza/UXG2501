using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttachmentArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChatAPTBehaviour.Instance.InAttachmentArea = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChatAPTBehaviour.Instance.InAttachmentArea = false;
    }
}
