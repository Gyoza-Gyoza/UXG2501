using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionNumber;
        public string questionText;
        public string[] answers; // A, B, C, D
    }

    [Header("Screens")]
    public GameObject introPage;
    public GameObject quizPage;
    public GameObject summaryPage;
    public GameObject scorePage;

    [Header("Question Bank")]
    public TMP_Text questionNumber;
    public TMP_Text questionText;
    private int currentQuestionIndex = 0;
    public List<Question> questions = new List<Question>();
    private bool isAnswerSelected = false;
    public TMP_Text[] answerOptions;
    public Button questionBackButton;

    //Question Page Answer Buttons
    public Toggle[] answerButtons;

    //Question Page Submit Button
    public Button submitButton;
    public Image submitButtonImage;
    public Sprite submitedEnabled;
    public Sprite submitedDisabled;

    [Header("Summary Page")]
    public TMP_Text summaryText;
    public Button backButton;
    public Button finishButton;

    [Header("Score Page")]
    public Button restartButton;

    //Stored answers
    private int[] selectedAnswers;


    void Start()
    {
        submitButton.onClick.AddListener(NextQuestion);
        backButton.onClick.AddListener(GoBack);
        questionBackButton.onClick.AddListener(GoBack);
        restartButton.onClick.AddListener(RestartQuiz);
        finishButton.onClick.AddListener(FinishQuiz);
    }

    public void OpenWindow()
    {
        //Show Intro Page and hide the other pages
        introPage.SetActive(true);
        quizPage.SetActive(false);
        summaryPage.SetActive(false);
        scorePage.SetActive(false);

        //Initialise selectedAnswers array
        selectedAnswers = new int[questions.Count];
        for (int i = 0; i < selectedAnswers.Length; i++) 
        selectedAnswers[i] = -1; // -1 means no answer selected

        foreach (Toggle toggle in answerButtons)
        {
            toggle.onValueChanged.AddListener(delegate { CheckAnswerSelection(); });
        }
        
    }

    public void CheckAnswerSelection()
    {
        isAnswerSelected = false;

        //Check if any toggle is selected
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i].isOn)
            {
                isAnswerSelected = true;
                selectedAnswers[currentQuestionIndex] = i; //Store selected answer index
                break;
            }
        }

        //Enable or disable the submit button based on selection
        submitButton.interactable = isAnswerSelected;
        submitButtonImage.sprite = isAnswerSelected ? submitedEnabled : submitedDisabled;
    }

    public void StartQuiz()
    {
        introPage.SetActive(false);
        quizPage.SetActive(true);

        submitButton.interactable = false;
        submitButtonImage.sprite = submitedDisabled;

        LoadQuestion(0);
    }

    public void LoadQuestion(int index)
    {
        if(index >= questions.Count)
        {
            ShowSummary(); //Trigger summary page
            return;
        }

        currentQuestionIndex = index;
        Question q = questions[index];

        questionNumber.text = q.questionNumber;
        questionText.text = q.questionText;

        for (int i = 0; i < answerOptions.Length; i++)
        {
            answerOptions[i].text = q.answers[i]; //Assign text from list
            answerButtons[i].isOn = (selectedAnswers[currentQuestionIndex] == i); //Restore previous selection
        }

        CheckAnswerSelection();
    }

    public void NextQuestion()
    {
        if(currentQuestionIndex < questions.Count - 1)
        {
            LoadQuestion(currentQuestionIndex + 1);
        }
        else
        {
            ShowSummary();
        }
    }

    public void GoBack()
    {
        if (summaryPage.activeSelf)
        {
            //If on summary page, return to the most recent question
            summaryPage.SetActive(false);
            quizPage.SetActive(true);
        }
        else if (currentQuestionIndex == 0)
        {
            //Reset all selections
            for (int i = 0; i < selectedAnswers.Length; i++)
            selectedAnswers[i] = -1;

            //If on the first question, go back to the intro page
            quizPage.SetActive(false);
            introPage.SetActive(true);
        }
        else
        {
            //Otherwise, go back to the previous question
            LoadQuestion(currentQuestionIndex - 1);
        }
    }

    public void ShowSummary()
    {
        quizPage.SetActive(false);
        summaryPage.SetActive(true);

        //Generate summary text in "Question X: A" format
        string summary = "Summary of Attempt:\n\n";
        for (int i = 0; i < questions.Count; i++)
        {
            //Convert selected answer index (0,1,2,3) to A, B, C, D
            string selectedAnswer = (selectedAnswers[i] != -1) ? ((char)('A' + selectedAnswers[i])).ToString() : "No Answer";

            summary += $"Question {i + 1}: {selectedAnswer}\n";
        }

        summaryText.text = summary;
    }

    public void FinishQuiz()
    {
        summaryPage.SetActive(false);
        quizPage.SetActive(false);
        scorePage.SetActive(true);
    }

    public void RestartQuiz()
    {
        //Reset all selections
        for (int i = 0; i < selectedAnswers.Length; i++)
            selectedAnswers[i] = -1;

        currentQuestionIndex = 0; //Reset to first question

        //Reset UI visibility
        scorePage.SetActive(false);
        quizPage.SetActive(false);
        summaryPage.SetActive(false);
        introPage.SetActive(true);
    }
    private void OnEnable()
    {
        OpenWindow();
    }
}
