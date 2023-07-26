using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCoins : MonoBehaviour
{
    #region SIngleton:GameCoins

    public static GameCoins Instance;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    [SerializeField] Text[] allCoinsUIText;

    private double Coins;
    private Player player;

    void Start()
    {
        StartCoroutine(GetData());
   }
    private IEnumerator GetCoins()
    {
        yield return StartCoroutine(NamePrefab.GetInstance().CheckPlayer(OnPlayer));

    }
    private IEnumerator GetData()
    {        
        yield return new WaitForSeconds(1f); // ??i 2 giây tr??c khi g?i GetCoins()

        yield return StartCoroutine(GetCoins());
        Coins = player.totalPoint;
        UpdateAllCoinsUIText();

    }


    private void OnPlayer(Player player1)
    {
        player = player1;
    }
    public void UseCoins(int amount)
    {
        Coins -= amount;

        if (player != null)
        {
            Debug.Log(player.id);
            Debug.Log("Vao day");
            player.totalPoint = player.totalPoint - amount;
            StartCoroutine(NamePrefab.GetInstance().UpdatePlayer(player));
        }
    }

    public bool HasEnoughCoins(int amount)
    {
        return (Coins >= amount);
    }

    public void UpdateAllCoinsUIText()
    {
        for (int i = 0; i < allCoinsUIText.Length; i++)
        {
            allCoinsUIText[i].text = Coins.ToString();
        }
    }
}
