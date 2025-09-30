using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class DailyReward : BasePopup
{

    public void Hide()
    {
        UIManager.HidePopup(Popup.DAILY_REWARD);
    }

    public override void OnPopupShow(int curr)
    {

    }
    public void redBox()
    {
        UIManager.ShowPopup(Popup.Reward, true, 2, false);
    }
    public void blueBox()
    {
        UIManager.ShowPopup(Popup.Reward, true, 4, false);
    }
    public void greenBox()
    {
        UIManager.ShowPopup(Popup.Reward, true, 3, false);
    }
}