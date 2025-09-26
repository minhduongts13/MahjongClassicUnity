using UnityEngine;
using DG.Tweening;
using System.Collections;
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

    public void Show(int firstTime = 0)
    {
        FadeIn(this.gameObject, 0.5f).OnComplete(() =>
        {
            UIManager.ShowPage(Page.LANTERN_CHALLENGE);
        });
    }
    public void Hide()
    {
        UIManager.ShowPage(Page.DASHBOARD, false);
        FadeOut();
    }

    private Tween FadeIn(GameObject obj, float duration)
    {
        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        obj.SetActive(true);
        canvasGroup.alpha = 0f;

        return canvasGroup.DOFade(1f, duration).SetEase(Ease.InOutQuad);
    }

    public void FadeOut()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.DOKill();
        if (cg == null)
        {
            cg = this.gameObject.AddComponent<CanvasGroup>();
        }
        float duration = 0.5f;
        cg.DOFade(0f, duration).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                cg.alpha = 1f;
            });
    }

}