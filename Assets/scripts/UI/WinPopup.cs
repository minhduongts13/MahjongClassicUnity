using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Mono.Cecil.Cil;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] GameObject top;
    [SerializeField] GameObject mid;
    [SerializeField] GameObject bot;
    [SerializeField] GameObject box;
    [SerializeField] GameObject button;
    private Sequence blockSeq;
    private Sequence auraSeq;
 private bool canTapToReward = false;
     [SerializeField]  GameObject aura;
    public override void OnPopupShow(int curr = 0)
    {
        leaf.SetActive(false);
        button.SetActive(false);
        this.aura.SetActive(false);


        foreach (GameObject hi in ribbon)
        {
            hi.SetActive(false);
        }

        PopWell(async () =>
        {
            await DropwellnZoomAsync(() =>
            {
                par.SetActive(true);
                leaf.SetActive(true);
                if(GameManager.instance.currentLevel.levelNumber % 10 != 0) popBut();
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
    private void popBut()
    {
        DOTween.Kill(this.button.transform);
        this.button.transform.localScale = Vector3.zero;
        this.button.SetActive(true);
        this.button.transform.DOScale(new Vector3(0.5f, 0.5f, 0), 0.4f).SetEase(Ease.OutBack);

    }
    private async Task DropwellnZoomAsync(System.Action onComplete = null)
    {
        this.bg.SetActive(true);
        this.welldone.SetActive(true);
        this.glow.SetActive(true);

        this.level.gameObject.GetComponent<TextMeshProUGUI>().text =
            "Level " + (GameManager.instance.currentLevel.levelNumber + 1).ToString();

        this.score.gameObject.GetComponent<TextMeshProUGUI>().text =
            GameManager.instance.pointManager.getScore().ToString();

        this.welldone.transform.localScale = new Vector3(1.4f, 1.4f, 0);
        this.glow.transform.localScale = Vector3.zero;
        this.bg.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(this.welldone.transform
            .DOScale(new Vector3(0.8f, 0.8f, 0), 0.4f));
        seq.Join(this.bg.transform
            .DOScale(new Vector3(1.1f, 1.1f, 0), 0.4f));
        seq.Join(this.glow.transform.DOScale(new Vector3(1.1f, 1.1f, 0), 0.4f));

        seq.AppendCallback(async () =>
        {
            this.scoreText.SetActive(true);

            Task fanTask = popFan();
            Task flowerTask = popFlower();

            await Task.WhenAll(fanTask, flowerTask);
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

        for (int i = 0; i < rubbon.Length; i++)
        {
            GameObject hi = rubbon[i];
            hi.SetActive(true);
            hi.transform.localScale = Vector3.zero;

            var scaleTween = hi.transform
                .DOScale(Vector3.one, 1.03f)
                .SetEase(Ease.OutBack, i < 2 ? 5f : 2f);

            if (i < 2)
            {
                seq.Join(scaleTween);
            }
            else
            {
                seq.Insert(0.13f, scaleTween);
            }
        }

        await seq.AsyncWaitForCompletion();
    }

    private async Task popFan()
    {
        List<Task> animationTasks = new List<Task>();
        this.score.transform.localScale = Vector3.zero;

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
                    .SetEase(Ease.InSine);
                })
                .AsyncWaitForCompletion();

            animationTasks.Add(scaleTask);
        }
        score.gameObject.SetActive(true);
        var hi = score.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.5f)
                .SetEase(Ease.OutBack, 4).AsyncWaitForCompletion();
        animationTasks.Add(hi);
        animationTasks.Add(popAllRibbon());
        await Task.WhenAll(animationTasks);
        foreach (GameObject cho in ribbon)
        {
            cho.SetActive(true);
        }
        fillProgressBar();
    }
    void Update()
    {
        if (canTapToReward)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame || Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
            {
                UIManager.ShowPopup(Popup.Reward,true,1,false);
                canTapToReward = false;
            }
        }
    }
    private void Jigglebox()
{
    blockSeq.Kill();
    this.box.SetActive(true);
    blockSeq = DOTween.Sequence();
    blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, -15), 0.15f).SetEase(Ease.OutSine));
    blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, 12), 0.12f).SetEase(Ease.InOutSine));
    blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, -8), 0.10f).SetEase(Ease.InOutSine));
    blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, 5), 0.08f).SetEase(Ease.InOutSine));
    blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, -2), 0.06f).SetEase(Ease.InOutSine));
    blockSeq.Append(this.box.transform.DORotate(Vector3.zero, 0.4f).SetEase(Ease.InSine));
    blockSeq.SetLoops(-1, LoopType.Restart); 
    
   
}
    private async Task popFlower()
    {
        List<Task> animationTasks = new List<Task>();

        for (int i = 0; i < flower.Length; i++)
        {
            GameObject flowerObj = flower[i];
            flowerObj.SetActive(true);
            flowerObj.transform.localRotation = Quaternion.identity;
            flowerObj.transform.localScale = Vector3.zero;

            float rotationAngle = 0f;
            if (i == 0 || i == 1)
            {
                rotationAngle = -25f;
            }
            else if (i == 2)
            {
                rotationAngle = 25f;
            }

            Sequence flowerSeq = DOTween.Sequence();

            flowerSeq.Join(flowerObj.transform.DOScale(Vector3.one, 0.8f)
                .SetEase(Ease.OutBack));

            flowerSeq.Join(flowerObj.transform.DORotate(
                new Vector3(0, 0, rotationAngle), 3.0f)
                .SetEase(Ease.OutSine));

            var flowerTask = flowerSeq.AsyncWaitForCompletion();
            animationTasks.Add(flowerTask);
        }

        await Task.WhenAll(animationTasks);
    }
    private void fillProgressBar()
    {
        resetProgress();
        if (GameManager.instance.currentLevel.levelNumber % 10 == 1)
        {
            showLeft();
            showRight();
            return;
        }
        else
        {
            fakeShow(GameManager.instance.currentLevel.levelNumber - 1);
            showMid();

        }
    }
    private void showLeft()
    {
        this.top.SetActive(true);

    }
    private void showRight()
    {
        this.bot.SetActive(true);
        this.mid.SetActive(false);
        this.bot.transform.localScale = new Vector3(0, 1, 0);
        this.bot.transform.DOScale(new Vector3(1, 1, 0), 0.5f);

    }
    private void showMid()
    {
        this.mid.SetActive(true);
        this.bot.SetActive(true);
        RectTransform rt1 = bot.transform as RectTransform;
        this.mid.transform.DOScale(this.mid.transform.localScale + new Vector3(2.2f, 0, 0), 0.5f).OnUpdate(() =>
        {
            rt1.anchoredPosition = new Vector2(this.mid.transform.localScale.x * 21, 0);
        }).OnComplete(() =>
        {             
            if (GameManager.instance.currentLevel.levelNumber % 10 == 0)
            {
                this.Jigglebox();
                rotateaura();
                canTapToReward = true;
            }
        });
    }
    private void fakeShow(int level)
    {
        showLeft();
        this.mid.SetActive(true);
        int hehe = level % 10;
        this.mid.transform.localScale = new Vector3((hehe * 22) / 10, 1, 0);
        RectTransform rt1 = bot.transform as RectTransform;
        rt1.anchoredPosition = this.mid.transform.localScale * 21;
    }
    private void resetProgress()
    {
        this.top.SetActive(true);
        this.mid.SetActive(false);
        this.bot.SetActive(true);
        this.top.transform.localScale = new Vector3(1, 1, 0);
        this.mid.transform.localScale = new Vector3(1, 1, 0);
        this.bot.transform.localScale = new Vector3(1, 1, 0);
        RectTransform rt1 = bot.transform as RectTransform;
        rt1.anchoredPosition = new Vector3(0, 0, 0);
        RectTransform rt2 = mid.transform as RectTransform;
        rt2.anchoredPosition = new Vector3(0, 0, 0);
    }


private void rotateaura()
{
    this.aura.SetActive(true);
    
    auraSeq?.Kill();
    
    auraSeq = DOTween.Sequence();
        auraSeq.Append(aura.transform.DOLocalRotate(
            new Vector3(0, 0, 360),
            2f,
            RotateMode.FastBeyond360)
            .SetEase(Ease.Linear));
    auraSeq.SetLoops(-1, LoopType.Restart);
}
}
 