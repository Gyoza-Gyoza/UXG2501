using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttachmentArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        ChatAPTBehaviour.instance.InAttachmentArea = true;
        Debug.Log("In drop area");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ChatAPTBehaviour.instance.InAttachmentArea = false;
        Debug.Log("Out drop area");
    }
}
