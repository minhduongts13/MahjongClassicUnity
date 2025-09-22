using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonClickEffect : MonoBehaviour, 
    IPointerClickHandler
{
    private Vector3 originalScale;
    private Vector3 hoverScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        this.gameObject.transform.DOScale(originalScale * 0.9f, 0.1f)
        .OnComplete(() => {
            DOVirtual.DelayedCall(0.2f, () => {
                this.gameObject.transform.DOScale(originalScale, 0.1f);
            });
        });
        transform.localScale =  originalScale;
    }
}
