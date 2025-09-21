using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Mono.Cecil.Cil;
using UnityEngine;

public class WinPopup : BasePopup
{
    [SerializeField] GameObject welldone;
    [SerializeField] GameObject bg;
    [SerializeField] GameObject[] ribbon;
    [SerializeField] GameObject[] rubbon;
    [SerializeField] GameObject[] fan;
    [SerializeField] GameObject[] flower;




    public override void OnPopupShow(int curr = 0)
    {

        foreach (GameObject hi in ribbon)
        {
            hi.SetActive(false);
        }

        PopWell(async () =>
        {
            await DropwellnZoomAsync(async () =>
            {
             await Task.WhenAll(popAllRibbon(), popFan(),popFlower());

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
        this.welldone.transform.localScale = new Vector3(1.4f, 1.4f, 0);

        this.bg.transform.localScale = Vector3.zero;
        var task1 = this.welldone.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.5f).AsyncWaitForCompletion();
        var task2 = this.bg.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.5f).AsyncWaitForCompletion();
        await Task.WhenAll(task1, task2);
        onComplete?.Invoke();
    }
    private async Task popAllRibbon()
    {
        List<Task> animationTasks = new List<Task>();

        for (int i = 0; i < rubbon.Length; i++)
        {
            GameObject hi = rubbon[i];
            hi.SetActive(true);
            hi.transform.localScale = Vector3.zero;
            float delay = 0f;
            if (i >= 2)
            {
                delay = 0.15f;
            }

            animationTasks.Add(
                hi.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f)
                    .SetEase(Ease.OutBack)
                    .SetDelay(delay)
                    .AsyncWaitForCompletion()
            );
        }

        await Task.WhenAll(animationTasks);
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
                startPos.x += 200f;
            }
            else if (i == 1)
            {
                startPos.x -= 200f;
            }

            fanObj.transform.localPosition = startPos;
            fanObj.transform.localScale = Vector3.zero;

            var scaleTask = fanObj.transform.DOScale(new Vector3(1.0f, 1.0f, 0), 0.6f)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();

            var moveTask = fanObj.transform.DOLocalMove(originalPos, 0.6f)
                .SetEase(Ease.OutBack)
                .AsyncWaitForCompletion();

            animationTasks.Add(scaleTask);
            animationTasks.Add(moveTask);
        }

        await Task.WhenAll(animationTasks);
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

    await Task.WhenAll(animationTasks);
}
}