using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Reward : BasePopup
{
    [SerializeField] GameObject box;
    [SerializeField] GameObject lip;
    [SerializeField] GameObject reward;
    [SerializeField] GameObject reward1;

    [SerializeField] GameObject glow;
    [SerializeField] GameObject par;
    [SerializeField] GameObject par1;
    [SerializeField] GameObject butt;
    [SerializeField] GameObject aura;
    [SerializeField] GameObject aura1;




    private Sequence lidSeq;
    private Sequence blockSeq;
    private Sequence he;

    float lidMoveDuration = 0.77f;
    float lidRotationDuration = 0.73f;
    Ease lidMoveEase = Ease.OutBack;
    Ease lidRotationEase = Ease.InOutQuart;
    float fadeDuration = 0.5f;
    Ease fadeEase = Ease.OutQuart;
    private Vector3 rewardPos;
    private Vector3 rewardPos1;

    private Vector3 boxOriginPos;
    private Quaternion boxOriginRot;
    private Vector3 lidOriginPos;
    private int curr;
    private Quaternion lidOriginRot;
    private void Awake()
    {
        boxOriginPos = box.transform.localPosition;
        boxOriginRot = box.transform.localRotation;
        lidOriginPos = lip.transform.localPosition;
        lidOriginRot = lip.transform.localRotation;
        rewardPos = reward.transform.localPosition;
        rewardPos1 = reward1.transform.localPosition;

    }
    void OnEnable()
    {
        ResetState();
    }
    public override void OnPopupShow(int curr )
    {
        ResetState();
        this.curr = curr;
        Jigglebox(async () =>
        {
            await OpenLid();
            //await StartZoomReward();
        });
    }

    private void Jigglebox(Action onComplete = null)
    { blockSeq.Kill();
        blockSeq = DOTween.Sequence();
        Vector3 origin = boxOriginPos + new Vector3(0, -300, 0);
        this.box.SetActive(true);


        blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, -15), 0.15f).SetEase(Ease.OutSine));
        blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, 12), 0.12f).SetEase(Ease.InOutSine));
        blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, -8), 0.10f).SetEase(Ease.InOutSine));
        blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, 5), 0.08f).SetEase(Ease.InOutSine));
        blockSeq.Append(box.transform.DORotate(new Vector3(0, 0, -2), 0.06f).SetEase(Ease.InOutSine));
        blockSeq.Append(this.box.transform.DORotate(Vector3.zero, 0.4f).SetEase(Ease.InSine));
        blockSeq.AppendCallback(() =>
        {
            this.box.transform.DOLocalMove(origin, 0.5f).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        });
    }

    private async Task OpenLid()
    {
        if (lip == null) return;

        lidSeq?.Kill();
        lidSeq = DOTween.Sequence();
        Vector3 start = lidOriginPos;
        Vector3 end = lidOriginPos + new Vector3(380, 450 - 82, 0);
        Vector3 peak = new Vector3(150, 300, 0);
        Vector3[] path = new Vector3[] { start, peak, end };
        Tween moveAnim = null;
        moveAnim = lip.transform.DOLocalPath(path, lidMoveDuration, PathType.CatmullRom, PathMode.Full3D)
        .SetEase(lidMoveEase)
        .OnUpdate(() =>
        {
            float t = moveAnim.ElapsedPercentage(false);
            float angle = Mathf.Lerp(0, -120f, t);
            lip.transform.localRotation = Quaternion.Euler(0, 0, angle);
        });

        lidSeq.Append(moveAnim);
        float fadeStartTime = lidRotationDuration * (90f / 120);
        lidSeq.InsertCallback(fadeStartTime, () => StartLidFade());
        lidSeq.InsertCallback(0.3f, async () => await StartZoomReward());
        lidSeq.InsertCallback(0.3f, () =>
            {
                //par.SetActive(true);
                par1.SetActive(true);
            });


        await lidSeq.AsyncWaitForCompletion();
    }


    private void StartLidFade()
    {
        var canvasGroup = lip.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0f, fadeDuration).SetEase(fadeEase).OnComplete(() =>
        {
            zoomButt();
            StartBoxFade();
        });
    }
    private async Task StartZoomReward()
    {
        he?.Kill();
        if (curr == 1)
        {
            Vector3 target = new Vector3(5, -160, 0) + new Vector3(0, 150, 0);
            this.reward.transform.localPosition = new Vector3(5, -160, 0);
            this.reward.transform.localScale = Vector3.zero;
            this.reward.SetActive(true);
            this.glow.SetActive(true);
            var glowCanvasGroup = glow.GetComponent<CanvasGroup>();
            if (glowCanvasGroup != null)
            {
                glowCanvasGroup.alpha = 0;
            }

            he = DOTween.Sequence();
            he.Join(reward.transform.DOLocalMove(target, 1f).SetEase(Ease.OutBack).OnUpdate(() =>
            {
                this.aura.SetActive(true);
                this.aura.transform.localPosition = reward.transform.localPosition;

            }));
            he.Join(reward.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));

            if (glowCanvasGroup != null)
            {
                he.Join(glowCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutQuad));
            }

            he.InsertCallback(0.76f, () =>
            {
                par1.SetActive(false);
            });

            await he.AsyncWaitForCompletion();
        }
        else
        {
            Vector3 target = this.reward.transform.localPosition + new Vector3(150, 150, 0);
            Vector3 target1 = this.reward1.transform.localPosition + new Vector3(-150, 150, 0);

            this.reward.transform.localPosition = this.rewardPos;
            this.reward1.transform.localPosition = this.rewardPos1;
            this.reward1.transform.localScale = Vector3.zero;
            this.reward.transform.localScale = Vector3.zero;
            this.reward.SetActive(true);
            this.reward1.SetActive(true);
            this.glow.SetActive(true);
            var glowCanvasGroup = glow.GetComponent<CanvasGroup>();
            if (glowCanvasGroup != null)
            {
                glowCanvasGroup.alpha = 0;
            }

            he = DOTween.Sequence();
            he.Join(reward.transform.DOLocalMove(target, 1f).SetEase(Ease.OutBack).OnUpdate(() =>
            {
                this.aura.SetActive(true);
                this.aura.transform.localPosition = reward.transform.localPosition;

            }));
            he.Join(reward.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));
            he.Join(reward1.transform.DOLocalMove(target1, 1f).SetEase(Ease.OutBack).OnUpdate(() =>
            {
                this.aura1.SetActive(true);
                this.aura1.transform.localPosition = reward1.transform.localPosition;

            }));
            he.Join(reward1.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));
            he.Join(reward.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));

            if (glowCanvasGroup != null)
            {
                he.Join(glowCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutQuad));
            }

            he.InsertCallback(0.76f, () =>
            {
                par1.SetActive(false);
            });

            await he.AsyncWaitForCompletion();
        }
    }
    private void zoomButt()
    {
        this.butt.transform.localScale = Vector3.zero;
        this.butt.SetActive(true);
        butt.transform.DOScale(new Vector3(1f, 1f, 1), 0.5f);
    }
    private void StartBoxFade()
    {
        var canvasGroup = box.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0f, fadeDuration).SetEase(fadeEase);
    }
    public void click()
    {
        if (curr == 1)
        {
            UIManager.HidePopup(Popup.Reward);
            int Shuffles = GameManager.instance.storageManager.getNumberShuffles();
            Shuffles += 1;
            GameManager.instance.storageManager.setNumberShuffles(Shuffles);
            GameManager.instance.nextLevel();
        }
        else
        {
            UIManager.HidePopup(Popup.Reward);
            int hints = GameManager.instance.storageManager.getNumberHints();
            hints += 1;
            GameManager.instance.storageManager.setNumberHints(hints);
            int Shuffles = GameManager.instance.storageManager.getNumberShuffles();
            Shuffles += 1;
            GameManager.instance.storageManager.setNumberShuffles(Shuffles);
        }
        
    }

    private void ResetState()
    {
        lidSeq?.Complete();
        blockSeq?.Complete();
        he?.Complete();

        lidSeq?.Kill();
        blockSeq?.Kill();
        he?.Kill();

        box.transform.DOKill();
        lip.transform.DOKill();
        reward.transform.DOKill();
        glow.transform.DOKill();
        butt.transform.DOKill();
        reward1.transform.DOKill();
        aura1.transform.DOKill();
        this.reward.SetActive(false);
                this.reward1.SetActive(false);

        this.glow.SetActive(false);
        this.butt.SetActive(false);
        this.par.SetActive(false);
        this.par1.SetActive(false);
        this.aura.SetActive(false);
        
        this.aura1.SetActive(false);

        box.transform.localPosition = boxOriginPos;
        box.transform.localRotation = boxOriginRot;
        reward1.transform.localPosition = rewardPos1;
        reward1.transform.localScale = Vector3.zero;
        reward.transform.localScale = Vector3.zero;
        lip.transform.localPosition = lidOriginPos;
        lip.transform.localRotation = lidOriginRot;
        butt.transform.localScale = Vector3.zero;
        if (curr == 1)
        {
            reward.transform.localPosition = new Vector3(5, -160, 0);
        }
        else
        {
            reward.transform.localPosition = rewardPos;
        }
        var lipCanvasGroup = lip.GetComponent<CanvasGroup>();
        if (lipCanvasGroup != null)
        {
            lipCanvasGroup.DOKill();
            lipCanvasGroup.alpha = 1;
        }

        var boxCanvasGroup = box.GetComponent<CanvasGroup>();
        if (boxCanvasGroup != null)
        {
            boxCanvasGroup.DOKill();
            boxCanvasGroup.alpha = 1;
        }

        var glowCanvasGroup = glow.GetComponent<CanvasGroup>();
        if (glowCanvasGroup != null)
        {
            glowCanvasGroup.DOKill();
            glowCanvasGroup.alpha = 0;
        }
        
    }

    protected override void ClosePopup()
    {
        lidSeq?.Kill();
        blockSeq?.Kill();

        if (UIManager.Instance != null)
        {
            var popupName = this.gameObject.name;
            if (Enum.TryParse<Page>(popupName, out Page popupType))
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