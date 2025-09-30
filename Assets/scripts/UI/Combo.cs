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
    private Sequence currentSequence;
    private Sequence currentFadeSequence;



    public void addCombo()
    {
        Debug.Log("addcombo" + comboCount);
        GameManager.instance.missionManager.UpdateMissionProgress(1, 1);
        GameManager.instance.missionManager.UpdateMissionProgress(4, 1);
        GameManager.instance.missionManager.UpdateMissionProgress(7, 1);
        comboCount++;
        if (comboCount >= 3)
        {
            showCombo();
        }
    }

    public void showCombo()
    {
        this.gameObject.SetActive(true);

        // Kill sequence cũ
        currentSequence?.Kill();
        currentFadeSequence?.Kill();

        if (comboCount <= 9)
        {
            number1.SetActive(true);
            number2.SetActive(false);
            combo.SetActive(true);

            number1.transform.localScale = Vector3.zero;
            combo.transform.localScale = Vector3.zero;

            number1.GetComponent<CanvasGroup>().alpha = 1f;
            combo.GetComponent<CanvasGroup>().alpha = 1f;

            number1.GetComponent<Image>().sprite = sprite[comboCount];
            number1.GetComponent<RectTransform>().sizeDelta =
                new Vector2(sprite[comboCount].rect.width, sprite[comboCount].rect.height);

            // Tạo sequence mới và lưu lại
            currentSequence = DOTween.Sequence();
            currentSequence.Insert(0, number1.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic));
            currentSequence.Insert(0, combo.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack));

            currentSequence.OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    currentFadeSequence = DOTween.Sequence();
                    currentFadeSequence.Insert(0, combo.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    currentFadeSequence.Insert(0, number1.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));

                    currentFadeSequence.OnComplete(() =>
                    {
                        combo.SetActive(false);
                        number1.SetActive(false);
                    });
                });
            });
        }
        else
        {
            number1.SetActive(true);
            number2.SetActive(true);
            combo.SetActive(true);

            number1.transform.localScale = Vector3.zero;
            number2.transform.localScale = Vector3.zero;
            combo.transform.localScale = Vector3.zero;

            number1.GetComponent<CanvasGroup>().alpha = 1f;
            number2.GetComponent<CanvasGroup>().alpha = 1f;
            combo.GetComponent<CanvasGroup>().alpha = 1f;

            int tensDigit = comboCount / 10;
            int onesDigit = comboCount % 10;

            number1.GetComponent<Image>().sprite = sprite[tensDigit];
            number1.GetComponent<RectTransform>().sizeDelta =
                new Vector2(sprite[tensDigit].rect.width, sprite[tensDigit].rect.height);

            number2.GetComponent<Image>().sprite = sprite[onesDigit];
            number2.GetComponent<RectTransform>().sizeDelta =
                new Vector2(sprite[onesDigit].rect.width, sprite[onesDigit].rect.height);

            // Sequence mới
            currentSequence = DOTween.Sequence();
            currentSequence.Insert(0, number1.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic));
            currentSequence.Insert(0, number2.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutElastic));
            currentSequence.Insert(0, combo.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));

            currentSequence.OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    currentFadeSequence = DOTween.Sequence();
                    currentFadeSequence.Insert(0, combo.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    currentFadeSequence.Insert(0, number1.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));
                    currentFadeSequence.Insert(0, number2.GetComponent<CanvasGroup>().DOFade(0f, 0.3f));

                    currentFadeSequence.OnComplete(() =>
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