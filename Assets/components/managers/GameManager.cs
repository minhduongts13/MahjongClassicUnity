using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] public TilePool tilePool;
    [SerializeField] public BoardManager board;

    [SerializeField] public MatchManager matchManager;
    public LevelGridData currentLevel;

    private Tile firstChosen;
    private Tile secondChosen;


    void Start()
    {
        if (!AssetsLoader.instance)
        {
            SceneManager.LoadScene(0);
        }

        GameManager.instance = this;


        SetUp();
    }

    private void SetUp()
    {
        currentLevel = LevelLoader.instance.GetLevel(50);
        tilePool.SetUp();
        board.SetUp();
        matchManager.SetUp();
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

    public void Reload()
    {
        board.SetUp();
        Debug.Log("reload");
    }
    public void showplay()
    {
        UIManager.ShowPage(Page.PLAY);
    }
    public void showdash()
    {
        UIManager.ShowPage(Page.DASHBOARD);
    }
}
