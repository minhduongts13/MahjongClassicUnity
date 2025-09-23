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
        combo.SetActive(true);
        if (comboCount <= 9)
        {
            DOTween.Kill(number1.transform);
            DOTween.Kill(number2.transform);
            number1.SetActive(true);
            number2.SetActive(false);
            number1.transform.localScale = Vector3.zero;
            number1.GetComponent<Image>().sprite = sprite[comboCount];
            number1.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite[comboCount%10 ].rect.width, sprite[comboCount%10].rect.height);
            number1.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        combo.SetActive(false);
                        number1.SetActive(false);
                    });
                });
        }
        else
        {
            DOTween.Kill(number1.transform);
            DOTween.Kill(number1.transform);
            number1.SetActive(true);
            number2.SetActive(true);
            number1.transform.localScale = Vector3.zero;
            number2.transform.localScale = Vector3.zero;
            number1.GetComponent<Image>().sprite = sprite[comboCount / 10];
            number1.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite[comboCount%10 ].rect.width, sprite[comboCount%10 ].rect.height);
            int ha = comboCount % 10;
            Debug.Log("hehe"+ha);
            number2.GetComponent<Image>().sprite = sprite[ha];
           number2.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite[comboCount%10].rect.width, sprite[comboCount%10].rect.height);
            Sequence sequence = DOTween.Sequence();

sequence.Insert(0, number1.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f).SetEase(Ease.OutBack));
sequence.Insert(0, number2.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f).SetEase(Ease.OutBack));
sequence.OnComplete(() =>
{
    DOVirtual.DelayedCall(1f, () =>
    {
        combo.SetActive(false);
        number1.SetActive(false);
        number2.SetActive(false);
    });
});

        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
    }
}