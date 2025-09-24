using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;

public class WinPopup : BasePopup
{
    [SerializeField] GameObject welldone;
    [SerializeField] GameObject bg;
    [SerializeField] GameObject par;
    [SerializeField] GameObject glow;
    [SerializeField] GameObject[] ribbon;
    [SerializeField] GameObject[] rubbon;
    [SerializeField] GameObject[] fan;
    [SerializeField] GameObject[] flower;

    [SerializeField] GameObject scoreText;
    [SerializeField] GameObject score;
    [SerializeField] GameObject level;
    [SerializeField] GameObject leaf;



    public override void OnPopupShow(int curr = 0)
    {
        leaf.SetActive(false);

        foreach (GameObject hi in ribbon)
        {
            hi.SetActive(false);
        }

        PopWell(async () =>
        {
            await DropwellnZoomAsync(async () =>
            {
                await Task.WhenAll( popFlower());
                par.SetActive(true);
                leaf.SetActive(true);
            });
        });
    }


    protected override void ClosePopup()
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

    private void PopWell(Action onComplete = null)
    {
        onComplete?.Invoke();
    }
    private async Task DropwellnZoomAsync(System.Action onComplete = null)
{
    this.bg.SetActive(true);
    this.welldone.SetActive(true);
    this.scoreText.SetActive(true);
    this.glow.SetActive(true);

    this.level.gameObject.GetComponent<TextMeshProUGUI>().text =
        "Level " + (GameManager.instance.currentLevel.levelNumber + 1).ToString();

    this.score.gameObject.GetComponent<TextMeshProUGUI>().text =
        GameManager.instance.pointManager.getScore().ToString();

    this.welldone.transform.localScale = new Vector3(1.4f, 1.4f, 0);
    this.score.transform.localScale = Vector3.zero;
    this.glow.transform.localScale = Vector3.zero;
    this.bg.transform.localScale = Vector3.zero;
    Sequence seq = DOTween.Sequence();

    seq.Append(this.welldone.transform
        .DOScale(new Vector3(0.8f, 0.8f, 0), 0.4f));
    seq.Join(this.bg.transform
        .DOScale(new Vector3(1.1f, 1.1f, 0), 0.4f));

    seq.AppendCallback(async () =>
    {
        await popFan(); 
    });

    seq.Append(this.welldone.transform
        .DOScale(new Vector3(1, 1, 0), 0.4f));
    seq.Join(this.bg.transform
        .DOScale(new Vector3(1, 1, 0), 0.4f));

    await seq.AsyncWaitForCompletion();
    onComplete?.Invoke();
}

    private async Task popAllRibbon()
{
    Sequence seq = DOTween.Sequence();

        for (int i = 0; i < 2; i++)
        {
            GameObject hi = rubbon[i];
            hi.SetActive(true);
            hi.transform.localScale = Vector3.zero;

            seq.Join(
                hi.transform.DOScale(Vector3.one, 1.03f)
                .SetEase(Ease.OutBack,5f)
            );
        
    }
            for (int i = 2; i < 4; i++)
            {
                GameObject hi = rubbon[i];
                hi.SetActive(true);
                hi.transform.localScale = Vector3.zero;
                seq.Insert(0.13f,
                        hi.transform.DOScale(Vector3.one, 1.03f)
                            .SetEase(Ease.OutBack,3f)
                       );

            }
            
    await seq.AsyncWaitForCompletion();
}

    private async Task popFan()
    {
        List<Task> animationTasks = new List<Task>();

        for (int i = 0; i < fan.Length; i++)
        {
            GameObject fanObj = fan[i];
            fanObj.SetActive(true);

            Vector3 originalPos = fanObj.transform.localPosition;
            Vector3 startPos = originalPos;
            if (i == 0)
            {
                startPos.x += 20f;
            }
            else if (i == 1)
            {
                startPos.x -= 20f;
            }

            fanObj.transform.localPosition = startPos;
            fanObj.transform.localScale = Vector3.zero;

            var scaleTask = fanObj.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.3f)
                .SetEase(Ease.OutBack)

                .OnComplete(() =>
                {

                    var moveTask = fanObj.transform.DOLocalMove(originalPos, 0.3f)
                    .SetEase(Ease.OutBack);
                })
                .AsyncWaitForCompletion();


            animationTasks.Add(scaleTask);
        }

        await Task.WhenAll(animationTasks);
        await popAllRibbon();
    }
    private async Task popFlower()
    {
        List<Task> animationTasks = new List<Task>();

        for (int i = 0; i < flower.Length; i++)
        {
            GameObject flowerObj = flower[i];
            flowerObj.SetActive(true);
            flowerObj.transform.localRotation = Quaternion.identity;

            float rotationAngle = 0f;
            if (i == 0 || i == 1)
            {
                rotationAngle = -25f;
            }
            else if (i == 2)
            {
                rotationAngle = 25f;
            }

            var rotateTask = flowerObj.transform.DORotate(
                new Vector3(0, 0, rotationAngle), 1.0f)
                .SetEase(Ease.OutSine)
                .AsyncWaitForCompletion();

            animationTasks.Add(rotateTask);
        }
        foreach (GameObject hi in ribbon)
        {
            if (!hi.activeSelf)
            {
                hi.SetActive(true);
            }
        }

        await Task.WhenAll(animationTasks);
    }
}