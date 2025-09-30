using UnityEngine;

public class Lose : BasePopup
{
    public GameObject CountShuffle;
    public GameObject ShuffleButton;

    public void Show()
    {

        var numShuff = GameManager.instance.storageManager.getNumberShuffles().ToString();
        CountShuffle.GetComponent<TMPro.TMP_Text>().text = numShuff;
        if (numShuff == "0")
        {
            ShuffleButton.SetActive(false);
        }
        else
        {
            ShuffleButton.SetActive(true);
        }
    }
    public async void OnShuffle()
    {
        Hide();
        if (GameManager.instance != null && GameManager.instance.board != null)
        {
            await GameManager.instance.board.Shuffle();
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