using TMPro;
using UnityEngine;

public class Lose : BasePopup
{
    [SerializeField] TMP_Text CountShuffle;
    public GameObject ShuffleButton;

    public void Show()
    {

        string numShuff = GameManager.instance.storageManager.getNumberShuffles().ToString();
        Debug.Log(CountShuffle);
        if (numShuff == "0")
        {
            ShuffleButton.SetActive(false);
        }
        else
        {
            ShuffleButton.SetActive(true);
            CountShuffle.text = numShuff;

        }

    }
    public async void OnShuffle()
    {
        Hide();
        if (GameManager.instance != null && GameManager.instance.board != null)
        {
            GameManager.instance.Shuffle();
        }
    }

    public async void OnRestart()
    {
        Hide();
        if (GameManager.instance != null)
        {
            await GameManager.instance.Reload();
        }
    }
    public void Hide()
    {
        UIManager.HidePopup(Popup.LOSE);
    }
}