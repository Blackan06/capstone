using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuestionByNPCTeacher : MonoBehaviour
{
    public Text questionText;
    public Button[] answerButtons;
    private QuestionList questionList;
    private int currentQuestionIndex;
    private bool isAnswered;
    public GameObject gameContainer;
    public GameObject buttonMessage;
    public GameObject eventtrigger;
    public float gameEndDelay = 2f;

    void Start()
    {
        StartCoroutine(GetQuestionsFromAPI());
    }

    IEnumerator GetQuestionsFromAPI()
    {
        string url = "http://anhkiet-001-site1.htempurl.com/api/Answers/GetAnswers/8262adf3-af3f-4864-a8c7-c1f589f1b2ae";

        UnityWebRequest www = UnityWebRequest.Get(url);

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("L?i khi g?i API: " + www.error);
        }
        else
        {
            string jsonResponse = www.downloadHandler.text;
            questionList = JsonUtility.FromJson<QuestionList>(jsonResponse);

            currentQuestionIndex = 0;
            isAnswered = false;

            DisplayCurrentQuestion();
        }
    }

    void DisplayCurrentQuestion()
    {
        if (currentQuestionIndex < questionList.data.Length)
        {
            var question = questionList.data[currentQuestionIndex];

            questionText.text = question.questionName;

            var answers = question.answerDtos;

            if (answerButtons.Length != answers.Length)
            {
                Debug.LogError("S? l??ng ??i t??ng Button không kh?p v?i s? l??ng câu tr? l?i");
                return;
            }

            for (int i = 0; i < answerButtons.Length; i++)
            {
                var answerButton = answerButtons[i];
                var answerText = answerButton.GetComponentInChildren<Text>();
                answerText.text = answers[i].answer1;

                int answerIndex = i;
                answerButton.onClick.RemoveAllListeners();

                if (!isAnswered)
                {
                    answerButton.onClick.AddListener(() => OnAnswerSelected(answerIndex));
                }

                answerButton.interactable = !isAnswered;
                answerButton.image.color = Color.white;
            }
        }
        else
        {
            questionText.text = "C?m ?n b?n ?ã ch?i game!";
            DisableAnswerButtons();
            StartCoroutine(GameEndDelay());
        }
    }

    void OnAnswerSelected(int answerIndex)
    {
        var question = questionList.data[currentQuestionIndex];
        var selectedAnswer = question.answerDtos[answerIndex];

        bool isCorrect = selectedAnswer.isRight;

        var answerButton = answerButtons[answerIndex];
        answerButton.image.color = isCorrect ? Color.green : Color.red;

        isAnswered = true;
        DisableAnswerButtons();
        StartCoroutine(NextQuestionDelay());
    }

    void DisableAnswerButtons()
    {
        foreach (var answerButton in answerButtons)
        {
            answerButton.interactable = false;
        }
    }

    IEnumerator NextQuestionDelay()
    {
        yield return new WaitForSeconds(gameEndDelay);
        currentQuestionIndex++;
        isAnswered = false;
        DisplayCurrentQuestion();
    }

    IEnumerator GameEndDelay()
    {
        foreach (var answerButton in answerButtons)
        {
            answerButton.gameObject.SetActive(false); 
        }
        yield return new WaitForSeconds(gameEndDelay);
        gameContainer.SetActive(false);
        buttonMessage.SetActive(false);
        eventtrigger.SetActive(false);
    }
}
