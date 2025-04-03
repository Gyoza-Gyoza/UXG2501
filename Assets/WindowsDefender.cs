using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WindowsDefender : MonoBehaviour
{
    [SerializeField]
    private GameObject finalPopup;

    [SerializeField]
    private GameObject resetKeyWindow;
    [SerializeField]
    private GameObject resetKeyInvalidPasswordText;

    [SerializeField]
    private GameObject windowsDefenderNotification;

    [SerializeField]
    private GameObject passwordInputGO;
    private TMP_InputField passwordInput;

    public static WindowsDefender Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        passwordInput = passwordInputGO.GetComponent<TMP_InputField>();
    }
    public void SetResetKeyWindowActive(bool state) => resetKeyWindow.SetActive(state);
    public void SetWDNotificationActive(bool state)
    {
        windowsDefenderNotification.SetActive(state);
        windowsDefenderNotification.transform.SetAsLastSibling();
    }
    public void SetWDNotificationActiveWithDelay(float duration)
    {
        StartCoroutine(Delay(duration));
    }
    private IEnumerator Delay(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetWDNotificationActive(true);
    }
    public void CheckPassword()
    {
        if (passwordInput.text == "111111")
        {
            SetResetKeyWindowActive(false);
            if(PhaseManager.Instance.CurrentPhase is FinalPhase finalPhase)
            {
                finalPhase.SetPopupActive(true);
            }
        }
        else
        {
            resetKeyInvalidPasswordText.SetActive(true);
            passwordInput.text = "";
        }
    }
    public void PlayEndSequence()
    {
        StartCoroutine(EndSequence());
    }
    private IEnumerator EndSequence()
    {
        yield return new WaitForSeconds(1f);
        ChatAPTBehaviour.Instance.CloseWindow();

        yield return new WaitForSeconds(1f);
        ChatAPTBehaviour.Instance.chatIcon.gameObject.SetActive(false);

        yield return new WaitForSeconds(3f);
        finalPopup.SetActive(true);
    }
}
