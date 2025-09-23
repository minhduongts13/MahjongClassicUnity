using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Shop : BasePopup
{

    public void Hide()
    {
        UIManager.HidePopup(Popup.SHOP);
    }

    public void OnBuy1()
    {
        int hints = GameManager.instance.storageManager.getNumberHints();
        hints += 1;
        GameManager.instance.storageManager.setNumberHints(hints);
    
        int Shuffles = GameManager.instance.storageManager.getNumberShuffles();
        Shuffles += 1;
        GameManager.instance.storageManager.setNumberShuffles(Shuffles);

    }
}