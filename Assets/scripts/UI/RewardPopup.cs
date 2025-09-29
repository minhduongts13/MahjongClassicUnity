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
    [SerializeField] GameObject glow;
    [SerializeField] GameObject par;
        [SerializeField] GameObject par1;


    private Sequence lidSeq;
    private Sequence blockSeq;
    private Sequence he;

    float lidMoveDuration = 1;
    float lidRotationDuration = 1f;
    Ease lidMoveEase = Ease.OutBack;
    Ease lidRotationEase = Ease.InOutQuart;
    float fadeDuration = 0.5f;
    Ease fadeEase = Ease.OutQuart;
    private Vector3 rewardPos;

    private Vector3 boxOriginPos;
    private Quaternion boxOriginRot;
    private Vector3 lidOriginPos;
    private  Quaternion lidOriginRot;
    private void Awake()
    {
        boxOriginPos = box.transform.localPosition;
        boxOriginRot = box.transform.localRotation;
        lidOriginPos = lip.transform.localPosition;
        lidOriginRot = lip.transform.localRotation;
        rewardPos = reward.transform.localPosition;

}

    public override void OnPopupShow(int curr = 0)
    {
        ResetState();

        Jigglebox(async () =>
        {
            await OpenLid();
           //await StartZoomReward();
        });
    }

    private void Jigglebox(Action onComplete = null)
    {
        Vector3 origin = boxOriginPos;
        this.box.transform.localPosition = origin + new Vector3(0, 400, 0);
        this.box.SetActive(true);

        blockSeq.Kill();
        blockSeq = DOTween.Sequence();
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
    Vector3 end = lidOriginPos + new Vector3(300, 200, 0);

    Vector3 peak = new Vector3(150, 300, 0);

    Vector3[] path = new Vector3[] { start, peak, end };

    Tween moveAnim = null;
    moveAnim = lip.transform.DOLocalPath(path, lidMoveDuration, PathType.CatmullRom, PathMode.Full3D)
    .SetEase(lidMoveEase)
    .OnUpdate(() =>
    {
        float t = moveAnim.ElapsedPercentage(false);
        float angle = Mathf.Lerp(0, -115f, t);
        lip.transform.localRotation = Quaternion.Euler(0, 0, angle);
    });

    lidSeq.Append(moveAnim);


    float fadeStartTime = lidRotationDuration * (70f / 115f);
    lidSeq.InsertCallback(fadeStartTime, () => StartLidFade());
    lidSeq.InsertCallback(0.2f, async () => await StartZoomReward());
    lidSeq.InsertCallback(0.2f, ()=>
        {
            par.SetActive(true);
            par1.SetActive(true);
        });


    await lidSeq.AsyncWaitForCompletion();
}


    private void StartLidFade()
    {
        var canvasGroup = lip.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0f, fadeDuration).SetEase(fadeEase);
    }
    private async Task StartZoomReward()
{
    he?.Kill(); 

    Vector3 target = rewardPos + new Vector3(0, 250, 0); 
    
    this.reward.transform.localPosition = rewardPos; 
    this.reward.transform.localScale = Vector3.zero;
    this.glow.transform.localScale = Vector3.zero;
    this.reward.SetActive(true);
    this.glow.SetActive(true);
    
    he = DOTween.Sequence();
    he.Join(reward.transform.DOLocalMove(target, 3f));
    he.Join(reward.transform.DOScale(Vector3.one, 0.6f).OnComplete(() =>
    {
            par1.SetActive(false);
    }));
    he.Join(glow.transform.DOScale(new Vector3(0.7f, 0.7f, 1), 0.5f));
    await he.AsyncWaitForCompletion();
}

   private void ResetState()
    {
        box.transform.localPosition = boxOriginPos;
        box.transform.localRotation = boxOriginRot;
        reward.transform.localPosition = rewardPos;
        lip.transform.localPosition = lidOriginPos;
        lip.transform.localRotation = lidOriginRot;
        this.reward.SetActive(false);
        this.glow.SetActive(false);
        var canvasGroup = lip.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
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
