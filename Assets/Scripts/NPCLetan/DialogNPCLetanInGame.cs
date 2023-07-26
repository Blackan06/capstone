using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogNPCLetanInGame : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;
    [SerializeField] List<string> lines;
    [SerializeField] private PlayerHistoryAPI playerHistoryAPI;


    private int currentLineIndex = 0;

    private void Start()
    {
        
        ShowDialog();
    }

    public void ShowDialog()
    {
      
        dialogBox.SetActive(true);
        StartCoroutine(TypeDialog(lines[currentLineIndex]));
    }

    public IEnumerator TypeDialog(string line)
    {
        dialogText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);

        currentLineIndex++;

        if (currentLineIndex < lines.Count)
        {
            StartCoroutine(TypeDialog(lines[currentLineIndex]));
        }

    }
    public void CheckMissionCheckIn()
    {
        DateTime currentTime = DateTime.Now;
        string formattedTime = currentTime.ToString("HH:mm:ss");
        Debug.Log(formattedTime);
        playerHistoryAPI.NotifyTaskCompletion("18be449c-4e28-4e76-8081-046ce29c1282", formattedTime);


    }


}
