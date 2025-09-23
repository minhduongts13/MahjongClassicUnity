using UnityEngine;

public  class DailyPopUp : BasePopup
{
     public override void OnPopupShow(int curr = 0, Vector3 pos = default) 
    { 
    }

    public override void OnPopupHide() 
    { 
    }

    public override void OnPopupDestroy() 
    { 
    }

    protected override void ClosePopup() 
    { 
        if (UIManager.Instance != null)
        {
            var popupName = this.gameObject.name;
            if (System.Enum.TryParse<Popup>(popupName, out Popup popupType))
            {
                UIManager.HidePopup(popupType);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}