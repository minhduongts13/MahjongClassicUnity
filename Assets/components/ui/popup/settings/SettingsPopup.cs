using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SettingsPopup : BasePopup
{
    public GameObject[] SoundButton;
    public GameObject[] MusicButton;
    public GameObject[] HighlightButton;
    private int soundState = (int)Button.ON;
    private int musicState = (int)Button.ON;
    private int highlightState = (int)Button.ON;


    public void toggleSound()
    {
        soundState = 1 - soundState;
        SoundButton[soundState].SetActive(true);
        SoundButton[1 - soundState].SetActive(false);
    }
    public void toggleMusic()
    {
        musicState = 1 - musicState;
        MusicButton[musicState].SetActive(true);
        MusicButton[1 - musicState].SetActive(false);
    }
    public void toggleHighlight()
    {
        highlightState = 1 - highlightState;
        HighlightButton[highlightState].SetActive(true);
        HighlightButton[1 - highlightState].SetActive(false);
    }
}