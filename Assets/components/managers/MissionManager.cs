using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor.Overlays;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public List<Missiondata> missiondatas = new List<Missiondata>();
    [SerializeField] public GameObject notif;

    private const string STAGE_UNLOCKED_KEY = "Stage_{0}_Unlocked";
    private const string STAGE_REWARD_CLAIMED_KEY = "Stage_{0}_RewardClaimed";

    public void setUp()
    {
        if (PlayerPrefs.HasKey("MissionCount"))
        {
            LoadMissions();
        }
        else
        {
            missiondatas.Add(new Missiondata { name = "Use 3 item", misionCount = 3, remain = 3, done = false });
            missiondatas.Add(new Missiondata { name = "Reach combo 15", misionCount = 15, remain = 15, done = false });
            missiondatas.Add(new Missiondata { name = "Win 10 levels", misionCount = 10, remain = 10, done = false });
            missiondatas.Add(new Missiondata { name = "Use 5 item", misionCount = 5, remain = 5, done = false });
            missiondatas.Add(new Missiondata { name = "Reach combo 25", misionCount = 25, remain = 25, done = false });
            missiondatas.Add(new Missiondata { name = "Win 20 levels", misionCount = 20, remain = 20, done = false });
            missiondatas.Add(new Missiondata { name = "Use 100 item", misionCount = 100, remain = 100, done = false });
            missiondatas.Add(new Missiondata { name = "Reach combo 150", misionCount = 150, remain = 150, done = false });
            missiondatas.Add(new Missiondata { name = "Win 100 levels", misionCount = 100, remain = 100, done = false });

            SaveMissions();
        }
        LoadStageData();
    }

    public void SaveMissions()
    {
        PlayerPrefs.SetInt("MissionCount", missiondatas.Count);

        for (int i = 0; i < missiondatas.Count; i++)
        {
            PlayerPrefs.SetString($"Mission_{i}_Name", missiondatas[i].name);
            PlayerPrefs.SetInt($"Mission_{i}_Count", missiondatas[i].misionCount);
            PlayerPrefs.SetInt($"Mission_{i}_remain", missiondatas[i].remain);
            PlayerPrefs.SetInt($"Mission_{i}_done", missiondatas[i].done ? 1 : 0);
        }

        PlayerPrefs.Save();
    }

    public void LoadMissions()
    {
        Debug.Log("hehheudfheudfe8u");
        missiondatas.Clear();

        int missionCount = PlayerPrefs.GetInt("MissionCount", 0);

        for (int i = 0; i < missionCount; i++)
        {
            Missiondata mission = new Missiondata();
            mission.name = PlayerPrefs.GetString($"Mission_{i}_Name", "");
            mission.misionCount = PlayerPrefs.GetInt($"Mission_{i}_Count", 0);
            mission.remain = PlayerPrefs.GetInt($"Mission_{i}_remain", 0);
            mission.done = PlayerPrefs.GetInt($"Mission_{i}_done", 0) == 1;
            missiondatas.Add(mission);
        }
    }

    public void UpdateMissionProgress(int missionIndex, int newCount)
    {
        if (missionIndex < 0 || missionIndex >= missiondatas.Count)
            return;

        int stageIndex = missionIndex / 3;

        if ( !IsStageUnlocked(stageIndex))
        {
            return;
        }

        missiondatas[missionIndex].remain -= newCount;
        if (missiondatas[missionIndex].remain < 0) missiondatas[missionIndex].remain = 0;

        if (missiondatas[missionIndex].remain == 0 && missiondatas[missionIndex].done == false)
        {
            missiondatas[missionIndex].done = true;
            ShowPopup(missiondatas[missionIndex].name);
        }

        SaveMissions();
        DisplayMissions();
    }

    public void AddMission(string missionName, int targetCount)
    {
        missiondatas.Add(new Missiondata { name = missionName, misionCount = targetCount });
        SaveMissions();
    }

    public void DisplayMissions()
    {
        Debug.Log("=== Current Missions ===");
        for (int i = 0; i < missiondatas.Count; i++)
        {
            Debug.Log($"{i + 1}. {missiondatas[i].name} - Count: {missiondatas[i].misionCount} - Remain:{missiondatas[i].remain}");
        }
    }

    void Start()
    {
        setUp();
        DisplayMissions();
    }
    
    public void ShowPopup(string name)
    {
        DOTween.Kill(this.notif.transform);
        Vector3 origin = this.notif.transform.localPosition;
        GameObject firstChild = this.notif.transform.GetChild(0).gameObject;
        firstChild.GetComponent<TextMeshProUGUI>().text = name;
        this.notif.transform.localPosition = origin + new Vector3(0, 400, 0);
        this.notif.SetActive(true);
        this.notif.transform.DOLocalMove(origin, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.8f, () =>
            {
                this.notif.SetActive(false);
            });
        });
    }

    public void resetMission()
    {
        for (int i = 0; i < missiondatas.Count; i++)
        {
            if (i == 2 || i == 5 || i == 8) continue;
            if (!missiondatas[i].done)
            {
                missiondatas[i].remain = missiondatas[i].misionCount;
                SaveMissions();
            }
        }
    }

    public void resetMissionAt(int missionIndex, int num)
    {
        if (missionIndex >= 0 && missionIndex < missiondatas.Count)
        {
            missiondatas[missionIndex].remain = num;
            SaveMissions();
        }
    }

    public void SaveStageData(int stageIndex, bool isUnlocked, bool isRewardClaimed)
    {
        string unlockedKey = string.Format(STAGE_UNLOCKED_KEY, stageIndex);
        string rewardKey = string.Format(STAGE_REWARD_CLAIMED_KEY, stageIndex);
        PlayerPrefs.SetInt(unlockedKey, isUnlocked ? 1 : 0);
        PlayerPrefs.SetInt(rewardKey, isRewardClaimed ? 1 : 0);
        PlayerPrefs.Save();
    }

    public bool IsStageUnlocked(int stageIndex)
    {
        string key = string.Format(STAGE_UNLOCKED_KEY, stageIndex);
        return PlayerPrefs.GetInt(key, stageIndex == 0 ? 1 : 0) == 1;
    }

    public bool IsStageRewardClaimed(int stageIndex)
    {
        string key = string.Format(STAGE_REWARD_CLAIMED_KEY, stageIndex);
        return PlayerPrefs.GetInt(key, 0) == 1;
    }

    private void LoadStageData()
    {
        for (int i = 0; i < 3; i++)
        {
            bool unlocked = IsStageUnlocked(i);
            bool rewardClaimed = IsStageRewardClaimed(i);
        }
    }

    public int GetCompletedMissionsCount()
    {
        int stageCount = 0;
        for (int i = 0; i < missiondatas.Count; i++)
        {
            if (missiondatas[i].done)
            {
                stageCount++;
            }
        }
        return stageCount;
    }

    public void CheckAndUnlockStages()
    {
        int stageCount = GetCompletedMissionsCount();

        if (stageCount % 3 == 0)
        {
            SaveStageData(stageCount / 3, true, false);
        }
    }
}