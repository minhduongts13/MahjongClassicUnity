using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;



public  class WrongMatch : BasePopup
{
  
   public override void OnPopupShow(int curr = 0, Vector3 pos = default)
{
    gameObject.transform.DOKill();
    CanvasGroup canvasGroup = gameObject.GetComponent<CanvasGroup>();
    if (canvasGroup != null)
    {
        canvasGroup.DOKill();
    }
    
    gameObject.transform.localPosition = pos;
    
    TextMeshProUGUI textComponent = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    if (curr == 1)
    {
        textComponent.text = "Locked by above tiles";
    }
    else if(curr==0)
    {
        textComponent.text = "Locked by left and right";
    }
    
    if (canvasGroup == null)
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    canvasGroup.alpha = 1f;
    
    Sequence sequence = DOTween.Sequence();
    sequence.Append(gameObject.transform.DOLocalMove(new Vector3(pos.x, pos.y + 300, 0), 1f));
    sequence.Join(canvasGroup.DOFade(0f, 1f));
    sequence.OnComplete(() => {
        gameObject.SetActive(false);
    });
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