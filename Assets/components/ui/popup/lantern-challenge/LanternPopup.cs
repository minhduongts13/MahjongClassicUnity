using UnityEngine;
using System.Collections;

public class LanternPopup : BasePopup
{
    public void OnJoin()
    {
        GameManager.instance.storageManager.setJoinedLantern(1);
        Hide();
        UIManager.ShowPageLantern();
    }
    public void Hide()
    {
        UIManager.HidePopup(Popup.LANTERN_INTRO);
    }
    
    
}