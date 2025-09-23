using UnityEngine;

public abstract class BasePage : MonoBehaviour
{
    public virtual void OnPageShow(int curr = 0) 
    { 
    }

    public virtual void OnPageHide() 
    { 
    }

    public virtual void OnPageDestroy()
    { 
        
    }

    protected virtual void ClosePage() 
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