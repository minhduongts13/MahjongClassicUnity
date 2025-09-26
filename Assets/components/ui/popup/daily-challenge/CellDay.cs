using UnityEngine;

using UnityEngine.UI;

using TMPro;

using System;
 
public class DayCell : MonoBehaviour {

    public TextMeshProUGUI dayText;

    public Image highlightImage;   // vòng tròn để highlight (enable/disable)

    public Image eventIcon;        // icon nhỏ (flower)

    public Button button;
 
    int day;

    DateTime date;
 
    public void Initialize(DateTime date, bool isCurrentMonth, bool isToday, bool hasEvent, Action<DateTime> onClick) {

    }
 
    public void SetSelected(bool selected) {

        highlightImage.gameObject.SetActive(selected);

    }

}

 