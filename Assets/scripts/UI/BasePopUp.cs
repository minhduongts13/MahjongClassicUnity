using UnityEngine;

public abstract class BasePopup : MonoBehaviour
{
     public virtual void OnPopupShow(int curr = 0, Vector3 pos = default) 
    { 
    }

    public virtual void OnPopupHide() 
    { 
    }

    public virtual void OnPopupDestroy() 
    { 
    }

    protected virtual void ClosePopup() 
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