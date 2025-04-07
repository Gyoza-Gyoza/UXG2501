using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using static System.Net.Mime.MediaTypeNames;

public class WindowsDefender : MonoBehaviour
{
    [SerializeField]
    private GameObject finalPopup;

    [SerializeField]
    private GameObject resetKeyWindow;
    [SerializeField]
    private GameObject resetKeyInvalidPasswordText;

    [SerializeField]
    private GameObject restartingText;

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
    public void SetResetKeyWindowActive(bool state)
    {
        resetKeyWindow.SetActive(state);
        resetKeyWindow.transform.SetAsLastSibling();
    }
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
        ChatAPTBehaviour.Instance.win = true;
        yield return new WaitForSeconds(1f);
        ChatAPTBehaviour.Instance.CloseWindow();
        ChatAPTBehaviour.Instance.timerUI.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
        ChatAPTBehaviour.Instance.chatIcon.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);
        restartingText.SetActive(true);
        restartingText.transform.SetAsLastSibling();

        yield return new WaitForSeconds(5f);
        Color color = restartingText.GetComponent<TextMeshProUGUI>().color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / 1f);
            restartingText.GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, newAlpha);
            yield return null;
        }

        // Ensure it's fully invisible
        restartingText.GetComponent<TextMeshProUGUI>().color = new Color(color.r, color.g, color.b, 0f);

        ChatAPTBehaviour.Instance.blackScreen.SetActive(false);

        yield return new WaitForSeconds(3f);
        finalPopup.SetActive(true);
    }
    public void DismissNotification()
    {
        finalPopup.SetActive(false);
    }
}
