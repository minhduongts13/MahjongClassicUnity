using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    private List<int[,]> mockUpLevel;
    public List<Tile[,]> board;
    public RectTransform anchor;
    public int remainTile;
    public bool shuffling = false;
    public int t1 = 0, t2 = 0;
    [SerializeField] private RectTransform topBar;
    [SerializeField] private RectTransform bottomBar;


    public Dictionary<string, List<int[,]>> tutorialLevels = new Dictionary<string, List<int[,]>>();

    void InitTutorialLevels()
    {
        tutorialLevels["Match two of the same tile"] = CreateTutorial1();
        tutorialLevels["Match the top tile to unlock other tiles"] = CreateTutorial2();
        tutorialLevels["Match the tile on the side to unlock other tiles"] = CreateTutorial3();
    }
    public string GetTutorialName(int index)
    {
        var keys = new List<string>(tutorialLevels.Keys);
        if (index >= 0 && index < keys.Count)
        {
            return keys[index];
        }
        return null;
    }

    private List<int[,]> CreateTutorial1()
    {
        List<int[,]> layers = new List<int[,]>();

        int[,] layer1 = {
            { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 1, 0, 1, 0 },
            { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 },
        };
        layers.Add(layer1);

        return layers;
    }

    private List<int[,]> CreateTutorial2()
    {
        List<int[,]> layers = new List<int[,]>();

        int[,] layer1 = {
            { 0, 0, 0, 0, 0, 0 },
            { 0, 11, 0, 11, 0, 28 },
            { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 },
        };
        layers.Add(layer1);

        int[,] layer2 = {
            { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 28, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0 },
        };
        layers.Add(layer2);

        return layers;
    }

    private List<int[,]> CreateTutorial3()
    {
        List<int[,]> layers = new List<int[,]>();

        int[,] layer1 = {
            { 0, 0, 0, 0, 0, 0, 0 },
            { 32, 0, 10, 0, 10, 0, 32 },
            { 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0 },
        };
        layers.Add(layer1);

        return layers;
    }


    public List<int[,]> GetTutorialLevel(int index)
    {
        var keys = new List<string>(tutorialLevels.Keys);
        if (index >= 0 && index < keys.Count)
        {
            return tutorialLevels[keys[index]];
        }

        return null;
    }

    public async Task SetUp()
    {

        shuffling = true;
        remainTile = 0;
        List<Task> tasks = new List<Task>();
        var middlePoint = (topBar.anchoredPosition.y + bottomBar.anchoredPosition.y) / 2;
        (GameManager.instance.tilePool.transform as RectTransform).anchoredPosition = new Vector2(0, middlePoint);
        Debug.Log("aaa");
        // LevelMock();
        // mockUpLevel = GameManager.instance.currentLevel.layers;
        Debug.Log("llalaal");
        LevelLoader.PrintLevelGridData(GameManager.instance.currentLevel);
        mockUpLevel = LevelLoader.instance.getArray(GameManager.instance.currentLevel);
        Debug.Log("MOCK" + mockUpLevel);
        Rescale(GameManager.instance.currentLevel);
        board = new List<Tile[,]>();

        for (int i = 0; i < mockUpLevel.Count; i++)
        {
            int[,] levelData = mockUpLevel[i];
            int rows = levelData.GetLength(0);
            int cols = levelData.GetLength(1);

            Tile[,] tileGrid = new Tile[rows, cols];

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {

                    // Tạo tile từ prefab
                    Tile t = GameManager.instance.tilePool.GetFirstItem();
                    t.Reset();
                    t.OffHint();
                    // t.transform.SetSiblingIndex(2 * (rows * (c + c % 2) + r + i * cols * rows) + (c % 2 == 0 ? 0 : 1));
                    t.idx = 2 * (rows * (c + c % 2) + r + i * cols * rows) + (c % 2 == 0 ? 0 : 1);
                    // t.transform.SetSiblingIndex((c - c % 2) * rows + (r - r % 2) * cols + i * rows * cols);
                    tileGrid[r, c] = t;
                    if (levelData[r, c] == 10)
                    {
                        t1++;
                    }
                    if (levelData[r, c] == 20)
                    {
                        t2++;
                    }
                    t.layer = i;
                    t.BlockInput();
                    t.coords = new Vector2Int(c, r);
                    t.SetTheme(GameManager.instance.currentTheme);
                    t.setTileType(levelData[r, c]);
                    // if (levelData[r, c] == 0)
                    // {
                    //     t.Kill();
                    // }
                }
            }



            board.Add(tileGrid);
            foreach (Tile t in tileGrid)
            {
                if (t != null)
                    t.TurnOnInput();
            }


        }
        ApplySiblingOrder(mockUpLevel, board);
        for (int i = 0; i < mockUpLevel.Count; i++)
        {

            int rows = mockUpLevel[i].GetLength(0);
            int cols = mockUpLevel[i].GetLength(1);
            int[,] levelData = mockUpLevel[i];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (board[i][r, c] != null)
                    {
                        int offset = c < cols / 2 ? -1000 : 1000;
                        (board[i][r, c].transform as RectTransform).anchoredPosition = GetPosFromCoords(c, r, i) + new Vector2(offset, 0);
                    }
                    if (levelData[r, c] == 0)
                    {
                        board[i][r, c].Kill();
                        // board[i][r, c].gameObject.SetActive(false);

                    }
                    else
                    {

                        board[i][r, c].setTileType(levelData[r, c]);
                        // tasks.Add(board[i][r, c].Zoom(i));
                        int noise = UnityEngine.Random.Range(0, 5);
                        tasks.Add(board[i][r, c].MoveToRealPos((i + 1) * 100 + (Math.Abs(r - rows / 2 + noise)) * 50, Ease.OutCirc));
                        remainTile++;
                    }
                }
            }
        }

        await Task.WhenAll(tasks);

        shuffling = false;


    }
    public void resetSibling()
    {
        ApplySiblingOrder(mockUpLevel, board);
    }

    public Vector2 GetPosFromCoords(int x, int y, int layerIndex)
    {
        int rows = board[layerIndex].GetLength(0);
        int cols = board[layerIndex].GetLength(1);

        float tileWidth = 125f / 2;
        float tileHeight = 155f / 2;

        // Calculate total grid size
        float totalWidth = cols * tileWidth;
        float totalHeight = rows * tileHeight;

        // Offset from center of grid
        float originX = -totalWidth / 2f + tileWidth / 2f;
        float originY = totalHeight / 2f - tileHeight / 2f;

        float posX = originX + (x * tileWidth);
        float posY = originY - (y * tileHeight);

        // Apply layer offset (up and left)
        float layerOffsetX = -tileWidth * 0.18f * layerIndex;  // shift left
        float layerOffsetY = tileHeight * 0.2f * layerIndex;  // shift up

        posX += layerOffsetX;
        posY += layerOffsetY;

        return new Vector2(posX, posY);
    }

    public List<Tile> getNeighbour(Tile tile)
    {
        List<Tile> neightbour = new List<Tile>();

        for (int i = -1; i < 2; i++)
        {
            if (tile.coords.x - 2 >= 0 && tile.coords.y + i >= 0 && tile.coords.y + i < board[0].GetLength(0))
            {
                neightbour.Add(board[tile.layer][tile.coords.y + i, tile.coords.x - 2]);
            }
            if (tile.coords.x + 2 < board[0].GetLength(1) && tile.coords.y + i >= 0 && tile.coords.y + i < board[0].GetLength(0))
            {
                neightbour.Add(board[tile.layer][tile.coords.y + i, tile.coords.x + 2]);
            }
        }
        if (tile.layer == board.Count - 1)
        {
            return neightbour;
        }
        for (int y = -1; y < 2; y++)
        {
            if (tile.coords.y + y < 0 || tile.coords.y + y >= board[0].GetLength(0)) continue;
            for (int x = -1; x < 2; x++)
            {
                if (tile.coords.x + x < 0 || tile.coords.x + x >= board[0].GetLength(1)) continue;
                neightbour.Add(board[tile.layer + 1][tile.coords.y + y, tile.coords.x + x]);
            }
        }
        return neightbour;

    }
    public void UpdateTheme()
    {
        foreach (Tile[,] layer in board)
        {
            foreach (Tile t in layer)
            {
                if (t != null)
                {
                    t.SetTheme(GameManager.instance.currentTheme);
                }
            }
        }
    }
    public List<Tile> getUnder(Tile tile)
    {
        List<Tile> neightbour = new List<Tile>();

        if (tile.layer == 0)
        {
            return neightbour;
        }
        for (int y = -1; y < 2; y++)
        {
            if (tile.coords.y + y < 0 || tile.coords.y + y >= board[0].GetLength(0)) continue;
            for (int x = -1; x < 2; x++)
            {
                if (tile.coords.x + x < 0 || tile.coords.x + x >= board[0].GetLength(1)) continue;
                neightbour.Add(board[tile.layer - 1][tile.coords.y + y, tile.coords.x + x]);
            }
        }
        return neightbour;
    }
    public void Rescale(LevelGridData level, float tileWidth = 133, float tileHeight = 166)
    {
        float sceneX = anchor.rect.width;

        float sceneY = anchor.rect.height - 500;

        int sizeX = level.layers.Max(layer => layer.gridData.Grid.GetLength(1));
        int sizeY = level.layers.Max(layer => layer.gridData.Grid.GetLength(0));

        float scaleX = (sceneX - 100) / ((sizeX + 3) * (tileWidth / 2f));
        float scaleY = (sceneY - 100) / ((sizeY + 3) * (tileHeight / 2f));

        float scale = Mathf.Min(scaleX, scaleY);
        GameManager.instance.tilePool.transform.localScale = new Vector3(scale, scale, 1);
    }


    public void ApplySiblingOrder(List<int[,]> levelData, List<Tile[,]> board)
    {
        List<Tile> ordered = new List<Tile>();

        for (int layer = 0; layer < levelData.Count; layer++)
        {
            int[,] data = levelData[layer];
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);

            Tile[,] grid = board[layer];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (data[r, c] == 0) continue;
                    Tile t = grid[r, c];
                    if (t == null) continue;

                    ordered.Add(t);
                }
            }
        }

        // Sort theo CompareTo trong Tile
        ordered.Sort();

        // Gán sibling index tuần tự
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].transform.SetSiblingIndex(i);
        }
    }







    public Tuple<Tile, Tile> getHint()
    {
        List<Tile> list = new List<Tile>();
        foreach (Tile[,] tiles in board)
        {
            foreach (Tile t in tiles)
            {
                if (t != null && GameManager.instance.matchManager.isFree(t))
                {
                    list.Add(t);
                }
            }
        }

        // duyệt tìm cặp
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[i].GetTileType() == list[j].GetTileType() && list[i].GetTileType() != 0)
                {
                    return Tuple.Create(list[i], list[j]);
                }
            }
        }

        // không tìm thấy
        return null;
    }

    public int getAllHints()
    {
        List<Tile> list = new List<Tile>();
        HashSet<int> seen = new HashSet<int>();
        int count = 0;
        foreach (Tile[,] tiles in board)
        {
            foreach (Tile t in tiles)
            {
                if (t != null && GameManager.instance.matchManager.isFree(t))
                {
                    list.Add(t);
                }
            }
        }

        // duyệt tìm cặp
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[i].GetTileType() == list[j].GetTileType() && list[i].GetTileType() != 0)
                {
                    if (seen.Contains(i) || seen.Contains(j)) continue;
                    seen.Add(i);
                    seen.Add(j);
                    count++;
                }
            }
        }

        // không tìm thấy
        return count;
    }

    private void RestoreTile(Vector3 pos, int type)
    {
        Tile tile = board[(int)pos.z][(int)pos.y, (int)pos.x];
        tile.setTileType(type);
        tile.Reset();
        tile.gameObject.SetActive(true);


        Vector2 p = GetPosFromCoords(tile.coords.x, tile.coords.y, tile.layer);
        tile.Zoom(0);
        (tile.transform as RectTransform).anchoredPosition = p;
        tile.used = true;
    }

    public async Task Undo(Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>> move)
    {
        remainTile += 2;
        await Task.WhenAll(AnimationManager.instance.tileMoveAnimation);
        RestoreTile(move.Item1.Item1, move.Item2.Item1);
        RestoreTile(move.Item1.Item2, move.Item2.Item2);
        ApplySiblingOrder(mockUpLevel, board);
    }


    public async Task Shuffle()
    {
        shuffling = true;
        GameManager.instance.UnChose();
        List<Tile> tilesList = new List<Tile>();
        List<int> typeList = new List<int>();
        List<Task> task = new List<Task>();
        List<Tile>
        freeTile = new List<Tile>();

        foreach (Tile[,] tiles in board)
        {
            foreach (Tile t in tiles)
            {
                if (t != null && t.GetTileType() != 0)
                {
                    tilesList.Add(t);
                    if (GameManager.instance.matchManager.isFree(t))
                    {
                        freeTile.Add(t);
                    }
                    t.OffHint();
                    t.OnUnChose();
                    typeList.Add(t.GetTileType());

                    RectTransform rt = t.transform as RectTransform;
                    t.ToggleShadow(false);

                    Vector2 startPos = rt.anchoredPosition;
                    Vector2 midPos = startPos + (startPos.normalized * 50f); // đẩy ngược ra ngoài 1 chút (150 đơn vị)
                    Vector2 endPos = Vector2.zero; // vị trí trung tâm

                    // Tạo sequence cho từng tile
                    Sequence seq = DOTween.Sequence();
                    seq.Append(rt.DOAnchorPos(midPos, 0.25f).SetEase(Ease.OutCubic))   // chạy ngược ra
                       .Append(rt.DOAnchorPos(endPos, 0.2f).SetEase(Ease.InCirc));    // rồi chạy vào trung tâm

                    task.Add(seq.AsyncWaitForCompletion());

                }
            }
        }
        await Task.WhenAll(task);
        AnimationManager.instance.ShuffleEffect();
        await Task.Delay(400);
        List<Task> ta = new();

        // Fisher–Yates shuffle
        System.Random rand = new System.Random();
        for (int i = typeList.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (typeList[i], typeList[j]) = (typeList[j], typeList[i]);
        }

        // gán ngược type lại cho tile
        for (int i = 0; i < tilesList.Count; i++)
        {
            tilesList[i].setTileType(typeList[i]);
        }

        // --- đảm bảo freeTile có ít nhất 1 cặp ---
        if (freeTile.Count >= 2)
        {
            // group theo type
            var groups = freeTile.GroupBy(t => t.GetTileType()).ToList();
            bool hasPair = groups.Any(g => g.Count() >= 2);

            if (!hasPair)
            {
                // nếu không có cặp nào, thì bắt buộc tạo cặp bằng cách swap
                Tile tileA = freeTile[rand.Next(freeTile.Count)];
                Tile tileB = freeTile[rand.Next(freeTile.Count)];
                while (tileB == tileA) tileB = freeTile[rand.Next(freeTile.Count)];

                // chọn một tile khác ngoài freeTile để swap type với tileB
                Tile swapWith = tilesList.FirstOrDefault(t => !freeTile.Contains(t) && t.GetTileType() == tileA.GetTileType());

                if (swapWith != null)
                {
                    int tmp = tileB.GetTileType();
                    tileB.setTileType(tileA.GetTileType());
                    swapWith.setTileType(tmp);
                }
                else
                {
                    // Handle the case when no matching tile is found.
                    // For example, log a warning, or choose another strategy.
                    Debug.LogWarning("No suitable tile found to swap with tileB.");
                }

            }
        }



        foreach (Tile[,] tiles in board)
        {
            foreach (Tile t in tiles)
            {
                if (t != null && t.GetTileType() != 0)
                {
                    ta.Add(t.MoveToRealPos(0, Ease.OutBack, 0.5f));
                    t.ToggleShadow(true);
                }
            }
        }
        await Task.WhenAll(ta);
        shuffling = false;
    }


    public List<Tile> getTileNear(Vector2 pos, bool onlyFree = true, float width = 133f, float height = 166f)
    {
        List<Tile> tiles = new List<Tile>();

        foreach (Tile[,] layer in board)
        {
            foreach (Tile t in layer)
            {
                if (t == null || t.GetTileType() == 0)
                    continue;


                Vector2 p = GetPosFromCoords(t.coords.x, t.coords.y, t.layer);

                bool inX = Mathf.Abs(p.x - pos.x) <= width;
                bool inY = Mathf.Abs(p.y - pos.y) <= height;

                if (inX && inY)
                {
                    tiles.Add(t);
                }
            }
        }

        return tiles;
    }
    public void KillBoard()
    {
        for (int i = 0; i < board.Count; i++)
        {
            Tile[,] tiles = board[i];
            int rows = tiles.GetLength(0);
            int cols = tiles.GetLength(1);
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (tiles[r, c] != null)
                    {
                        tiles[r, c].Kill();
                    }
                }
            }
        }
    }
    public async Task SetUpTutorial(int num)
    {
        InitTutorialLevels();
        shuffling = true;
        remainTile = 0;
        List<Task> tasks = new List<Task>();
        var middlePoint = (topBar.anchoredPosition.y + bottomBar.anchoredPosition.y) / 2;
        (GameManager.instance.tilePool.transform as RectTransform).anchoredPosition = new Vector2(0, middlePoint);

        mockUpLevel = GetTutorialLevel(num);
        if (num > 2) return;

        Debug.Log("MOCK" + mockUpLevel);
        board = new List<Tile[,]>();

        for (int i = 0; i < mockUpLevel.Count; i++)
        {
            int[,] levelData = mockUpLevel[i];
            int rows = levelData.GetLength(0);
            int cols = levelData.GetLength(1);

            Tile[,] tileGrid = new Tile[rows, cols];

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {

                    // Tạo tile từ prefab
                    Tile t = GameManager.instance.tilePool.GetFirstItem();
                    t.Reset();
                    t.OffHint();
                    // t.transform.SetSiblingIndex(2 * (rows * (c + c % 2) + r + i * cols * rows) + (c % 2 == 0 ? 0 : 1));
                    t.idx = 2 * (rows * (c + c % 2) + r + i * cols * rows) + (c % 2 == 0 ? 0 : 1);
                    // t.transform.SetSiblingIndex((c - c % 2) * rows + (r - r % 2) * cols + i * rows * cols);
                    tileGrid[r, c] = t;
                    if (levelData[r, c] == 10)
                    {
                        t1++;
                    }
                    if (levelData[r, c] == 20)
                    {
                        t2++;
                    }
                    t.layer = i;
                    t.BlockInput();
                    t.coords = new Vector2Int(c, r);
                    t.SetTheme(GameManager.instance.currentTheme);
                    t.setTileType(levelData[r, c]);
                    // if (levelData[r, c] == 0)
                    // {
                    //     t.Kill();
                    // }
                }
            }



            board.Add(tileGrid);
            foreach (Tile t in tileGrid)
            {
                if (t != null)
                    t.TurnOnInput();
            }


        }
        ApplySiblingOrder(mockUpLevel, board);
        for (int i = 0; i < mockUpLevel.Count; i++)
        {

            int rows = mockUpLevel[i].GetLength(0);
            int cols = mockUpLevel[i].GetLength(1);
            int[,] levelData = mockUpLevel[i];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (board[i][r, c] != null)
                    {
                        int offset = c < cols / 2 ? -1000 : 1000;
                        (board[i][r, c].transform as RectTransform).anchoredPosition = GetPosFromCoords(c, r, i) + new Vector2(offset, 0);
                    }
                    if (levelData[r, c] == 0)
                    {
                        board[i][r, c].Kill();
                        // board[i][r, c].gameObject.SetActive(false);

                    }
                    else
                    {

                        board[i][r, c].setTileType(levelData[r, c]);
                        // tasks.Add(board[i][r, c].Zoom(i));
                        int noise = UnityEngine.Random.Range(0, 5);
                        tasks.Add(board[i][r, c].MoveToRealPos((i + 1) * 100 + (Math.Abs(r - rows / 2 + noise)) * 50, Ease.OutCirc));
                        remainTile++;
                    }
                }
            }
        }

        await Task.WhenAll(tasks);

        shuffling = false;


    }


}
