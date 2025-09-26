using UnityEngine;
using DG.Tweening;

public class TapToContinue : MonoBehaviour
{
    public GameObject Overlay;
    public GameObject Title;
    public GameObject WinLevel;
    public GameObject Arrow1;
    public GameObject CollectLantern;
    public GameObject Arrow2;
    public GameObject ImproveRanking;
    public GameObject TapToContinueText;

    public void Open()
    {
        this.gameObject.SetActive(true);

        float fadeDuration = 0.3f; // thời gian fade
        float delay = 0.1f;        // khoảng cách giữa các object

        Sequence seq = DOTween.Sequence();

        seq.Join(FadeIn(Overlay, 0.4f));
        seq.Join(FadeIn(Title, 0.4f));
        seq.Join(FadeIn(WinLevel, 0.4f));
        seq.AppendInterval(delay);

        seq.Append(FadeIn(Arrow1, fadeDuration));
        seq.AppendInterval(delay);

        seq.Append(FadeIn(CollectLantern, fadeDuration));
        seq.AppendInterval(delay);

        seq.Append(FadeIn(Arrow2, fadeDuration));
        seq.AppendInterval(delay);

        seq.Append(FadeIn(ImproveRanking, fadeDuration));
        seq.AppendInterval(delay);

        seq.Append(FadeIn(TapToContinueText, fadeDuration));
    }

    private Tween FadeIn(GameObject obj, float duration)
    {
        var canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }

        obj.SetActive(true);
        canvasGroup.alpha = 0f;

        return canvasGroup.DOFade(1f, duration).SetEase(Ease.InOutQuad);
    }

    public void FadeOut()
    {
        var cg = GetComponent<CanvasGroup>();
        cg.DOKill();
        if (cg == null)
        {
            cg = this.gameObject.AddComponent<CanvasGroup>();
        }
        float duration = 0.5f;
        cg.DOFade(0f, duration).SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                cg.alpha = 1f;
            });
    }
}