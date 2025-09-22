using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoardManager : MonoBehaviour
{
    private List<int[,]> mockUpLevel;
    public List<Tile[,]> board;
    public int remainTile;


    void LevelMock()
    {
        mockUpLevel = new List<int[,]>();

        int[,] layer1 =
        {
            { 1,0,20, 0, 0, 1 },
            {0,0, 0, 0, 0, 0 },
            {0,1, 0, 0, 20, 0 },
            { 0, 0, 0, 0 ,0,0},
        };
        mockUpLevel.Add(layer1);

        int[,] layer2 =
        {
            { 0,0,1, 0, 0, 1 },
            {1,0, 0, 0, 0, 0 },
            {0,0, 0, 2, 0, 0 },
            { 0, 0, 0, 0 ,0,2},
        };
        mockUpLevel.Add(layer2);
    }

    public void SetUp()
    {
        remainTile = 0;
        Debug.Log("aaa");
        // LevelMock();
        // mockUpLevel = GameManager.instance.currentLevel.layers;
        Debug.Log("llalaal");
        LevelLoader.PrintLevelGridData(GameManager.instance.currentLevel);
        mockUpLevel = LevelLoader.instance.getArray(GameManager.instance.currentLevel);
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
                    t.transform.SetSiblingIndex(2 * (rows * (c + c % 2) + r + i * cols * rows) + (c % 2 == 0 ? 0 : 1));
                    t.idx = 2 * (rows * (c + c % 2) + r + i * cols * rows) + (c % 2 == 0 ? 0 : 1);
                    // t.transform.SetSiblingIndex((c - c % 2) * rows + (r - r % 2) * cols + i * rows * cols);
                    tileGrid[r, c] = t;
                    t.layer = i;
                    t.coords = new Vector2Int(c, r);
                    t.setTileType(levelData[r, c]);
                    // if (levelData[r, c] == 0)
                    // {
                    //     t.Kill();
                    // }
                }
            }



            board.Add(tileGrid);


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
                        (board[i][r, c].transform as RectTransform).anchoredPosition = GetPosFromCoords(c, r, i);
                    if (levelData[r, c] == 0)
                    {
                        board[i][r, c].Kill();
                        // board[i][r, c].gameObject.SetActive(false);

                    }
                    else
                    {

                        board[i][r, c].setTileType(levelData[r, c]);
                        board[i][r, c].Zoom(i);
                        remainTile++;
                    }
                }
            }
        }



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
        float layerOffsetX = -tileWidth * 0.2f * layerIndex;  // shift left
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

    public void ApplySiblingOrder(List<int[,]> levelData, List<Tile[,]> board)
    {
        List<(Tile tile, int index)> ordered = new List<(Tile, int)>();

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
                    if (data[r, c] == 0) continue; // ô trống
                    Tile t = grid[r, c];
                    if (t == null) continue;

                    // công thức index độc nhất
                    int index = 2 * (rows * (c - c % 2) + r + layer * cols * rows)
                                + c % 2;

                    ordered.Add((t, index));
                }
            }
        }

        // sort toàn bộ tile theo index
        ordered.Sort((a, b) => a.index.CompareTo(b.index));

        // gán sibling index tuần tự
        for (int i = 0; i < ordered.Count; i++)
        {
            ordered[i].tile.transform.SetSiblingIndex(i);
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
        tile.gameObject.SetActive(true);
        tile.Reset();
        Vector2 p = GetPosFromCoords(tile.coords.x, tile.coords.y, tile.layer);
        (tile.transform as RectTransform).anchoredPosition = p;
        tile.used = true;
    }

    public void Undo(Tuple<Tuple<Vector3, Vector3>, Tuple<int, int>> move)
    {
        RestoreTile(move.Item1.Item1, move.Item2.Item1);
        RestoreTile(move.Item1.Item2, move.Item2.Item2);
        ApplySiblingOrder(mockUpLevel, board);
    }


    public async Task Shuffle()
    {
        List<Tile> tilesList = new List<Tile>();
        List<int> typeList = new List<int>();
        List<Task> task = new List<Task>();
        List<Tile> freeTile = new List<Tile>();

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
                    task.Add(rt.DOAnchorPos(new Vector2(0, 0), 0.5f).AsyncWaitForCompletion());
                }
            }
        }

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
                Tile swapWith = tilesList.First(t => !freeTile.Contains(t) && t.GetTileType() == tileA.GetTileType());

                int tmp = tileB.GetTileType();
                tileB.setTileType(tileA.GetTileType());
                swapWith.setTileType(tmp);
            }
        }

        await Task.WhenAll(task);
        await Task.Delay(500);

        foreach (Tile[,] tiles in board)
        {
            foreach (Tile t in tiles)
            {
                if (t != null && t.GetTileType() != 0)
                {
                    t.MoveToRealPos();
                }
            }
        }
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


}
