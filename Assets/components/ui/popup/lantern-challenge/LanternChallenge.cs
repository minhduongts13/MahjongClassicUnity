using UnityEngine;
using DG.Tweening;
public class LanternChallenge : BasePage
{
    public GameObject TapToContinue;
    public void OnQuestion()
    {
        var tapToContinue = TapToContinue.GetComponent<TapToContinue>();
        if (tapToContinue != null)
        {
            Debug.Log("Starting TapToContinue animation.");
            tapToContinue.Open();
        }
        else
        {
            Debug.LogError("TapToContinue component not found on the TapToContinue GameObject.");
        }
    }
    
    public void Show()
    {
        
    }
    public void Hide()
    {

        UIManager.ShowPage(Page.DASHBOARD);
    }
}