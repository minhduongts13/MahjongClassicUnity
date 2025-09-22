using UnityEngine;
using DG.Tweening;
using System;

public class PlayPages : BasePage
{
    [SerializeField] GameObject[] top;
    [SerializeField] GameObject[] bot;

    public override void OnPageShow(int curr = 0)
    {
        DropDownAnimation(() => {
            if (GameManager.instance != null)
            {
                Debug.Log("Play Page Show");
                GameManager.instance.playSetUp();
            }
        });
    }

    public override void OnPageHide()
    {
    }

    public override void OnPageDestroy()
    {
    }

    protected override void ClosePage()
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

    private void DropDownAnimation(Action onComplete = null)
    {
        if ((top == null || top.Length == 0) && (bot == null || bot.Length == 0))
        {
            onComplete?.Invoke();
            return;
        }

        Vector3[] originalPositions = new Vector3[top?.Length ?? 0];
        Vector3[] originalPositions1 = new Vector3[bot?.Length ?? 0];

        if (top != null)
        {
            for (int i = 0; i < top.Length; i++)
            {
                if (top[i] != null)
                {
                    originalPositions[i] = top[i].transform.localPosition;
                    top[i].transform.localPosition = originalPositions[i] + Vector3.up * 1000f;
                }
            }
        }

        if (bot != null)
        {
            for (int i = 0; i < bot.Length; i++)
            {
                if (bot[i] != null)
                {
                    originalPositions1[i] = bot[i].transform.localPosition;
                    bot[i].transform.localPosition = originalPositions1[i] + Vector3.down * 1000f; 
                }
            }
        }

        int completedAnimations = 0;
        int totalAnimations = 0;

        if (top != null)
        {
            for (int i = 0; i < top.Length; i++)
            {
                if (top[i] != null) totalAnimations++;
            }
        }

        if (bot != null)
        {
            for (int i = 0; i < bot.Length; i++)
            {
                if (bot[i] != null) totalAnimations++;
            }
        }

        if (totalAnimations == 0)
        {
            onComplete?.Invoke();
            return;
        }

        if (top != null)
        {
            for (int i = 0; i < top.Length; i++)
            {
                if (top[i] != null)
                {
                    top[i].transform.DOLocalMove(originalPositions[i], 0.5f)
                        .SetEase(Ease.OutCubic)
                        .OnComplete(() => {
                            completedAnimations++;
                            if (completedAnimations >= totalAnimations)
                            {
                                onComplete?.Invoke();
                            }
                        });
                }
            }
        }
        if (bot != null)
        {
            for (int i = 0; i < bot.Length; i++)
            {
                if (bot[i] != null)
                {
                    bot[i].transform.DOLocalMove(originalPositions1[i], 0.5f)
                        .SetEase(Ease.OutCubic)
                        .OnComplete(() => {
                            completedAnimations++;
                            if (completedAnimations >= totalAnimations)
                            {
                                onComplete?.Invoke();
                            }
                        });
                }
            }
        }
    }
}