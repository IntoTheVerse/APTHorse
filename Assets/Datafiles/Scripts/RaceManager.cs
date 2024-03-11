using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    public float DummyToken = 10f;

    public TMP_Text _availableToken;
    public TMP_InputField tMP_InputField_betAmount;
    public TMP_Dropdown tMP_Dropdown_Horsecolor;
    public TMP_InputField tMP_InputField_totallap;
    public Button _Bet;
    public Button _start;
    public CanvasGroup _toast;
    public TMP_Text _toastText;
    public GameObject BetPanel;
    public GameObject ResultPanel;
    public Transform _content;
    public PlayerItem playerItem;


    public HorseController[] jockeys;
    public List<HorseController> _horseRanks = new List<HorseController>();
    static Action UpdateAtt;

    //On which playe has bet
    public int jockeyId;
    public int totalLap;

    public Stack<HorseController> horses = new Stack<HorseController>();

    private void Start()
    {

        #region Go through each jockey and assign the properties
        /* foreach (var jockey in jockeys)
         {
             if (jockey.playerProperties.color == HorseColor.Green)
             {

             }else if (jockey.playerProperties.color == HorseColor.White)
             {

             }
             else if (jockey.playerProperties.color == HorseColor.Blue)
             {

             }
             else if (jockey.playerProperties.color == HorseColor.Brown)
             {

             }
         }*/
        #endregion

        UpdateAtt += UpdateAttributes;
        UpdateAtt?.Invoke();
        _Bet.onClick.AddListener(() =>
        {
            Bet();
        }
        );
        _start.onClick.AddListener(() =>
        {
            StartRace();
        });
    }

    private void StartRace()
    {
        foreach (var jockey in jockeys)
        {
            jockey._maxSpeed = UnityEngine.Random.Range(4, 6);
            jockey._minSpeed = UnityEngine.Random.Range(1, 2);
            jockey._acceleration = UnityEngine.Random.Range(0.1f, 1.1f);
            jockey.totalLap = totalLap;
            jockey.StartRace();
        }
        BetPanel.gameObject.SetActive(false);
    }

    private void Bet()
    {
        int.TryParse(tMP_InputField_totallap.text, out int totalLapVal);
        if (int.TryParse(tMP_InputField_betAmount.text, out int betAmount))
        {
            if(betAmount < DummyToken)
            {
                _Bet.interactable = false;
                _start.interactable = true;
                DummyToken = DummyToken - betAmount;
                jockeyId = tMP_Dropdown_Horsecolor.value;
                totalLap = totalLapVal;
                UpdateAtt?.Invoke();
            }
            else
            {
                _toastText.text = "Not enough token";
                LeanTween.alphaCanvas(_toast, 1, 1f).setOnComplete(() => { LeanTween.alphaCanvas(_toast, 0, 3f); });
            }
        }
        else
        {
            _toastText.text = "Not valid input";
            LeanTween.alphaCanvas(_toast, 1, 1f).setOnComplete(() => { LeanTween.alphaCanvas(_toast, 0, 3f); });
        }
    }

    public void UpdateAttributes()
    {
        _availableToken.text = "Token balance " + DummyToken.ToString();

    }

    private void OnDestroy()
    {
        UpdateAtt -= UpdateAttributes;
    }

    public void PrintStack()
    {
        foreach(var horse in horses)
        {
            Debug.LogError(horse.playerProperties.color);
        }
        if (horses.Count>3)
        {
            ResultPanel.SetActive(true);
            while (horses.Count>0)
            { 
                _horseRanks.Add(horses.Pop());
            }
            _horseRanks.Reverse();
            int rank = 1;
            foreach (var horse in _horseRanks)
            {
                GameObject playerInfo = Instantiate(playerItem.gameObject, _content);
                playerInfo.GetComponent<PlayerItem>()._playerName.text = rank.ToString() + ". " + horse.playerProperties.color.ToString();
                rank++;
            }
        }
    }
}