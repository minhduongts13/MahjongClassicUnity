using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public class Badges : BasePage
{
    public GameObject TabJourney;
    public GameObject TabDaily;
    public GameObject Journey;
    public GameObject Daily;
    public GameObject CalendarController;
    private bool isJourney = false;
    public void OnTrophy(int month)
    {
        var delta = month - DateTime.Today.Month;
        Debug.Log("Delta month: " + delta);
        CalendarController.GetComponent<CalendarController>().ChangeMonth(delta);
        Hide();
    }

    public void Show(int firstTime = 0)
    {
        FadeIn(this.gameObject, 0.5f).OnComplete(() =>
        {
            UIManager.ShowPage(Page.BADGES);
        });
    }
    public void Hide()
    {
        UIManager.ShowPage(Page.DAILY_CHALLENGE, false);
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

    public void ToggleTab()
    {
        isJourney = !isJourney;
        TabJourney.SetActive(isJourney);
        TabDaily.SetActive(!isJourney);
        Journey.SetActive(isJourney);
        Daily.SetActive(!isJourney);
    }

}