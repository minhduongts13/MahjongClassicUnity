        using UnityEngine;
        using System.Collections.Generic;
        using TMPro;
        public class DailyPopUp : BasePopup
        {
        [SerializeField] public List<GameObject> listMission = new List<GameObject>();
        public override void OnPopupShow(int curr = 0)
        {
        onStage1();
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
    private void onStage1()
    {
        for (int i = 0; i < listMission.Count; i++)
        {
            GameObject firstChild = this.listMission[i].transform.GetChild(1).gameObject;
            GameObject second = this.listMission[i].transform.GetChild(2).gameObject;
            GameObject pg = this.listMission[i].transform.GetChild(3).gameObject;
            GameObject left = pg.transform.GetChild(0).gameObject;
            GameObject mid = pg.transform.GetChild(1).gameObject;
            GameObject right = pg.transform.GetChild(2).gameObject;
            firstChild.GetComponent<TextMeshProUGUI>().text = GameManager.instance.missionManager.missiondatas[i].name;
            int remain = GameManager.instance.missionManager.missiondatas[i].remain;
            int missionCount = GameManager.instance.missionManager.missiondatas[i].misionCount;
            bool done = GameManager.instance.missionManager.missiondatas[i].done;
            if (!done)
            {
                second.GetComponent<TextMeshProUGUI>().text =
                $"<color=#00FF00>{missionCount - remain}</color><color=#8B4513>/{missionCount}</color>";
                if (missionCount - remain > 0)
                {
                    left.SetActive(true);
                    mid.SetActive(true);
                    right.SetActive(true);
                    mid.transform.localScale = new Vector3((remain * 27.79f) / missionCount, 1, 1);
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
        }