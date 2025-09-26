using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

[System.Serializable]
public class Cor
{
    public float x;
    public float y;
    public int tile; // 1: có, 0: không
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
    public string seeds;

    public LevelGridData(int levelNum, string seeds)
    {

        levelNumber = levelNum;
        layers = new List<LayerGridData>();
        this.seeds = seeds;
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
        int count = 0;
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
                count++;

            }
        }

        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        var levelGridData = new LevelGridData(levelNumber, level.seeds[0]);
        List<int> decode = DecodePacked(level.seeds[0], count, 50);
        decode.Reverse();
        int seedIndex = 0;
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

                // grid[gridY, gridX] = decode[seedIndex];
                grid[gridY, gridX] = seedIndex % 2 == 0 ? 20 : 10;
                seedIndex++;
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
        ExportLevelToFile(levelGridData);
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
            list.Add(grid);
        }
        int c = 0;
        int OddCount = 0;
        Vector3 xy = new Vector3();
        foreach (int[,] arr in list)
        {
            for (int y = 0; y < arr.GetLength(0); y++)
            {
                for (int x = 0; x < arr.GetLength(1); x++)
                {
                    if (arr[y, x] != 0)
                    {
                        arr[y, x] = c % 2 == 0 ? 20 : 10;
                        xy = new Vector3(x, y, list.IndexOf(arr));
                        c++;
                        if (arr[y, x] == 10) OddCount++;
                    }
                }
            }
        }
        if (OddCount % 2 != 0)
        {
            list[(int)xy.z][(int)xy.y, (int)xy.x] = 20;
        }
        // int count = 0;

        // foreach (int[,] lst in list)
        // {
        //     foreach (int item in lst)
        //     {
        //         if (item != 0)
        //         {
        //             count++;
        //         }
        //     }
        // }
        // List<int> decode = DecodePacked(lv.seeds, count, 50);
        // int inx = 0;
        // foreach (int[,] lst in list)
        // {
        //     for (int y = 0; y < lst.GetLength(0); y++)
        //     {
        //         for (int x = 0; x < lst.GetLength(1); x++)
        //         {
        //             if (lst[y, x] != 0)
        //             {
        //                 lst[y, x] = decode[inx];
        //                 inx++;
        //             }
        //         }
        //     }

        // }
        return list;
    }



    private static int GetBitLength(int maxValue)
    {
        return (int)Math.Ceiling(Math.Log(maxValue, 2));
    }

    private static List<byte> Base64ToArray(string base64)
    {
        const string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
        var bytes = new List<byte>();

        for (int i = 0; i < base64.Length; i += 4)
        {
            int enc1 = base64Chars.IndexOf(base64[i]);
            int enc2 = base64Chars.IndexOf(base64[i + 1]);
            int enc3 = base64[i + 2] == '=' ? 0 : base64Chars.IndexOf(base64[i + 2]);
            int enc4 = base64[i + 3] == '=' ? 0 : base64Chars.IndexOf(base64[i + 3]);

            byte byte1 = (byte)((enc1 << 2) | (enc2 >> 4));
            byte byte2 = (byte)(((enc2 & 0b1111) << 4) | (enc3 >> 2));
            byte byte3 = (byte)(((enc3 & 0b11) << 6) | enc4);

            bytes.Add(byte1);
            if (base64[i + 2] != '=') bytes.Add(byte2);
            if (base64[i + 3] != '=') bytes.Add(byte3);
        }

        return bytes;
    }
    public static void ExportLevelToFile(LevelGridData levelGridData)
    {
        string path = Application.persistentDataPath + $"/level_{levelGridData.levelNumber}.txt";

        using (StreamWriter writer = new StreamWriter(path, false))
        {
            writer.WriteLine($"LEVEL {levelGridData.levelNumber}");
            writer.WriteLine($"Seeds: {levelGridData.seeds}");
            writer.WriteLine($"Layers: {levelGridData.layers.Count}");
            writer.WriteLine("=====================================");
            int totalTiles = 0;
            foreach (var layer in levelGridData.layers)
                totalTiles += layer.gridData.Grid.Cast<int>().Count(v => v != 0);
            List<int> decodeList = LevelLoader.DecodePacked(levelGridData.seeds, totalTiles, 50);
            writer.WriteLine("DECODED LIST:");
            writer.WriteLine(string.Join(", ", decodeList));
            writer.WriteLine("=====================================");

            foreach (var layerData in levelGridData.layers)
            {
                writer.WriteLine($"Layer {layerData.layerNumber} (Size: {layerData.gridData.Width}x{layerData.gridData.Height}):");

                // in grid
                for (int y = layerData.gridData.Height - 1; y >= 0; y--)
                {
                    string line = "";
                    for (int x = 0; x < layerData.gridData.Width; x++)
                    {
                        line += layerData.gridData.Grid[y, x].ToString("D2") + " ";
                    }
                    writer.WriteLine(line);
                }

                writer.WriteLine("-------------------------------------");
            }
        }

        Debug.Log($"Exported level {levelGridData.levelNumber} to: {path}");
    }

    public static List<int> DecodePacked(string base64, int itemCount, int maxValue = 50)
    {
        int bitsPerNumber = GetBitLength(maxValue);
        List<byte> buffer = Base64ToArray(base64);

        var result = new List<int>();
        int bitPos = 0;

        for (int n = 0; n < itemCount; n++)
        {
            int value = 0;
            for (int i = 0; i < bitsPerNumber; i++)
            {
                int byteIndex = bitPos / 8;
                int bitOffset = bitPos % 8;
                int bit = (buffer[byteIndex] >> bitOffset) & 1;

                value |= bit << i;
                bitPos++;
            }
            result.Add(value);
        }
        return result;
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
