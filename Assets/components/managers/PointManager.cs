using System;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    public GameObject LevelText;
    public GameObject ScoreText;
    public GameObject MatchesText;
    public GameObject LevelShowText;

    private int score = 0;
    private int combo = 0;
    private int currentBonus = 0;
    public void Setup()
    {
        combo = 0;
        score = 0;
        LevelShowText.GetComponent<TMPro.TextMeshProUGUI>().text = "Level";
        LevelText.GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.currentLevelNumber.ToString();

        ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "0";
        MatchesText.GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.board.getAllHints().ToString();
    }

    public void OnChangeMatches()
    {
        int matches = GameManager.instance.board.getAllHints();
        MatchesText.GetComponent<TMPro.TextMeshProUGUI>().text = matches.ToString();
    }

    public void OnChangeLevel()
    {
        score = 0;
        combo = 0;
        currentBonus = 0;
        LevelShowText.GetComponent<TMPro.TextMeshProUGUI>().text = "Level";
        LevelText.GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.currentLevelNumber.ToString();
        ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "0";
        MatchesText.GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.board.getAllHints().ToString();
    }

    public void OnDailyChallenge(DateTime date)
    {
        score = 0;
        combo = 0;
        currentBonus = 0;
        LevelShowText.GetComponent<TMPro.TextMeshProUGUI>().text = "Date";
        LevelText.GetComponent<TMPro.TextMeshProUGUI>().text = date.ToString("MM/dd");
        ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = "0";
        MatchesText.GetComponent<TMPro.TextMeshProUGUI>().text = GameManager.instance.board.getAllHints().ToString();
    }

    public Tuple<int, int> OnMatchPoint()
    {
        combo++;
        int point = Point.comboStepPoint * Math.Min(combo, Point.comboPointEnd);
        int bonus = 0;
        if (combo >= Point.bonusStart && combo % Point.bonusStep == 0)
        {
            if (combo < Point.bonusMaxStart)
                bonus = Point.bonusPoint;
            else
                bonus = Point.bonusMax;
        }
        currentBonus = Point.basePoint + point + bonus;
        score += Point.basePoint + point + bonus;
        ScoreText.GetComponent<TMPro.TextMeshProUGUI>().text = score.ToString();
        return new Tuple<int, int>(point, bonus);
    }

    public void OnNoMatch()
    {
        combo = 0;
    }
    public int getScore()
    {
        return score;
    }
    public int getBonus()
    {
        return currentBonus;
    }


}