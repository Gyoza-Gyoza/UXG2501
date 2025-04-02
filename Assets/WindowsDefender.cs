using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsDefender : MonoBehaviour
{
    [SerializeField]
    private GameObject finalPopup;

    public static WindowsDefender Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this; 
    }
    public void PlayEndSequence()
    {
        StartCoroutine(EndSequence());
    }
    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(1f);
        ChatAPTBehaviour.Instance.CloseWindow();

        yield return new WaitForSeconds(3f);
        finalPopup.SetActive(true);
    }
}
