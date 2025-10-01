using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject sakurafail;
    [SerializeField] private GameObject btn;

    public void showplay()
    {
        sakurafail.SetActive(false);

        UIManager.ShowPage(Page.PLAY);
        combo.ResetCombo();


    }

    public int currentLevelNumber = 0;
    public int numTut = 0;
    public THEME currentTheme = THEME.Green;
    [SerializeField] public TilePool tilePool;
    [SerializeField] public BoardManager board;
    [SerializeField] public FloatingPoint floatingPoint;
    [SerializeField] public MissionManager missionManager;

    [SerializeField] public MatchManager matchManager;
    [SerializeField] public PointManager pointManager;
    [SerializeField] public StorageManager storageManager;
    [SerializeField] public Combo combo;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text dateText;

    public LevelGridData currentLevel;

    private Tile firstChosen;
    private Tile secondChosen;
    public bool highLight = false;
    public Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>> moves;

    public GameObject hintButton;
    public GameObject shuffleButton;
    public bool hinting = false;

    public bool dailyChallenge = false;
    public DateTime dailyDate;


    void Start()
    {

        currentLevelNumber = storageManager.getCurrentLevel();
        if (!AssetsLoader.instance)
        {
            SceneManager.LoadScene(0);
        }

        GameManager.instance = this;
        UIManager.ShowPage(Page.DASHBOARD);
        currentTheme = (THEME)storageManager.getChosenTheme();


        // SetUp();
    }


    private async void SetUp()
    {
        dailyChallenge = false;
        moves = new Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>>();
        // currentLevelNumber = storageManager.getCurrentLevel();
        currentLevel = LevelLoader.instance.GetLevel(currentLevelNumber);
        if (currentLevelNumber == 1)
        {
            storageManager.Increase();
        }
        tilePool.SetUp();
        setupTool();

        Task t = board.SetUp();
        matchManager.SetUp();
        pointManager.Setup();

        ShowMatchable();
        await t;
    }
    void Update()
    {
        levelText.text = "Level " + (currentLevelNumber == 0 ? 1 : currentLevelNumber);
        if (dateText != null)
            dateText.text = DateTime.Now.ToString("dd");
        setupTool();

    }
    public async void JumpTo(int level, DateTime date)
    {
        setupTool();

        dailyChallenge = true;
        dailyDate = date;
        Debug.Log("Jumping to level: " + level);
        currentLevel = LevelLoader.instance.GetLevel(level);
        moves = new Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>>();
        UnChose();
        tilePool.ReturnAll();
        Debug.Log(currentLevel.levelNumber);
        // await board.SetUp();
        Task t = board.SetUp();
        matchManager.SetUp();
        pointManager.OnDailyChallenge(date);

        combo.ResetCombo();
        Debug.Log("reload");
        ShowMatchable();
        await t;

    }

    public async void Undo()
    {
        if (moves.Count == 0) return;
        var move = moves.Pop();
        await board.Undo(move);
    }

    public async void Chose(Tile tile)
    {
        if (!matchManager.isFree(tile))
        {
            tile.OnBlocked();
            GameManager.instance.missionManager.resetMissionAt(1, 15);
            GameManager.instance.missionManager.resetMissionAt(4, 25);
            GameManager.instance.missionManager.resetMissionAt(7, 150);

            combo.ResetCombo();
            return;
        }
        if (!firstChosen)
        {
            firstChosen = tile;
            tile.OnChose();
            tile.MoveOffset(matchManager.BlockRight(tile), matchManager.BlockRight(tile) || matchManager.BlockLeft(tile));
            return;
        }
        if (tile.GetTileType() != firstChosen.GetTileType())
        {
            UnChose();
            firstChosen = tile;
            firstChosen.OnChose();
            tile.MoveOffset(matchManager.BlockRight(tile), matchManager.BlockRight(tile) || matchManager.BlockLeft(tile));
            return;
        }
        if (tile == firstChosen)
        {
            UnChose();
            return;
        }

        secondChosen = tile;
        secondChosen.OnChose();

        Tile t1 = firstChosen;
        Tile t2 = secondChosen;
        UnChose();
        await matchManager.Match(t1, t2);

    }

    public void UnChose()
    {
        firstChosen?.OnUnChose();
        secondChosen?.OnUnChose();
        firstChosen = null;
        secondChosen = null;
    }
    public void setLevel(int number)
    {
        currentLevelNumber = number;
    }
    public void AdvanceLevel()
    {
        currentLevelNumber++;
    }

    public async Task Reload()
    {
        moves = new Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>>();
        UnChose();
        setupTool();
        tilePool.ReturnAll();
        currentLevel = LevelLoader.instance.GetLevel(currentLevelNumber);
        Debug.Log(currentLevel.levelNumber);
        await board.SetUp();
        pointManager.OnChangeLevel();

        storageManager.setCurrentLevel(currentLevelNumber);
        combo.ResetCombo();
        Debug.Log("reload");
        ShowMatchable();
    }

    public void playSetUp()
    {
        this.SetUp();
    }
    public void showdash()
    {
        sakurafail.SetActive(true);
        UIManager.ShowPage(Page.DASHBOARD);
        board.KillBoard();
    }

    public void ToggleShowMatch()
    {
        highLight = !highLight;
        ShowMatchable();
    }

    public void ShowMatchable()
    {
        if (board.board == null) return;
        foreach (Tile[,] grid in board.board)
        {
            foreach (Tile t in grid)
            {

                t.ToggleOverlay(highLight && !matchManager.isFree(t));

            }
        }
    }

    public async void nextLevel()
    {
        this.missionManager.UpdateMissionProgress(2, 1);
        this.missionManager.UpdateMissionProgress(5, 1);
        this.missionManager.UpdateMissionProgress(8, 1);
        this.missionManager.resetMission();
        UIManager.HidePopup(Popup.WIN);
        AdvanceLevel();
        await Reload();
    }


    public void ShowHint()
    {
        if (board.shuffling) return;
        if (hinting) return;
        var numHints = storageManager.getNumberHints();
        if (numHints <= 0) return;
        Tuple<Tile, Tile> hint = board.getHint();
        if (hint == null) return;
        hint.Item1.OnHint();
        hint.Item2.OnHint();
        hinting = true;
        this.missionManager.UpdateMissionProgress(0, 1);
        this.missionManager.UpdateMissionProgress(3, 1);
        this.missionManager.UpdateMissionProgress(6, 1);



        var bgNum0 = hintButton.transform.GetChild(2);
        var bgNum1 = hintButton.transform.GetChild(0);
        var textGO = hintButton.transform.GetChild(1).gameObject;
        var text = textGO.GetComponent<TMPro.TextMeshProUGUI>();

        if (numHints == 1)
        {
            bgNum0.gameObject.SetActive(true);
            bgNum1.gameObject.SetActive(false);
            textGO.SetActive(false);
        }
        else
        {
            bgNum0.gameObject.SetActive(false);
            bgNum1.gameObject.SetActive(true);
            text.text = (numHints - 1).ToString();
            textGO.SetActive(true);
        }
        storageManager.setNumberHints(numHints - 1);
    }
    public async void Match()
    {
        if (board.shuffling) return;
        if (board.remainTile <= 0) return;
        Tuple<Tile, Tile> hint = board.getHint();
        if (hint == null) return;
        Task t = matchManager.Match(hint.Item1, hint.Item2);
        AnimationManager.instance.tileMoveAnimation.Add(t);
        await t;
    }

    public async void Shuffle()
    {
        if (board.shuffling) return;
        var numshuffles = storageManager.getNumberShuffles();
        if (numshuffles <= 0) return;
        await board.Shuffle();
        pointManager.OnChangeMatches();
        this.missionManager.UpdateMissionProgress(0, 1);
        this.missionManager.UpdateMissionProgress(3, 1);
        this.missionManager.UpdateMissionProgress(6, 1);
        pointManager.OnChangeMatches();
        var bgNum0 = shuffleButton.transform.GetChild(2);
        var bgNum1 = shuffleButton.transform.GetChild(0);
        var textGO = shuffleButton.transform.GetChild(1).gameObject;
        var text = textGO.GetComponent<TMPro.TextMeshProUGUI>();

        if (numshuffles == 1)
        {
            bgNum0.gameObject.SetActive(true);
            bgNum1.gameObject.SetActive(false);
            textGO.SetActive(false);
        }
        else
        {
            bgNum0.gameObject.SetActive(false);
            bgNum1.gameObject.SetActive(true);
            text.text = (numshuffles - 1).ToString();
            textGO.SetActive(true);
        }
        storageManager.setNumberShuffles(numshuffles - 1);
    }
    public void winDebug()
    {
        board.KillBoard();
        UIManager.ShowPopup(Popup.WIN);
    }

    public void setupTool()
    {
        var numHints = storageManager.getNumberHints() <= 0 ? 0 : storageManager.getNumberHints();
        storageManager.setNumberHints(numHints);

        var bgNum0 = hintButton.transform.GetChild(2);
        var bgNum1 = hintButton.transform.GetChild(0);
        var textGO = hintButton.transform.GetChild(1).gameObject;
        var text = textGO.GetComponent<TMPro.TextMeshProUGUI>();

        if (numHints == 0)
        {
            bgNum0.gameObject.SetActive(true);
            bgNum1.gameObject.SetActive(false);
            textGO.SetActive(false);
        }
        else
        {
            bgNum0.gameObject.SetActive(false);
            bgNum1.gameObject.SetActive(true);
            text.text = (numHints).ToString();
            textGO.SetActive(true);
        }

        var numshuffles = storageManager.getNumberShuffles() <= 0 ? 0 : storageManager.getNumberShuffles();
        storageManager.setNumberShuffles(numshuffles);
        bgNum0 = shuffleButton.transform.GetChild(2);
        bgNum1 = shuffleButton.transform.GetChild(0);
        textGO = shuffleButton.transform.GetChild(1).gameObject;
        text = textGO.GetComponent<TMPro.TextMeshProUGUI>();

        if (numshuffles == 0)
        {
            bgNum0.gameObject.SetActive(true);
            bgNum1.gameObject.SetActive(false);
            textGO.SetActive(false);
        }
        else
        {
            bgNum0.gameObject.SetActive(false);
            bgNum1.gameObject.SetActive(true);
            text.text = (numshuffles).ToString();
            textGO.SetActive(true);
        }
    }

    public async void Replay()
    {
        await Reload();
    }
    public void showReward()
    {
        UIManager.ShowPopup(Popup.Reward, true, 2, false);
    }

    public void ShowDebug()
    {
        // Toggle trạng thái theo thằng child đầu tiên (hoặc mặc định true/false)
        bool newState = true;

        if (btn.transform.childCount > 0)
            newState = !btn.transform.GetChild(0).gameObject.activeSelf;

        for (int i = 0; i < btn.transform.childCount; i++)
        {
            btn.transform.GetChild(i).gameObject.SetActive(newState);
        }
    }

    private async void SetUpTutorial()
    {
        moves = new Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>>();
        // currentLevelNumber = storageManager.getCurrentLevel();
        tilePool.SetUp();
        setupTool();

        Task t = board.SetUpTutorial(numTut);
        matchManager.SetUp();

        ShowMatchable();
        await t;
        UIManager.ShowPopup(Popup.Tutorial, true, GameManager.instance.numTut, false, false, false);

    }
    public void playSetUpTutorial()
    {
        this.SetUpTutorial();
    }
    public void DailyReward()
    {
        UIManager.ShowPopup(Popup.DAILY_REWARD);
    }

}
