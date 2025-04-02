using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowsDefenderConfirm : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private int moves = 3;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(moves >= 1)
        {
            if (PhaseManager.Instance.CurrentPhase is FinalPhase finalPhase)
            {
                finalPhase.MovePopup();
                moves--;
            }
        }

        if (moves <= 0)
        {
            LastMove();
        }
    }
    private void LastMove()
    {
        if (PhaseManager.Instance.CurrentPhase is FinalPhase finalPhase)
        {
            StartCoroutine(finalPhase.SpamMessages());
        }
    }
    private void Update()
    {
        
    }
}
