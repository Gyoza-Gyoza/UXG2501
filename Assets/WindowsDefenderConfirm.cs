using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowsDefenderConfirm : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField]
    private int moves = 3;
    private void Start()
    {
        if(PhaseManager.Instance.CurrentPhase is FinalPhase finalPhase)
        {
            GetComponent<Button>().onClick.AddListener(() =>
            {
                finalPhase.StopMessages();
                //StopCoroutine(ChatAPTBehaviour.Instance.windowShake);
                WindowsDefender.Instance.PlayEndSequence();

                transform.parent.gameObject.SetActive(false);
            });
        }
    }
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
}
