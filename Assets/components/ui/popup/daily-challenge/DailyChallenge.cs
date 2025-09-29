using UnityEngine;
using DG.Tweening;
using System.Collections;
public class DailyChallenge : BasePage
{
    public CalendarController CalendarController;
    public void OnBadge()
    {
        UIManager.ShowPageBadges();
    }

    public void Show(int firstTime = 0)
    {
        FadeIn(this.gameObject, 0.5f).OnComplete(() =>
        {
            UIManager.ShowPage(Page.DAILY_CHALLENGE);
            CalendarController.Setup();
        });
    }
    public void Hide()
    {
        Debug.Log("Hiding Daily Challenge and returning to Dashboard.");
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

    public void OnPlay()
    {
        var selectedDate = CalendarController.selectedDayCell.date;

        UIManager.DailyChallengePlay(selectedDate);
    }

}