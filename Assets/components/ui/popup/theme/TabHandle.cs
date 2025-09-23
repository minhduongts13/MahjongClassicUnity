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

     public override void OnPopupShow(int curr = 0, Vector3 pos = default) 
    {
        var themes = Tiles.transform.GetChild(0).GetChild(0);
        var themeIndex = GameManager.instance.storageManager.getChosenTheme();
        Debug.Log("Theme index: " + themeIndex);
        var theme = themes.GetChild((int)themeIndex);
        Transform selected = theme.GetChild(theme.childCount - 1);
        selected.gameObject.SetActive(true);
    }

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

        var themes = Tiles.transform.GetChild(0).GetChild(0);
        var themeIndex = GameManager.instance.storageManager.getChosenTheme();
        Debug.Log("Theme index: " + themeIndex);

        var theme = themes.GetChild((int)themeIndex);
        Transform selected = theme.GetChild(theme.childCount - 1);
        selected.gameObject.SetActive(true);
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

        var backgrounds = Backgrounds.transform;
        var backgroundIndex = GameManager.instance.storageManager.getChosenBackground();
        Debug.Log("Background index: " + backgroundIndex);
        var background = backgrounds.GetChild((int)backgroundIndex);
        Transform selected = background.GetChild(background.childCount - 1);
        selected.gameObject.SetActive(true);
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

        var parentName = selectedTheme.transform.parent.name;
        var themeIndex = parentName[parentName.Length - 1] - '0' - 1;
        GameManager.instance.storageManager.setChosenTheme((THEME_STYLE)themeIndex);
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

        var parentName = selectedTheme.transform.parent.name;
        var themeIndex = parentName[parentName.Length - 1] - '0' - 1;
        GameManager.instance.storageManager.setChosenBackground((BACKGROUND)themeIndex);
    }
    
    public void Hide()
    {
        UIManager.HidePopup(Popup.THEME);
    }

}