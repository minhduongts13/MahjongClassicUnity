using System;
using UnityEngine;

public class StorageManager : MonoBehaviour
{

    void Start()
    {

    }
    public int getJoinedLantern()
    {
        return PlayerPrefs.GetInt("joinedLantern", 0);
    }
    public int getCurrentLevel()
    {
        return PlayerPrefs.GetInt("currentLevel", 1);
    }

    public THEME_STYLE getChosenTheme()
    {
        int theme = PlayerPrefs.GetInt("theme", (int)THEME_STYLE.Classic);
        return (THEME_STYLE)theme;
    }

    public BACKGROUND getChosenBackground()
    {
        int bg = PlayerPrefs.GetInt("background", (int)BACKGROUND.BG1);
        return (BACKGROUND)bg;
    }

    public int getNumberHints()
    {
        return PlayerPrefs.GetInt("hints", 0);
    }

    public int getNumberShuffles()
    {
        return PlayerPrefs.GetInt("shuffles", 0);
    }

    public bool hasPlayedDay(DateTime day)
    {
        return PlayerPrefs.GetInt(day.Date.ToString("yyyy-MM-dd"), 0) == 1;
    }
    /************** Setters ******************/
    public void setCurrentLevel(int level)
    {
        PlayerPrefs.SetInt("currentLevel", level);
        PlayerPrefs.Save();
    }

    public void setChosenTheme(THEME_STYLE theme)
    {
        PlayerPrefs.SetInt("theme", (int)theme);
        PlayerPrefs.Save();
    }

    public void setChosenBackground(BACKGROUND bg)
    {
        PlayerPrefs.SetInt("background", (int)bg);
        PlayerPrefs.Save();
    }

    public void setNumberHints(int hints)
    {
        PlayerPrefs.SetInt("hints", hints);
        PlayerPrefs.Save();
    }

    public void setNumberShuffles(int shuffles)
    {
        PlayerPrefs.SetInt("shuffles", shuffles);
        PlayerPrefs.Save();
    }

    public void setJoinedLantern(int lantern)
    {
        PlayerPrefs.SetInt("joinedLantern", lantern);
        PlayerPrefs.Save();
    }
    
    public void setPlayedDay(DateTime day)
    {
        PlayerPrefs.SetInt(day.Date.ToString("yyyy-MM-dd"), 1);
        PlayerPrefs.Save();
    }
}