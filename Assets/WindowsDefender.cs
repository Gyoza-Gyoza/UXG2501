using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WindowsDefender : MonoBehaviour, IPointerEnterHandler
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
                transform.parent.gameObject.SetActive(false);
                //StopCoroutine(ChatAPTBehaviour.Instance.windowShake);
                StartCoroutine(EndSequence());
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
    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(1f);
        ChatAPTBehaviour.Instance.CloseWindow();


    }
}
