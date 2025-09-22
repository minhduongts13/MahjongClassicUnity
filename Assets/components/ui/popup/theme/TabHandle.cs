using UnityEngine;
using TMPro;

public class TabHandle : BasePopup
{
    public GameObject Tab;
    public GameObject Backgrounds;
    public GameObject Tiles;
    public GameObject TileButton;
    public GameObject BackgroundButton;
    public GameObject[] TileList;

    public void OnShowTiles()
    {
        Tiles.SetActive(true);
        Backgrounds.SetActive(false);

        var tabBG = Tab.transform.GetChild(0);
        var rectTransform = tabBG.GetComponent<RectTransform>();
        rectTransform.Rotate(0f, 180f, 0f);

        var backgroundBG = Tab.transform.GetChild(2);
        var bgbg = backgroundBG.GetChild(0);
        bgbg.gameObject.SetActive(true);
        var backgroundText = backgroundBG.GetChild(1).GetComponent<TextMeshProUGUI>();
        backgroundText.fontSize = 50;


        var tileBG = Tab.transform.GetChild(1);
        var tileBgBg = tileBG.GetChild(0);
        tileBgBg.gameObject.SetActive(false);
        var tileText = tileBG.GetChild(1).GetComponent<TextMeshProUGUI>();
        tileText.fontSize = 60;

        var tileButtonbtn = TileButton.GetComponent<UnityEngine.UI.Button>();
        tileButtonbtn.interactable = false;
        var backgroundButtonbtn = BackgroundButton.GetComponent<UnityEngine.UI.Button>();
        backgroundButtonbtn.interactable = true;
    }
    public void OnShowBackgrounds()
    {
        Tiles.SetActive(false);
        Backgrounds.SetActive(true);

        var tabBG = Tab.transform.GetChild(0);
        var rectTransform = tabBG.GetComponent<RectTransform>();
        rectTransform.Rotate(0f, 180f, 0f);

        var backgroundBG = Tab.transform.GetChild(2);
        var bgbg = backgroundBG.GetChild(0);
        bgbg.gameObject.SetActive(false);
        var backgroundText = backgroundBG.GetChild(1).GetComponent<TextMeshProUGUI>();
        backgroundText.fontSize = 52;

        var tileBG = Tab.transform.GetChild(1);
        var tileBgBg = tileBG.GetChild(0);
        tileBgBg.gameObject.SetActive(true);
        var tileText = tileBG.GetChild(1).GetComponent<TextMeshProUGUI>();
        tileText.fontSize = 50;

        var tileButtonbtn = TileButton.GetComponent<UnityEngine.UI.Button>();
        tileButtonbtn.interactable = true;
        var backgroundButtonbtn = BackgroundButton.GetComponent<UnityEngine.UI.Button>();
        backgroundButtonbtn.interactable = false;
    }

    public void OnChooseTheme(GameObject selectedTheme)
    {
        var content = Tiles.transform.GetChild(0).GetChild(0);
        var tileTheme = content; // tileTheme là Transform

        foreach (Transform theme in tileTheme)
        {
            // lấy child cuối cùng của theme
            Transform selected = theme.GetChild(theme.childCount - 1);
            selected.gameObject.SetActive(false);
        }

        selectedTheme.SetActive(true);
    }

    public void OnChooseBackground(GameObject selectedTheme)
    {
        var backgroundTheme = Backgrounds.transform; // backgroundTheme là Transform

        foreach (Transform theme in backgroundTheme)
        {
            // lấy child cuối cùng của theme
            Transform selected = theme.GetChild(theme.childCount - 1);
            selected.gameObject.SetActive(false);
        }

        selectedTheme.SetActive(true);
    }
    
    public void Hide()
    {
        UIManager.HidePopup(Popup.THEME);
    }

}