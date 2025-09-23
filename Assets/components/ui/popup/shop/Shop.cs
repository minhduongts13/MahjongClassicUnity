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
}