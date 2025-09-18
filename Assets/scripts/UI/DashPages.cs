using UnityEngine;

public  class DashPages : BasePage
{
    public override void OnPageShow(int curr = 0) 
    { 
    }

    public override void OnPageHide() 
    { 
    }

    public override void OnPageDestroy() 
    { 
    }

    protected override void ClosePage() 
    { 
        if (UIManager.Instance != null)
        {
            var popupName = this.gameObject.name;
            if (System.Enum.TryParse<Page>(popupName, out Page popupType))
            {
                UIManager.HidePage(popupType);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}