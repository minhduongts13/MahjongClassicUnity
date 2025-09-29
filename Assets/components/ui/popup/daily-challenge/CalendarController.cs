using UnityEngine;

using UnityEngine.UI;

using TMPro;

using System;

using System.Collections.Generic;

using UnityEngine.EventSystems;

public class CalendarController : MonoBehaviour
{

    [Header("UI References")]

    public GridLayoutGroup grid;                // Grid container (7 columns)
    public GameObject dayCellPrefab;            // prefab DayCell
    public TextMeshProUGUI monthText;
    public GameObject prevButton;
    public GameObject nextButton;
    public Transform weekHeaderParent;          // optional: for "Sun Mon ..."

    public GameObject playButton;
    public GameObject restartButton;
    public GameObject playBackButton;
    public CellDay selectedDayCell;
    [Header("Event Data (example)")]
    DateTime currentMonth;
    List<GameObject> cells = new List<GameObject>();
    CellDay selectedCell;


    public void Setup()
    {
        currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        BuildCalendar(currentMonth);
    }

    public void ChangeMonth(int delta)
    {
        if (delta == 0) return;
        currentMonth = currentMonth.AddMonths(delta);

        BuildCalendar(currentMonth);

    }

    void ClearCells()
    {

        foreach (var go in cells)
        {

            Destroy(go);

        }

        cells.Clear();

    }

    void BuildCalendar(DateTime month)
    {

        ClearCells();
        if (month.Month == DateTime.Today.Month && month.Year == DateTime.Today.Year)
        {
            nextButton.SetActive(false);
        }
        else
        {
            nextButton.SetActive(true);
        }
        monthText.text = month.ToString("MM/yyyy");
        DateTime firstOfMonth = new DateTime(month.Year, month.Month, 1);
        int startOffset = (int)firstOfMonth.DayOfWeek; // Sunday=0  
        int daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
        // (Optional) show previous month's trailing days:

        DateTime prevMonth = month.AddMonths(-1);

        int daysInPrev = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

        // Create leading blanks (or prev-month days)

        for (int i = 0; i < startOffset; i++)
        {

            int dayNum = daysInPrev - (startOffset - 1) + i;

            DateTime dt = new DateTime(prevMonth.Year, prevMonth.Month, dayNum);

            CreateCell(dt, isCurrentMonth: false);

        }

        // main days
        for (int d = 1; d <= daysInMonth; d++)
        {
            DateTime dt = new DateTime(month.Year, month.Month, d);
            CreateCell(dt);
        }
        if (month.Month != DateTime.Today.Month || month.Year != DateTime.Today.Year)
        {
            selectedCell = cells[cells.Count - 1].GetComponent<CellDay>();
            cells[cells.Count - 1].GetComponent<CellDay>().SetSelected();
        }
    }

    void CreateCell(DateTime date, bool isCurrentMonth = true)
    {
        GameObject go = Instantiate(dayCellPrefab, grid.transform);
        var dc = go.GetComponent<CellDay>();
        bool isToday = date.Date == DateTime.Today;
        dc.Initialize(date, isToday, OnDayClicked);
        if (!isCurrentMonth)
        {
            dc.Blank();
        }
        cells.Add(go);
    }

    void OnDayClicked(GameObject CellDay)
    {
        selectedCell = CellDay.GetComponent<CellDay>();
        // deselect previous
        foreach (var go in cells)
        {

            var dc = go.GetComponent<CellDay>();

            if (dc != null) dc.SetSelected(false);

        }
        var cd = CellDay.GetComponent<CellDay>();
        if (cd != null) cd.SetSelected();
        changeButton();
    }

    void changeButton()
    {
        if (GameManager.instance.storageManager.hasPlayedDay(selectedCell.date))
        {
            playButton.SetActive(false);
            restartButton.SetActive(true);
            playBackButton.SetActive(false);
        }
        else if (selectedCell.date == DateTime.Today)
        {
            playButton.SetActive(true);
            restartButton.SetActive(false);
            playBackButton.SetActive(false);
        }
        else if (selectedCell.date < DateTime.Today)
        {
            playButton.SetActive(false);
            restartButton.SetActive(false);
            playBackButton.SetActive(true);
        }
    }
}

 