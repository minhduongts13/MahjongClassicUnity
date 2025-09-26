using UnityEngine;

using UnityEngine.UI;

using TMPro;

using System;

using System.Collections.Generic;

using UnityEngine.EventSystems;
 
public class CalendarController : MonoBehaviour {

    [Header("UI References")]

    public GridLayoutGroup grid;                // Grid container (7 columns)
    public GameObject dayCellPrefab;            // prefab DayCell
    public TextMeshProUGUI monthText;
    public Button prevButton;
    public Button nextButton;
    public Transform weekHeaderParent;          // optional: for "Sun Mon ..."

    [Header("Event Data (example)")]
    public List<string> eventDatesISO;         // list of "yyyy-MM-dd" with events (or populate via code)

    // OR better: HashSet<DateTime> eventDates
 
    HashSet<string> eventDateSet = new HashSet<string>();
    DateTime currentMonth;
    List<GameObject> cells = new List<GameObject>();
    DayCell selectedCell;
 
    void Start() {

        // parse eventDatesISO into set

        foreach (var s in eventDatesISO) {

            eventDateSet.Add(s);

        }

 
        currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        BuildCalendar(currentMonth);

    }
 
    void ChangeMonth(int delta) {

        currentMonth = currentMonth.AddMonths(delta);

        BuildCalendar(currentMonth);

    }
 
    void ClearCells() {

        foreach (var go in cells) {

            Destroy(go);

        }

        cells.Clear();

    }
 
    void BuildCalendar(DateTime month) {

        ClearCells();
 
        // Header text: "September 2025"

        monthText.text = month.ToString("MMMM yyyy");
 
        // compute first day offset

        DateTime firstOfMonth = new DateTime(month.Year, month.Month, 1);

        // if you want Monday as first day, adjust:

        // int startOffset = ((int)firstOfMonth.DayOfWeek + 6) % 7;

        int startOffset = (int)firstOfMonth.DayOfWeek; // Sunday=0
 
        int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
 
        // (Optional) show previous month's trailing days:

        DateTime prevMonth = month.AddMonths(-1);

        int daysInPrev = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

        // Create leading blanks (or prev-month days)

        for (int i = 0; i < startOffset; i++) {

            int dayNum = daysInPrev - (startOffset - 1) + i;

            DateTime dt = new DateTime(prevMonth.Year, prevMonth.Month, dayNum);

            CreateCell(dt, isCurrentMonth:false);

        }
 
        // main days

        for (int d = 1; d <= daysInMonth; d++) {

            DateTime dt = new DateTime(month.Year, month.Month, d);

            CreateCell(dt, isCurrentMonth:true);

        }
 
        // fill trailing to complete 6 rows (optional)

        int totalCells = startOffset + daysInMonth;

        int trailing = (7 - (totalCells % 7)) % 7;

        DateTime nextMonth = month.AddMonths(1);

        for (int i = 1; i <= trailing; i++) {

            DateTime dt = new DateTime(nextMonth.Year, nextMonth.Month, i);

            CreateCell(dt, isCurrentMonth:false);

        }

    }
 
    void CreateCell(DateTime date, bool isCurrentMonth) {

        GameObject go = Instantiate(dayCellPrefab, grid.transform);

        var dc = go.GetComponent<DayCell>();

        bool isToday = date.Date == DateTime.Today;

        bool hasEvent = eventDateSet.Contains(date.ToString("yyyy-MM-dd"));
 
        dc.Initialize(date, isCurrentMonth, isToday, hasEvent, OnDayClicked);

        cells.Add(go);

    }
 
    void OnDayClicked(DateTime date) {

        // deselect previous

        foreach (var go in cells) {

            var dc = go.GetComponent<DayCell>();

            if (dc != null) dc.SetSelected(false);

        }
 
        // find this cell and set selected (simple approach)

        foreach (var go in cells) {

            var dc = go.GetComponent<DayCell>();

            if (dc != null && dc != null && dc.gameObject.activeSelf) {

                // match by date text

                if (dc.dayText.text == date.Day.ToString()) {

                    dc.SetSelected(true);

                    break;

                }

            }

        }
 
        Debug.Log("Clicked date: " + date.ToString("yyyy-MM-dd"));

        // show popup or events...

    }

}

 