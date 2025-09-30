using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] List<Sprite> imagesBox = new List<Sprite>();
    [SerializeField] List<Sprite> imagesLip = new List<Sprite>();




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
    {       this.curr = curr;
            ResetState();
        if (curr == 1)
        {
            box.GetComponent<Image>().sprite = imagesBox[0];
            lip.GetComponent<Image>().sprite = imagesLip[0];
        }
        else
        {
            box.GetComponent<Image>().sprite = imagesBox[curr - 2];
            lip.GetComponent<Image>().sprite = imagesLip[curr - 2];
        }
               

        
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
                this.lip.SetActive(true);



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
    this.glow.SetActive(true);
    var glowCanvasGroup = glow.GetComponent<CanvasGroup>();
    if (glowCanvasGroup != null)
    {
        glowCanvasGroup.alpha = 0;
    }

    he = DOTween.Sequence();

    if (curr == 1)
    {
        SetupReward(reward, new Vector3(5, -160, 0), new Vector3(5, -10, 0));
        AnimateReward(reward, aura, new Vector3(5, -10, 0));
    }
    else
    {
        SetupReward(reward, rewardPos, rewardPos + new Vector3(150, 150, 0));
        SetupReward(reward1, rewardPos1, rewardPos1 + new Vector3(-150, 150, 0));
        
        AnimateReward(reward, aura, rewardPos + new Vector3(150, 150, 0));
        AnimateReward(reward1, aura1, rewardPos1 + new Vector3(-150, 150, 0));
    }

    if (glowCanvasGroup != null)
    {
        he.Join(glowCanvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutQuad));
    }

    he.InsertCallback(0.76f, () => par1.SetActive(false));

    await he.AsyncWaitForCompletion();

    void SetupReward(GameObject rewardObj, Vector3 startPos, Vector3 targetPos)
    {
        rewardObj.transform.localPosition = startPos;
        rewardObj.transform.localScale = Vector3.zero;
        rewardObj.SetActive(true);
    }

    void AnimateReward(GameObject rewardObj, GameObject auraObj, Vector3 targetPos)
    {
        he.Join(rewardObj.transform.DOLocalMove(targetPos, 1f).SetEase(Ease.OutBack)
            .OnUpdate(() =>
            {
                auraObj.SetActive(true);
                auraObj.transform.localPosition = rewardObj.transform.localPosition;
            }));
        
        he.Join(rewardObj.transform.DOScale(Vector3.one, 0.6f).SetEase(Ease.OutBack));
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
        box.SetActive(false);
    lip.SetActive(false);
        
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