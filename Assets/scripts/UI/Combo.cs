using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Combo : MonoBehaviour
{
    public GameObject number1;
    public GameObject number2;
    public GameObject combo;
    public List<Sprite> sprite = new List<Sprite>();
    public int comboCount = 0;

    

    public  void addCombo()
    {
        Debug.Log("addcombo" + comboCount);
        comboCount++;
        if (comboCount >= 3)
        {
            showCombo();
        }
    }

    public void showCombo()
    {
        this.gameObject.SetActive(true);
        
        if (comboCount <= 9)
        {
            DOTween.Kill(number1.transform);
            DOTween.Kill(number2.transform);
            DOTween.Kill(combo.transform);
            
            DOTween.Kill(number1.GetComponent<CanvasGroup>());
            DOTween.Kill(combo.GetComponent<CanvasGroup>());
            
            number1.SetActive(true);
            number2.SetActive(false);
            combo.transform.localScale = Vector3.zero;
            combo.SetActive(true);
            number1.transform.localScale = Vector3.zero;
            
            number1.GetComponent<CanvasGroup>().alpha = 1f;
            combo.GetComponent<CanvasGroup>().alpha = 1f;
            
            Sequence sequence = DOTween.Sequence();
            number1.GetComponent<Image>().sprite = sprite[comboCount];
            number1.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite[comboCount % 10].rect.width, sprite[comboCount % 10].rect.height);
            sequence.Insert(0, number1.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f).SetEase(Ease.OutElastic));
            sequence.Insert(0, combo.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f).SetEase(Ease.OutBack));
            
            sequence.OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    Sequence fadeSequence = DOTween.Sequence();
                    fadeSequence.Insert(0, combo.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    fadeSequence.Insert(0, number1.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    
                    fadeSequence.OnComplete(() =>
                    {
                        combo.SetActive(false);
                        number1.SetActive(false);
                    });
                });
            });
        }
        else
        {
            DOTween.Kill(number1.transform);
            DOTween.Kill(number2.transform);
            DOTween.Kill(combo.transform);
            DOTween.Kill(number1.GetComponent<CanvasGroup>());
            DOTween.Kill(number2.GetComponent<CanvasGroup>());
            DOTween.Kill(combo.GetComponent<CanvasGroup>());
            
            number1.SetActive(true);
            number2.SetActive(true);
            combo.transform.localScale = Vector3.zero;
            combo.SetActive(true);
            number1.transform.localScale = Vector3.zero;
            number2.transform.localScale = Vector3.zero;
            
            number1.GetComponent<CanvasGroup>().alpha = 1f;
            number2.GetComponent<CanvasGroup>().alpha = 1f;
            combo.GetComponent<CanvasGroup>().alpha = 1f;
            
            int tensDigit = comboCount / 10;
            int onesDigit = comboCount % 10;
            
            number1.GetComponent<Image>().sprite = sprite[tensDigit];
            number1.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite[tensDigit].rect.width, sprite[tensDigit].rect.height);
            number2.GetComponent<Image>().sprite = sprite[onesDigit];
            number2.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite[onesDigit].rect.width, sprite[onesDigit].rect.height);
            
            Sequence sequence = DOTween.Sequence();
            sequence.Insert(0, number1.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f).SetEase(Ease.OutElastic));
            sequence.Insert(0, number2.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f).SetEase(Ease.OutElastic));
            sequence.Insert(0, combo.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f).SetEase(Ease.OutBack));
            
            sequence.OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    Sequence fadeSequence = DOTween.Sequence();
                    fadeSequence.Insert(0, combo.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    fadeSequence.Insert(0, number1.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    fadeSequence.Insert(0, number2.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    
                    fadeSequence.OnComplete(() =>
                    {
                        combo.SetActive(false);
                        number1.SetActive(false);
                        number2.SetActive(false);
                    });
                });
            });
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
    }
}