using System.Collections;
using UnityEngine;

public class LanternPopup : BasePopup
{
    public void OnJoin()
    {
        GameManager.instance.storageManager.setJoinedLantern(1);
        Hide();
        UIManager.ShowPageLantern(1);
    }
    public void Hide()
    {
        UIManager.HidePopup(Popup.LANTERN_INTRO);
    }
    
    
}