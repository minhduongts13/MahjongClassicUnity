using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Cor
{
    public float x;
    public float y;
}

[System.Serializable]
public class Layer
{
    public int index;
    public List<Cor> tiles = new List<Cor>();
}

[System.Serializable]
public class Level
{
    public List<Layer> layers = new List<Layer>();
    public List<string> seeds = new List<string>();
}

[System.Serializable]
public class LevelDataWrapper
{
    public List<Level> levels = new List<Level>();
}

[System.Serializable]
public class LayerGridData
{
    public int layerNumber;
    public GridData gridData;

    public LayerGridData(int layerNum, GridData grid)
    {
        layerNumber = layerNum;
        gridData = grid;
    }
}

[System.Serializable]
public class LevelGridData
{
    public int levelNumber;
    public List<LayerGridData> layers;

    public LevelGridData(int levelNum)
    {
        levelNumber = levelNum;
        layers = new List<LayerGridData>();
    }
}

public class GridData
{
    public int LayerIndex { get; set; }
    public int[,] Grid { get; set; }   // [row (Y), col (X)]
    public int MinX { get; set; }
    public int MinY { get; set; }
    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public int Width => MaxX - MinX + 1;
    public int Height => MaxY - MinY + 1;
}

public class LevelLoader : MonoBehaviour
{
    System.Random rand = new System.Random();
    private const int scale = 2;
    public static LevelLoader instance;
    public static List<LevelGridData> levelGridDataList = new List<LevelGridData>();
    public static List<TextAsset> jsonMap = new List<TextAsset>();

    // Chuyển JSON sang LevelGridData
    public static List<LevelGridData> ConvertToLevelGridData(string jsonData)
    {
        var wrappedJson = "{\"levels\":" + jsonData + "}";
        var wrapper = JsonUtility.FromJson<LevelDataWrapper>(wrappedJson);

        var levelGridDataList = new List<LevelGridData>();

        for (int i = 0; i < wrapper.levels.Count; i++)
        {
            var level = wrapper.levels[i];
            var levelGridData = ConvertLevelToUnifiedGrid(level, i + 1);
            levelGridDataList.Add(levelGridData);
        }

        return levelGridDataList;
    }

    // Chuyển Level thành grid chuẩn [row, col] = [y, x]
    public static LevelGridData ConvertLevelToUnifiedGrid(Level level, int levelNumber)
    {
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

        // Tìm bounding box chung
        foreach (var layer in level.layers)
        {
            foreach (var tile in layer.tiles)
            {
                int intX = (int)(tile.x * scale);
                int intY = (int)(tile.y * scale);
                minX = Math.Min(minX, intX);
                minY = Math.Min(minY, intY);
                maxX = Math.Max(maxX, intX);
                maxY = Math.Max(maxY, intY);
            }
        }

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        var levelGridData = new LevelGridData(levelNumber);

        foreach (var layer in level.layers)
        {
            // grid[row, col] = grid[y, x]
            var grid = new int[height, width];

            foreach (var tile in layer.tiles)
            {
                int intX = (int)(tile.x * scale);
                int intY = (int)(tile.y * scale);

                int gridX = intX - minX; // col
                int gridY = intY - minY; // row

                grid[gridY, gridX] = 1;
            }

            var gridData = new GridData
            {
                LayerIndex = layer.index,
                Grid = grid,
                MinX = minX,
                MinY = minY,
                MaxX = maxX,
                MaxY = maxY
            };
            var layerGridData = new LayerGridData(layer.index, gridData);
            levelGridData.layers.Add(layerGridData);
        }

        return levelGridData;
    }

    // Debug: in ra lưới
    public static void PrintLevelGridData(LevelGridData levelGridData)
    {
        Debug.Log($" LEVEL {levelGridData.levelNumber} ");
        Debug.Log($" layers: {levelGridData.layers.Count}");

        foreach (var layerData in levelGridData.layers)
        {
            Debug.Log($"Layer {layerData.layerNumber} (Size: {layerData.gridData.Width}x{layerData.gridData.Height}):");
            PrintGrid(layerData.gridData);
        }
        Debug.Log("end");
    }

    public static void PrintGrid(GridData gridData)
    {
        string gridString = "";
        for (int y = gridData.Height - 1; y >= 0; y--) // in từ trên xuống dưới
        {
            for (int x = 0; x < gridData.Width; x++)
            {
                gridString += gridData.Grid[y, x] == 1 ? "1" : "0";
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
    }

    // Kiểm tra có tile tại tọa độ thế giới không
    public static bool HasTileAt(GridData gridData, float worldX, float worldY)
    {
        int intX = (int)(worldX * scale);
        int intY = (int)(worldY * scale);

        if (intX < gridData.MinX || intX > gridData.MaxX ||
            intY < gridData.MinY || intY > gridData.MaxY)
        {
            return false;
        }

        int gridX = intX - gridData.MinX;
        int gridY = intY - gridData.MinY;

        return gridData.Grid[gridY, gridX] == 1;
    }

    // Lấy level theo số thứ tự
    public LevelGridData GetLevel(int levelNumber)
    {
        int realLevel = levelNumber % 30;
        if (realLevel == 0) realLevel = 30;
        levelGridDataList = ConvertToLevelGridData(jsonMap[levelNumber / 30].text);
        return levelGridDataList.Find(level => level.levelNumber == realLevel);
    }

    public LayerGridData GetLayer(int levelNumber, int layerNumber)
    {
        var level = GetLevel(levelNumber);
        return level?.layers.Find(layer => layer.layerNumber == layerNumber);
    }

    // Load toàn bộ json
    public void loadJson()
    {
        int x = 1;
        for (int i = 0; i < 104; i++)
        {
            string fileName = $"levels/level_{x.ToString("D6")}-30";
            TextAsset asset = Resources.Load<TextAsset>(fileName);
            if (asset != null)
                jsonMap.Add(asset);
            x += 30;
        }
    }

    // Xuất grid của 1 level thành list<int[,]>
    public List<int[,]> getArray(LevelGridData lv)
    {
        List<int[,]> list = new List<int[,]>();
        foreach (LayerGridData layer in lv.layers)
        {
            // clone mảng gốc để không thay đổi dữ liệu ban đầu
            int[,] grid = (int[,])layer.gridData.Grid.Clone();
            RandomizePairs(grid);
            list.Add(grid);
        }
        AddSpecial(list);
        return list;
    }

    private void AddSpecial(List<int[,]> list)
    {
        rand = new System.Random();
        int i = rand.Next(0, list.Count);

        int[,] grid = list[i];
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        int x, y, type;
        do
        {
            x = rand.Next(0, cols);
            y = rand.Next(0, rows);
            type = grid[y, x];
        }
        while (type == 0);
        list[i][y, x] = rand.Next(0, 2) == 0 ? (int)SpecialTile.FLOWER : (int)SpecialTile.SEASON;
        foreach (int[,] g in list)
        {
            for (int r = 0; r < g.GetLength(0); r++)
            {
                for (int c = 0; c < g.GetLength(1); c++)
                {
                    if (g[r, c] == type)
                    {
                        g[r, c] = list[i][y, x];
                        return;
                    }
                }
            }
        }

    }


    private void RandomizePairs(int[,] grid)
    {
        List<(int r, int c)> ones = new List<(int, int)>();

        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // gom tất cả ô có value = 1
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (grid[r, c] == 1)
                {
                    ones.Add((r, c));
                }
            }
        }

        if (ones.Count == 0) return;

        // xáo trộn danh sách
        ones = ones.OrderBy(_ => rand.Next()).ToList();

        // xử lý theo từng cặp
        for (int i = 0; i + 1 < ones.Count; i += 2)
        {
            int val = rand.Next(1, 20); // 1..5
            var (r1, c1) = ones[i];
            var (r2, c2) = ones[i + 1];
            grid[r1, c1] = val;
            grid[r2, c2] = val;
        }

        // nếu dư 1 ô thì xử lý riêng
        if (ones.Count % 2 == 1)
        {
            var (r, c) = ones.Last();
            // Ví dụ: ép nó trùng với 1 ô trong grid (chọn random 1 giá trị đã dùng)
            int val = rand.Next(1, 6);
            grid[r, c] = val;
        }
    }


    void Start()
    {
        if (!LevelLoader.instance)
        {
            LevelLoader.instance = this;
            DontDestroyOnLoad(this.gameObject);
            loadJson();
            var x = GetLevel(3091 - 30);
            PrintLevelGridData(x);
        }
        else { Destroy(this.gameObject); }
    }
}
