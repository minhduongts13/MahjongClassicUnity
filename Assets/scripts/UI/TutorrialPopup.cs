using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Tutorial : BasePopup
{

    public void Hide()
    {
        UIManager.HidePopup(Popup.Tutorial);
    }

    public override async void OnPopupShow(int curr)
    {   
        gameObject.transform.localScale = Vector3.zero;
                this.gameObject.SetActive(true);

        float screenX = GameManager.instance.board.anchor.rect.width;
        float screenY = GameManager.instance.board.anchor.rect.height;

        float scaleFactor = screenX / 1080;
        float scaleY = screenY / 2000;
        scaleFactor = Math.Min(scaleY, scaleFactor);
        this.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = GameManager.instance.board.GetTutorialName(curr);
        await gameObject.transform.DOScale(Vector3.one * scaleFactor, 0.2f).SetEase(Ease.OutBack).AsyncWaitForCompletion();
    }
}