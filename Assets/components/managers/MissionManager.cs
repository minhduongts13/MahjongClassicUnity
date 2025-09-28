using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;



public class MissionManager : MonoBehaviour
{
    public List<Missiondata> missiondatas = new List<Missiondata>();
    [SerializeField] public GameObject notif;


    public void setUp()
    {
        if (PlayerPrefs.HasKey("MissionCount"))
        {
            LoadMissions();
        }
        else
        {
            missiondatas.Add(new Missiondata { name = "Use 3 item", misionCount = 3, remain = 3 });
            missiondatas.Add(new Missiondata { name = "Reach combo 15", misionCount = 15, remain = 15 });
            missiondatas.Add(new Missiondata { name = "Win 10 levels", misionCount = 10, remain = 10 });

            SaveMissions();
        }
    }

    public void SaveMissions()
    {
        PlayerPrefs.SetInt("MissionCount", missiondatas.Count);

        for (int i = 0; i < missiondatas.Count; i++)
        {
            PlayerPrefs.SetString($"Mission_{i}_Name", missiondatas[i].name);
            PlayerPrefs.SetInt($"Mission_{i}_Count", missiondatas[i].misionCount);
            PlayerPrefs.SetInt($"Mission_{i}_remain", missiondatas[i].remain);

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
            missiondatas.Add(mission);
        }

    }

    public void UpdateMissionProgress(int missionIndex, int newCount)
    {
        if (missionIndex >= 0 && missionIndex < missiondatas.Count)
        {
            missiondatas[missionIndex].remain -= newCount;
            SaveMissions();
        }
        DisplayMissions();
        ShowPopup();

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
    public void ShowPopup()
    
    {
        DOTween.Kill(this.notif.transform);        
         Vector3 origin = this.notif.transform.localPosition;
        this.notif.transform.localPosition = origin + new Vector3(0, 400, 0);
        this.notif.SetActive(true);
        this.notif.transform.DOLocalMove(origin, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {   
            DOVirtual.DelayedCall(0.8f, () => {
                    this.notif.SetActive(false);
                });
        });
    }
}