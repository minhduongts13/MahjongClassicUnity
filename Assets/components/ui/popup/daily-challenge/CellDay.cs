using UnityEngine;

using UnityEngine.UI;

using TMPro;

using System;
using DG.Tweening;

public class CellDay : MonoBehaviour
{

    public GameObject dayText;

    public GameObject check;   // vòng tròn để highlight (enable/disable)

    public GameObject finished;        // icon nhỏ (flower) 
    public DateTime date;
    public void Initialize(DateTime date, bool isToday, Action<GameObject> OnDayClicked = null)
    {
        this.date = date;
        dayText.GetComponent<TextMeshProUGUI>().text = date.Day.ToString();
        finished.SetActive(false);
        check.SetActive(false);
        if (GameManager.instance.storageManager.hasPlayedDay(date))
        {
            finished.SetActive(true);
        }
        else if (isToday) SetSelected();
        else if (date > DateTime.Today) HaventComeYet();

        // Optional: add click listener
        if (OnDayClicked != null)
        {
            var button = GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => OnDayClicked(gameObject));
            }
            else
            {
                Debug.LogWarning("No Button component found on " + gameObject.name);
            }
        }
    }

    public void SetSelected(bool selected = true)
    {
        check.SetActive(selected);
        if (selected)
        {
            // animate the check appearance
            Image img = check.GetComponent<Image>();
            img.fillAmount = 0f;
            img.DOFillAmount(1f, 0.5f).SetEase(Ease.Linear);
        }
        else
        {
            check.SetActive(false);
        }
    }

    public void Blank()
    {
        dayText.GetComponent<TextMeshProUGUI>().text = "";
        check.SetActive(false);
        finished.SetActive(false);
        var button = GetComponent<UnityEngine.UI.Button>();
        if (button != null)
        {
            button.interactable = false;
        }
    }

    public void HaventComeYet()
    {
        var text = dayText.GetComponent<TextMeshProUGUI>();
        Color c = text.color;
        text.color = new Color(c.r, c.g, c.b, 0.65f);
        check.SetActive(false);
        finished.SetActive(false);
        var button = GetComponent<UnityEngine.UI.Button>();
        if (button != null)
        {
            button.interactable = false;
        }
    }

}

 