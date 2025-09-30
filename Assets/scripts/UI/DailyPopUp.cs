using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEditor;

public class DailyPopUp : BasePopup
{
    [SerializeField] public List<GameObject> listMission = new List<GameObject>();
    [SerializeField] public List<GameObject> listStage = new List<GameObject>();
    [SerializeField] public GameObject left;
    [SerializeField] public GameObject mid;
    [SerializeField] public GameObject glow;
    [SerializeField] public GameObject glow1;
    [SerializeField] public GameObject glow2;

    [SerializeField] public GameObject right;
    [SerializeField] public GameObject warning;
    [SerializeField] public GameObject overlay;

    private MissionManager missionManager => GameManager.instance.missionManager;

    public override void OnPopupShow(int curr = 0)
    {
        onStage(0);
        checkfill();
        checkUnlock();
        checkBox();
    }

    public override void OnPopupHide()
    {
    }

    public override void OnPopupDestroy()
    {
    }

    protected override void ClosePopup()
    {
        if (UIManager.Instance != null)
        {
            var popupName = this.gameObject.name;
            if (System.Enum.TryParse<Popup>(popupName, out Popup popupType))
            {
                UIManager.HidePopup(popupType);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    private void checkBox()
    {
        if (missionManager.IsStageUnlocked(1) && !missionManager.IsStageRewardClaimed(0))
        {
            glow.SetActive(true);
            glow.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        }
        if (missionManager.IsStageUnlocked(2) && !missionManager.IsStageRewardClaimed(1))
        {    glow1.SetActive(true);

            glow1.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        }
       if (missionManager.IsStageUnlocked(3) && !missionManager.IsStageRewardClaimed(2))
        {
            glow2.SetActive(true);

            glow1.transform.DORotate(new Vector3(0, 0, 360), 2f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart);
        }
    }

    public void goStage2()
    {
        onStage(1);
    }
    public void openBox1()
    {
        if (missionManager.IsStageUnlocked(1) && !missionManager.IsStageRewardClaimed(0))
        {
            UIManager.ShowPopup(Popup.Reward, true, 2, true);
            missionManager.SaveStageData(0, true, true);
            glow.SetActive(false);
        }
    }
    public void openBox2()
    {
        if (missionManager.IsStageUnlocked(2) && !missionManager.IsStageRewardClaimed(1))
        {
            UIManager.ShowPopup(Popup.Reward, true, 2, true);
            missionManager.SaveStageData(0, true, true);
            glow1.SetActive(false);
        }
    }
    public void openBox3()
    {
        if (missionManager.IsStageUnlocked(3) && !missionManager.IsStageRewardClaimed(1))
        {
            UIManager.ShowPopup(Popup.Reward, true, 2, true);
            missionManager.SaveStageData(0, true, true);
            glow2.SetActive(false);
        }
    }

    public void goStage3()
    {
        onStage(2);
    }
    public void goStage1()
    {
        onStage(0);
    }

    public void close()
    {
        this.overlay.SetActive(false);
        warning.transform.DOScale(Vector3.zero, 0.2f).OnComplete(() =>
        {
            this.warning.SetActive(false);
        });
    }

    public void pop()
    {
        this.warning.transform.localScale = Vector3.zero;
        this.warning.SetActive(true);
        this.overlay.SetActive(true);
        warning.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
    }

    public void onStage(int stageIndex)
    {
        if (!missionManager.IsStageUnlocked(stageIndex))
        {
            pop();
            return;
        }

        int startIndex = stageIndex * 3;

        for (int i = 0; i < listMission.Count; i++)
        {
            int missionDataIndex = startIndex + i;
            GameObject firstChild = this.listMission[i].transform.GetChild(1).gameObject;
            GameObject second = this.listMission[i].transform.GetChild(2).gameObject;
            GameObject pg = this.listMission[i].transform.GetChild(3).gameObject;
            GameObject play = this.listMission[i].transform.GetChild(5).gameObject;
            GameObject done1 = this.listMission[i].transform.GetChild(6).gameObject;
            GameObject left = pg.transform.GetChild(0).gameObject;
            GameObject mid = pg.transform.GetChild(1).gameObject;
            GameObject right = pg.transform.GetChild(2).gameObject;

            var missionData = missionManager.missiondatas[missionDataIndex];
            firstChild.GetComponent<TextMeshProUGUI>().text = missionData.name;
            int remain = missionData.remain;
            int missionCount = missionData.misionCount;
            bool done = missionData.done;

            if (!done)
            {
                play.SetActive(true);
                done1.SetActive(false);
                second.GetComponent<TextMeshProUGUI>().text =
                $"<color=#00FF00>{missionCount - remain}</color><color=#8B4513>/{missionCount}</color>";

                if (missionCount - remain > 0)
                {
                    left.SetActive(true);
                    mid.SetActive(true);
                    right.SetActive(true);
                    mid.transform.localScale = new Vector3((Mathf.Abs(missionCount - remain) * 27.79f) / missionCount, 1, 1);
                    RectTransform rt1 = right.transform as RectTransform;
                    rt1.anchoredPosition = new Vector2(mid.transform.localScale.x * 14, 0);
                }
                else if (missionCount - remain == 0)
                {
                    left.SetActive(false);
                    mid.SetActive(false);
                    right.SetActive(false);
                }
            }
            else
            {
                play.SetActive(false);
                done1.SetActive(true);
                second.GetComponent<TextMeshProUGUI>().text =
                $"<color=#00FF00>{missionCount}</color><color=#00FF00>/{missionCount}</color>";
                left.SetActive(true);
                mid.SetActive(true);
                right.SetActive(true);
                mid.transform.localScale = new Vector3(27.79f, 1, 1);
                RectTransform rt1 = right.transform as RectTransform;
                rt1.anchoredPosition = new Vector2(mid.transform.localScale.x * 14, 0);
            }
        }
    }

    public void checkUnlock()
    {
        missionManager.CheckAndUnlockStages();

        for (int i = 0; i < listStage.Count; i++)
        {
            GameObject firstChild = this.listStage[i].transform.GetChild(1).gameObject;
            GameObject second = this.listStage[i].transform.GetChild(2).gameObject;
            
            if (missionManager.IsStageUnlocked(i + 1))
            {
                firstChild.SetActive(false);
                second.SetActive(false);
            }
            else
            {
                firstChild.SetActive(true);
                second.SetActive(true);
            }
        }
    }

    public void checkfill()
    {
        int stageCount = missionManager.GetCompletedMissionsCount();
        
        this.left.SetActive(true);
        this.mid.SetActive(true);
        this.right.SetActive(true);

        this.mid.transform.localScale = new Vector3((48f * stageCount) / 9f, 1, 1);
        RectTransform rt1 = right.transform as RectTransform;
        rt1.anchoredPosition = new Vector2(mid.transform.localScale.x * 14, 0);
    }
}