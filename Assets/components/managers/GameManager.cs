using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] private GameObject sakurafail;

    public void showplay()
    {
        sakurafail.SetActive(false);
        UIManager.ShowPage(Page.PLAY);
    }

    private int currentLevelNumber = 1;
    [SerializeField] public TilePool tilePool;
    [SerializeField] public BoardManager board;

    [SerializeField] public MatchManager matchManager;
    [SerializeField] public PointManager pointManager;
    [SerializeField] public StorageManager storageManager;
    public LevelGridData currentLevel;

    private Tile firstChosen;
    private Tile secondChosen;
    public bool highLight = true;
    public Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>> moves;

    public GameObject hintButton;
    public GameObject shuffleButton;


    void Start()
    {
        if (!AssetsLoader.instance)
        {
            SceneManager.LoadScene(0);
        }

        GameManager.instance = this;
        UIManager.ShowPage(Page.DASHBOARD);


        // SetUp();
    }

    private void SetUp()
    {
        moves = new Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>>();
        // currentLevelNumber = storageManager.getCurrentLevel();
        currentLevel = LevelLoader.instance.GetLevel(currentLevelNumber);
        tilePool.SetUp();
        board.SetUp();
        matchManager.SetUp();
        pointManager.Setup();
        setupTool();
        ShowMatchable();
    }

    public void Undo()
    {
        if (moves.Count == 0) return;
        var move = moves.Pop();
        board.Undo(move);
    }

    public async void Chose(Tile tile)
    {
        if (!matchManager.isFree(tile))
        {
            tile.OnBlocked();
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

    public void Reload()
    {
        moves = new Stack<Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>>>();
        tilePool.ReturnAll();
        currentLevel = LevelLoader.instance.GetLevel(currentLevelNumber);
        Debug.Log(currentLevel.levelNumber);
        board.SetUp();
        pointManager.OnChangeLevel();
        storageManager.setCurrentLevel(currentLevelNumber);
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

        foreach (Tile[,] grid in board.board)
        {
            foreach (Tile t in grid)
            {

                t.ToggleOverlay(highLight && !matchManager.isFree(t));

            }
        }
    }

    public void nextLevel()
    {
        UIManager.HidePopup(Popup.WIN);
        AdvanceLevel();
        Reload();
    }

    public void ShowHint()
    {
        Tuple<Tile, Tile> hint = board.getHint();
        if (hint == null) return;
        hint.Item1.OnHint();
        hint.Item2.OnHint();

        var numHints = storageManager.getNumberHints();
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
        if (board.remainTile <= 0) return;
        Tuple<Tile, Tile> hint = board.getHint();
        if (hint == null) return;
        await matchManager.Match(hint.Item1, hint.Item2);
    }

    public async void Shuffle()
    {
        await board.Shuffle();
        var numshuffles = storageManager.getNumberShuffles();
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
        var numHints = storageManager.getNumberHints();
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

        var numshuffles = storageManager.getNumberShuffles();
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

}
