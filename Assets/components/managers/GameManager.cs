using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] public TilePool tilePool;
    [SerializeField] public BoardManager board;

    [SerializeField] public MatchManager matchManager;

    private Tile firstChosen;
    private Tile secondChosen;


    void Start()
    {
        if (!GameManager.instance)
        {
            GameManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
        SetUp();
    }

    private void SetUp()
    {
        tilePool.SetUp();
        board.SetUp();
        matchManager.SetUp();
    }

    public void Chose(Tile tile)
    {
        if (!tile.LegalChose()) return;
        if (!firstChosen)
        {
            firstChosen = tile;
            tile.OnChose();
            return;
        }
        if (tile.GetTileType() != firstChosen.GetTileType())
        {
            UnChose();
            firstChosen = tile;
            return;
        }
        if (tile == firstChosen)
        {
            UnChose();
            return;
        }

        secondChosen = tile;
        secondChosen.OnChose();
        matchManager.Match(firstChosen, secondChosen);
        UnChose();
    }

    public void UnChose()
    {
        firstChosen?.OnUnChose();
        secondChosen?.OnUnChose();
        firstChosen = null;
        secondChosen = null;
    }
}
