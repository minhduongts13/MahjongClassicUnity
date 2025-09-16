using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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
    }

    public void UnChose()
    {
        firstChosen?.OnUnChose();
        secondChosen?.OnUnChose();
        firstChosen = null;
        secondChosen = null;
    }
}
