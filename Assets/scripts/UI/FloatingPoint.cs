using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloatingPoint : MonoBehaviour
{
    private RectTransform rectTransform;
    private CanvasGroup hi;
    private Sequence blockSeq;
    private TextMeshProUGUI text;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        hi = GetComponent<CanvasGroup>();
        text = GetComponent<TextMeshProUGUI>();
    }
    public Vector2 GetMidPoint(Vector2 pos1, Vector2 pos2)
    {
        return (pos1 + pos2) / 2f;
    }
    public async void Showpoint(Tile tile1, Tile tile2)
    {
        if (hi == null)
        {
            hi = GetComponent<CanvasGroup>();
        }
        hi.alpha = 1;
        if (text == null)
        {
            text = GetComponent<TextMeshProUGUI>();
            if (text == null)
            {
                Debug.LogError("TextMeshProUGUI component not found");
                return;
            }
        }

        text.text = "+" + GameManager.instance.pointManager.getBonus().ToString();
        RectTransform rt1 = tile1.transform as RectTransform;
        RectTransform rt2 = tile2.transform as RectTransform;



        Vector2 mid = GetMidPoint(rt1.anchoredPosition, rt2.anchoredPosition);
        if (blockSeq != null && blockSeq.IsActive()) blockSeq.Kill();
        Vector2 startPos = mid + new Vector2(0, 50);

        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        rectTransform.anchoredPosition = startPos;
        gameObject.SetActive(true);
        blockSeq = DOTween.Sequence();
        blockSeq.Append(rectTransform.DOAnchorPos(startPos + new Vector2(0, 100), 0.5f).SetEase(Ease.OutCubic));

        blockSeq.Join(hi.DOFade(0, 1f));
        blockSeq.OnComplete(() => gameObject.SetActive(false));
        await blockSeq.AsyncWaitForCompletion();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
