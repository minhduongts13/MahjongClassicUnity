using System;
using System.Collections.Generic;
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
    public int[,] Grid { get; set; }
    public int MinX { get; set; }
    public int MinY { get; set; }
    public int MaxX { get; set; }
    public int MaxY { get; set; }
    public int Width => MaxX - MinX + 1;
    public int Height => MaxY - MinY + 1;
}

public class LevelLoader : MonoBehaviour
{
    private const int scale = 2;
    
    public static List<LevelGridData> levelGridDataList = new List<LevelGridData>();
    public static List<TextAsset> jsonMap = new List<TextAsset>();
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

    public static LevelGridData ConvertLevelToUnifiedGrid(Level level, int levelNumber)
    {
        int minX = int.MaxValue, minY = int.MaxValue;
        int maxX = int.MinValue, maxY = int.MinValue;

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
            var grid = new int[width, height];
            foreach (var tile in layer.tiles)
            {
                int intX = (int)(tile.x * scale);
                int intY = (int)(tile.y * scale);
                int gridX = intX - minX;
                int gridY = intY - minY;
                grid[gridX, gridY] = 1;
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
        for (int y = gridData.Height - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridData.Width; x++)
            {
                gridString += gridData.Grid[x, y] == 1 ? "1" : "0";
            }
            gridString += "\n";
        }
        Debug.Log(gridString);
    }

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

        return gridData.Grid[gridX, gridY] == 1;
    }

    public LevelGridData GetLevel(int levelNumber)
    {
        int realLevel = levelNumber % 30;
        if (realLevel == 0) realLevel = 30;
        levelGridDataList = ConvertToLevelGridData(jsonMap[levelNumber/30].text);
        return levelGridDataList.Find(level => level.levelNumber == realLevel);
    }

    public LayerGridData GetLayer(int levelNumber, int layerNumber)
    {
        var level = GetLevel(levelNumber);
        return level?.layers.Find(layer => layer.layerNumber == layerNumber);
    }
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


    void Start()
    {
        
        loadJson();
        var x = GetLevel(3091-30);
        PrintLevelGridData(x);
    }
}